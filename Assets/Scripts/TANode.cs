using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TANode
{
    // hCost and gCost for our nodes that we use
    public int gCost, hCost;

    // Saves the locations its at on the grid
    public int gridX;
    public int gridY;

    // parent node
    public TANode parent;

    // fCost is how much the gCost added with the hCost is.
    public int fCost 
    {
        get
        {
            return gCost + hCost; 
        } 
    }
    // Array of the connections we have
    public TANode[] connections;
    // Position of the node
    public Vector3 position;
    // Used to make sure it will ignore any sort of wall
    public bool ignore = false;
}
