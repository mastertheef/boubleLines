using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFinder : MonoBehaviour
{
    private Node start, goal;
    private TileGraph graph;

    Queue<Node> frontierNodes;
    List<Node> exploredNodes;

    bool isComplete = false;


    public void Init(TileGraph graph)
    {
        this.graph = graph;
        frontierNodes = new Queue<Node>();
        exploredNodes = new List<Node>();

        for (int x = 0; x < graph.Width; x++)
        {
            for (int y = 0; y < graph.Height; y++)
            {
                graph.tileNodes[x, y].Reset();
            }
        }
    }

    public void Reset()
    {
        Init(graph);
    }

    public List<Node> FindPath(Node start, Node goal)
    {
        this.start = start;
        this.goal = goal;

        frontierNodes.Enqueue(start);

        while (!isComplete)
        {
            if (frontierNodes.Any())
            {
                var currentNode = frontierNodes.Dequeue();

                if (!exploredNodes.Contains(currentNode))
                {
                    exploredNodes.Add(currentNode);
                }
                ExpandFrontier(currentNode);
            }
            else
            {
                isComplete = true;
            }
        }
        List<Node> pathNodes = new List<Node>();

        pathNodes.Add(goal);
        var current = goal.previousNode;

        while (current != null)
        {
            pathNodes.Insert(0, current);
            current = current.previousNode;
        }
        isComplete = false;
        return pathNodes;

    }

    private void ExpandFrontier(Node node)
    {
        for (int i = 0;  i < node.neighbourNodes.Count; i++)
        {
            if (!exploredNodes.Contains(node.neighbourNodes[i]) &&
                !frontierNodes.Contains(node.neighbourNodes[i]))
            {
                node.neighbourNodes[i].previousNode = node;
                frontierNodes.Enqueue(node.neighbourNodes[i]);
            }
        }
    }

    
}
