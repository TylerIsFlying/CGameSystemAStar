using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TAPathing
{

    // moves the object towards the target
    public static List<TANode> SetTarget(Vector3 seeker, Vector3 target, TAGrid grid)
    {
        // Starting the thread for it.
        List<TANode> tmpPath = new List<TANode>();
        Thread thread = new Thread(() => GetTargetPath(seeker,target, grid.NodeFromWorldPoint(seeker), grid.NodeFromWorldPoint(target), out tmpPath));
        thread.Start();
        while (thread.IsAlive) ; // dont do anything if the thread is alive
        return tmpPath;
    }
    // returns a path of the target
    private static void GetTargetPath(Vector2 seeker, Vector2 target,TANode startNode, TANode endNode, out List<TANode> path)
    {
        path = FindPath(seeker, target, startNode, endNode);
    }

    // finds the path to the target
    private static List<TANode> FindPath(Vector2 seeker, Vector2 target,TANode startNode, TANode endNode) 
    {
        // setting it up the closed and open
        List<TANode> open = new List<TANode>();
        HashSet<TANode> closed = new HashSet<TANode>();
        open.Add(startNode);
        while(open.Count > 0) 
        {
            TANode node = open[0];

            for(int i = 1; i < open.Count; i++) 
            {
                if(open[i].fCost < node.fCost || open[i].fCost == node.fCost) 
                {
                    if(open[i].hCost < node.hCost) 
                    {
                        node = open[i];
                    }
                }
            }
            open.Remove(node);
            closed.Add(node);

            if(node == endNode) 
            {
                return RetracePath(startNode, endNode);
            }

            foreach(TANode neighbor in node.connections) 
            {
                if(neighbor.ignore || closed.Contains(neighbor)) 
                {
                    continue;
                }
                int newCostToNeighbour = node.gCost + GetDistance(node, neighbor);
                if(newCostToNeighbour < neighbor.gCost || !open.Contains(neighbor)) 
                {
                    neighbor.gCost = newCostToNeighbour;
                    neighbor.hCost = GetDistance(neighbor,endNode);
                    neighbor.parent = node;

                    if (!open.Contains(neighbor)) 
                    {
                        open.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }



    private static List<TANode> RetracePath(TANode startNode, TANode endNode) 
    {
        List<TANode> path = new List<TANode>();
        TANode currentNode = endNode;

        while(currentNode != startNode) 
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
    private static int GetDistance(TANode n1, TANode n2)
    {
        int dstX = Mathf.Abs(n1.gridX - n2.gridX);
        int dstY = Mathf.Abs(n1.gridY - n2.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
