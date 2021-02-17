using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TAGrid : MonoBehaviour
{

    // struct for thr gridSize
    private struct GridSize
    {
        public int x, y;
    }

    // variables
    // gets the grid
    public TANode[,] gridGlobal { get { return _grid; } }
    public float nodeDiameterGlobal { get { return nodeRadius * 2; } }
    public float nodeRadiusGlobal { get { return nodeRadius; } }
    [Header("Radius")]
    [SerializeField]
    private float nodeRadius;
    [Header("Grid")]
    [SerializeField]
    private Vector2 gridWorldSize;
    [Header("Layers")]
    [SerializeField]
    private LayerMask ignoreLayer;
    [Header("Gizmos")]
    [SerializeField]
    private bool enableGizmos;
    [SerializeField]
    private bool enableConnectionVisual;
    [SerializeField]
    private Color gizmoWallColors;
    [SerializeField]
    private Color gizmoNodeColors;
    [SerializeField]
    private Color gizmoConnectionsColor;

    // private vars
    private bool finishedGrid = false;
    private Thread thread;
    private TANode[,] _grid;
    private GridSize gridSize;
    private float nodeDiameter;
    // end of varibles
    private void Awake()
    {
        // Start the grid threading
        GridThreading();
    }

    // function to start the grid threading
    private void GridThreading() 
    {
        // setting renderer and bounds
        nodeDiameter = nodeRadius * 2;
        // setting up the grid
        gridSize.x = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSize.y = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        // telling it to stop if its currently using a thread for it
        if (thread != null) thread.Abort();
        // crate a new thread for it to run on.
        Vector3 pos = gameObject.transform.position;
        thread = new Thread(() => CalGrid(pos,out finishedGrid, out _grid));
        // start the thread
        thread.Start();

        
        while (thread.IsAlive) 
        {
            // waiting till the thread is not alive
        }
        // puts all the ones we need to ignore
        _grid = CheckIgnore(_grid);
        // start a new thread after it dies.
        thread = new Thread(() => SetConnections(pos, _grid, out _grid));
        thread.Start();
        
    }
    // will be used to calulate the grid
    private void CalGrid(Vector3 position,out bool finished,out TANode[,] grid) 
    {
        grid = CreateGrid(position);
        finished = true;
    }

    // will switch the array
    private TANode[] GUpdateArray(TANode[] arr, int newSize, TANode nodeAdded) 
    {
        TANode[] tmp = arr;
        arr = new TANode[newSize];
        for(int i = 0; i < tmp.Length; i++) 
        {
            arr[i] = tmp[i];
        }
        arr[newSize - 1] = nodeAdded;
        return arr;
    }

    // finds the nearest node
    private TANode GetNearestNode(Vector3 position) 
    {
        foreach(TANode n in _grid) 
        {
            if(Vector3.Distance(new Vector3(n.position.x, 0.0f,n.position.z), new Vector3(position.x,0.0f, position.z)) < 1.0f)
            {
                return n;
            }
        }
        return new TANode();
    }
    // gets closest node to the targets position
    public TANode NodeFromWorldPoint(Vector3 position) 
    {
        TANode node = new TANode();
        Thread nThread = new Thread(() => { node = GetNearestNode(position); }) ;
        nThread.Start();
        while (nThread.IsAlive) 
        {
            // wait for it till it stops
        }
        return node;
    }
    private TANode[,] CheckIgnore(TANode[,] grid) 
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y].ignore = (Physics.CheckSphere(grid[x,y].position, nodeRadius, ignoreLayer));
                if (grid[x, y].ignore) 
                {
                    // used to just have to put around it around it more to make it easy for it to ignore the it and go smoothly around it.
                    int adder = 1;
                    
                    grid[x + adder, y + adder].ignore = true;
                    grid[x - adder, y - adder].ignore = true;

                    grid[x + adder, y - adder].ignore = true;
                    grid[x + adder, y - adder].ignore = true;

                    grid[x - adder, y + adder].ignore = true;
                    grid[x - adder, y + adder].ignore = true;
                    
                }
            }
        }
        return grid;
    }
    // Creates the grid
    private TANode[,] CreateGrid(Vector3 position)
    {
        TANode[,] grid = new TANode[gridSize.x, gridSize.y];
        Vector3 worldBottomLeft = position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //bool ignore = !(Physics.CheckSphere(worldPoint, nodeRadius, ignoreLayer));
                TANode node = new TANode();
                node.position = worldPoint;
            //node.ignore = ignore;
            grid[x, y] = node;
            }
        }
        return grid;
    }
    
    // sets our grid connections
    private void SetConnections(Vector3 position, TANode[,] gridInput,out TANode[,] grid) 
    {
        // Set connections
        if (gridInput == null) 
        {
            grid = gridInput;
            return;
        }
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                gridInput[x, y] = SetConnection(gridInput, x, y);
            }
        }
        grid = gridInput;
    }

    // use to the the node
    private TANode SetConnection(TANode[,] grid, int x, int y)
    {
        int cx, cy, counter = 0;

        // set node to be eqaul to nodes[x,y]
        TANode node = grid[x, y];
        node.gridX = x;
        node.gridY = y;
        // setting up node connections variable
        if (node.connections == null)
        {
            node.connections = new TANode[1];
        }
        // checking x

        // checking x above
        cx = x + 1;
        if (cx >= 0 && cx < grid.GetLength(0))
        {
            node.connections = GUpdateArray(node.connections, ++counter, grid[cx, y]);
        }
        // checking x below
        cx = x - 1;
        if (cx >= 0 && cx < grid.GetLength(0))
        {
            node.connections = GUpdateArray(node.connections, ++counter, grid[cx, y]);
        }
        // checking y
        cy = y + 1;
        if (cy >= 0 && cy < grid.GetLength(1))
        {
            node.connections = GUpdateArray(node.connections, ++counter, grid[x, cy]);
        }
        // checking z below
        cy = y - 1;
        if (cy >= 0 && cy < grid.GetLength(1))
        {
            node.connections = GUpdateArray(node.connections, ++counter, grid[x, cy]);
        }
        return node;

    }
    

    // Draws the gizmos on the screen for us
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // draws the gizmos if the game is currently running
            if (enableGizmos)
            {
                foreach (TANode n in _grid)
                {
                    Gizmos.color = (n.ignore) ? Color.blue : Color.red;
                    Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter - 0.1f));
                    foreach (TANode c in n.connections)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(n.position, c.position);
                    }
                }
            }
        }
        else 
        {
            // Draws gizmos and updates it when game is not active
            if (!enableGizmos)
            {
                finishedGrid = false;
                return;
            }
            if (!finishedGrid)
            {
                GridThreading();
            }
            else
            {
                if (_grid != null)
                {
                    foreach (TANode n in _grid)
                    {
                        Gizmos.color = (n.ignore) ? new Color(gizmoWallColors.r, gizmoWallColors.g, gizmoWallColors.b) : new Color(gizmoNodeColors.r, gizmoNodeColors.g, gizmoNodeColors.b);
                        Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter - 0.1f));
                        if (enableConnectionVisual) 
                        {
                            foreach (TANode c in n.connections)
                            {
                                Gizmos.color = new Color(gizmoConnectionsColor.r, gizmoConnectionsColor.g, gizmoConnectionsColor.b);
                                Gizmos.DrawLine(n.position, c.position);
                            }
                        }
                    }
                }
            }
        }
    }
}