using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGraph : MonoBehaviour {

    public Node[,] tileNodes;
    public List<Node> nodesWithBalls = new List<Node>();
    int width, height;
    public float ballAcceleration = 0.1f;


    public int Width { get { return width; } }
    public int Height { get { return height; } }

    public delegate void BallMovedHandler();
    public event BallMovedHandler OnBallMoved;

    public static readonly Vector2[] directions =
    {
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(1, -1),
        new Vector2(0, -1),
        new Vector2(-1, -1),
        new Vector2(-1, 0),
        new Vector2(-1, 1)
    };

    public void Init(int[,] tileMatrix)
    {
        width = tileMatrix.GetLength(0);
        height = tileMatrix.GetLength(0);

        tileNodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var nodeType = (NodeType)tileMatrix[x, y];
                var newNode = new Node(x, y, nodeType);
                tileNodes[x, y] = newNode;
            }
        }
    }

    public void InitNeighbours()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tileNodes[x, y].nodeType != NodeType.Closed)
                {
                    tileNodes[x, y].neighbourNodes = GetNeighbours(x, y);
                }
            }
        }
    }

    public void AddBall(int x, int y, GameObject ball)
    {
        tileNodes[x, y].ball = ball;
        nodesWithBalls.Add(tileNodes[x, y]);
    }

    public void RemoveBall(int x, int y)
    {
        Destroy(tileNodes[x, y].ball.gameObject);
        tileNodes[x, y].ball = null;
        nodesWithBalls.Remove(tileNodes[x, y]);
    }

    public void RemoveBall(Node node)
    {
        RemoveBall(node.x, node.y);
    }

    private void MoveBall(Node start, Node end)
    {
        if (start.ball != null && end.ball == null)
        {
            //start.ball.transform.position = end.tile.transform.position;
            end.ball = start.ball;
            start.ball = null;
            nodesWithBalls.Remove(start);
            nodesWithBalls.Add(end);
        }
    }

    public IEnumerator MoveBall(List<Node> path, float speed)
    {
        var pathQueue = new Queue<Node>(path);
        var start = pathQueue.Dequeue();
        start.ball.GetComponentInChildren<Ball>().Deselect();
        while (pathQueue.Any())
        {
            var next = pathQueue.Dequeue();
            while (start.ball.transform.position != next.tile.transform.position)
            {
                start.ball.transform.position = Vector2.MoveTowards(start.ball.transform.position, next.tile.transform.position, speed * Time.deltaTime);
                speed += ballAcceleration;
                yield return null;
            }
        }
        
        MoveBall(path.First(), path.Last());
        OnBallMoved();
    }

    public void AnalizeAndDestroyBalls(int minLineLength, GameObject destroyEffect)
    {
        List<Node> nodeBallsToDestroy = new List<Node>();
        foreach (var ballNode in nodesWithBalls)
        {
            nodeBallsToDestroy.AddRange(getNodeLines(ballNode, minLineLength));
        }

        var distinctList = nodeBallsToDestroy.Distinct().ToList();
        if (distinctList.Any())
        {
            var color = distinctList[0].ball.GetComponentInChildren<Ball>().Color;
            foreach (var ballNode in distinctList)
            {
                var blow = Instantiate(destroyEffect, ballNode.tile.transform.position, Quaternion.identity);
                var blowMain = blow.GetComponent<ParticleSystem>().main;
                blowMain.startColor = color;
                RemoveBall(ballNode);
                StartCoroutine(Camera.main.GetComponent<RipplePostProcessor>().Ripple(ballNode.tile.transform.position));
            }
        }

        InitNeighbours();
    }

    private List<Node> getNodeLines(Node node, int minLineLength)
    {
        List<Node> nodesToDestroy = new List<Node>();
        foreach (var dir in directions)
        {
            List<Node> line = new List<Node>();
            var currentNode = node;
            line.Add(currentNode);
            int neighbourX = currentNode.x + (int)dir.x;
            int neighbourY = currentNode.y + (int)dir.y;
            var neighbourNode = IsInBounds(neighbourX, neighbourY)
                ? tileNodes[neighbourX, neighbourY]
                : null;

            while (currentNode != null && neighbourNode != null && 
                currentNode.ball != null && neighbourNode.ball != null && currentNode.ball.GetComponentInChildren<Ball>().Color == neighbourNode.ball.GetComponentInChildren<Ball>().Color)
            {
                line.Add(neighbourNode);
                neighbourX += (int)dir.x;
                neighbourY += (int)dir.y;

                currentNode = neighbourNode;
                neighbourNode = IsInBounds(neighbourX, neighbourY)
                ? tileNodes[neighbourX, neighbourY]
                : null;
            }

            if (line.Count >= minLineLength)
            {
                nodesToDestroy.AddRange(line);
            }
        }

        return nodesToDestroy.Distinct().ToList();
    }


    private bool IsInBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    private List<Node> GetNeighbours(int x, int y)
    {
        var result = new List<Node>();

        foreach (var dir in directions)
        {
            int newX = x + (int)dir.x;
            int newY = y + (int)dir.y;

            if (IsInBounds(newX, newY) && 
                tileNodes[newX, newY] != null && 
                tileNodes[newX, newY].nodeType != NodeType.Closed && 
                tileNodes[newX, newY].ball == null )
            {
                result.Add(tileNodes[newX, newY]);
            }
        }

        return result;
    }
}
