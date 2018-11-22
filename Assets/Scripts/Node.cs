using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Open,
    Closed
}

public class Node
{
    public NodeType nodeType;
    public int x, y = -1; // matrix coordinates
    public Vector2 tileCoordinates;
    public Ball ball;
    public TileView tile;

    public List<Node> neighbourNodes;
    public Node previousNode = null;

    public Node(int x, int y, NodeType nodeType)
    {
        neighbourNodes = new List<Node>();
        this.x = x;
        this.y = y;
        this.nodeType = nodeType;
    }

    public void Reset()
    {
        previousNode = null;
    }
}
