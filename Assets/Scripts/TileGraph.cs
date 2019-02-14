using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGraph : MonoBehaviour {

    public Node[,] tileNodes;
    public List<Node> nodesWithBalls = new List<Node>();
    public List<Node> emptyNodes = new List<Node>();
    int width, height;
    public float ballAcceleration = 0.1f;

    public GameObject destroyEffect;

    public int Width { get { return width; } }
    public int Height { get { return height; } }

    public delegate void BallMovedHandler(Node start, Node end);
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
                emptyNodes.Add(newNode);
            }
        }
    }

    public void InitNeighbours()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //tileNodes[x, y].tile.GetComponent<SpriteRenderer>().color = Color.white;
                if (tileNodes[x, y].nodeType != NodeType.Closed)
                {
                    tileNodes[x, y].neighbourNodes = GetNeighbours(x, y);
                }
            }
        }

        //foreach (var node in nodesWithBalls)
        //{
        //    node.tile.GetComponent<SpriteRenderer>().color = Color.red;
        //}
    }

    public void AddBall(int x, int y, GameObject ball)
    {
        tileNodes[x, y].ball = ball;
        nodesWithBalls.Add(tileNodes[x, y]);
        emptyNodes.Remove(tileNodes[x, y]);
    }

    public void RemoveBall(int x, int y)
    {
        Destroy(tileNodes[x, y].ball.gameObject);
        tileNodes[x, y].ball = null;
        nodesWithBalls.Remove(tileNodes[x, y]);
        emptyNodes.Add(tileNodes[x, y]);
    }

    public void RemoveBall(Node node)
    {
        RemoveBall(node.x, node.y);
    }

    public void MoveBall(Node start, Node end)
    {
        if (start.ball != null && end.ball == null)
        {
            //start.ball.transform.position = end.tile.transform.position;
            end.ball = start.ball;
            start.ball = null;
            nodesWithBalls.Remove(start);
            nodesWithBalls.Add(end);
            emptyNodes.Remove(end);
            emptyNodes.Add(start);

        }
    }

    public IEnumerator MoveBall(List<Node> path, float speed, bool triggerEvent = true)
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
        if (triggerEvent)
        {
            OnBallMoved(path.First(), path.Last());
        }

        InitNeighbours();
    }

    public IEnumerator ShuffleMove(GameObject ball, Node end, float speed, Dictionary<int, bool> sync, int i)
    {
        end.ball = ball;
        nodesWithBalls.Add(end);
        emptyNodes.Remove(end);
        while (ball.transform.position != end.tile.transform.position)
        {
            ball.transform.position = Vector2.MoveTowards(ball.transform.position, end.tile.transform.position, speed * Time.deltaTime);
            speed += ballAcceleration;
            yield return null;
        }
        
        sync[i] = true;
    }

    public void DestroyBalls(FoundLines lines, Node startNode)
    {
        List<Node> nodeBallsToDestroy = new List<Node>();
        nodeBallsToDestroy.AddRange(lines.horizontal);
        nodeBallsToDestroy.AddRange(lines.vertical);
        nodeBallsToDestroy.AddRange(lines.leftDiag);
        nodeBallsToDestroy.AddRange(lines.rightDiag);

        DestroyBalls(nodeBallsToDestroy, startNode);
    }

    public void DestroyBalls(List<Node> nodeBallsToDestroy, Node startNode)
    {
        var distinctList = nodeBallsToDestroy.Distinct().ToList();
        if (distinctList.Any())
        {
            StartCoroutine(Camera.main.GetComponent<RipplePostProcessor>().Ripple(startNode.tile.transform.position));
            
            foreach (var ballNode in distinctList)
            {
                var color = ballNode.ball.GetComponentInChildren<Ball>().Color;
                var blow = Instantiate(destroyEffect, ballNode.tile.transform.position, Quaternion.identity);
                var blowMain = blow.GetComponent<ParticleSystem>().main;
                blowMain.startColor = color;
                RemoveBall(ballNode);
            }
        }

        InitNeighbours();
    }

    public void DestroySIngleBall(Node ballNode)
    {
        var color = ballNode.ball.GetComponentInChildren<Ball>().Color;
        var blow = Instantiate(destroyEffect, ballNode.tile.transform.position, Quaternion.identity);
        var blowMain = blow.GetComponent<ParticleSystem>().main;
        blowMain.startColor = color;
        RemoveBall(ballNode);
    }

    public FoundLines FindLines(Node node, int minLineLength)
    {
        var result = new FoundLines();

        List<Node> nodeBallsToDestroy = new List<Node>();
        // hirizintal line
        var horizontalLine = getLine(node, new Vector2(0, 1));
        horizontalLine.AddRange(getLine(node, new Vector2(0, -1)));
        horizontalLine = horizontalLine.Distinct().ToList();

        result.horizontal = (horizontalLine.Count >= minLineLength)
            ? horizontalLine
            : new List<Node>();

        // vertical line
        var verticalLine = getLine(node, new Vector2(1, 0));
        verticalLine.AddRange(getLine(node, new Vector2(-1, 0)));
        verticalLine = verticalLine.Distinct().ToList();

        result.vertical = (verticalLine.Count >= minLineLength)
            ? verticalLine
            : new List<Node>();

        // right diagonal
        var rightDiag = getLine(node, new Vector2(1, 1));
        rightDiag.AddRange(getLine(node, new Vector2(-1, -1)));
        rightDiag = rightDiag.Distinct().ToList();

        result.rightDiag = (rightDiag.Count >= minLineLength)
            ? rightDiag
            : new List<Node>();

        // right diagonal
        var leftDiag = getLine(node, new Vector2(1, -1));
        leftDiag.AddRange(getLine(node, new Vector2(-1, 1)));
        leftDiag = leftDiag.Distinct().ToList();

        result.leftDiag = (leftDiag.Count >= minLineLength)
            ? leftDiag
            : new List<Node>();

        return result;
    }


    private List<Node> getLine(Node node, Vector2 direction)
    {
        List<Node> line = new List<Node>();
        var currentNode = node;
        line.Add(currentNode);
        int neighbourX = currentNode.x + (int)direction.x;
        int neighbourY = currentNode.y + (int)direction.y;
        var neighbourNode = IsInBounds(neighbourX, neighbourY)
            ? tileNodes[neighbourX, neighbourY]
            : null;

        while (currentNode != null && neighbourNode != null &&
            currentNode.ball != null && neighbourNode.ball != null && currentNode.ball.GetComponentInChildren<Ball>().Color == neighbourNode.ball.GetComponentInChildren<Ball>().Color)
        {
            line.Add(neighbourNode);
            neighbourX += (int)direction.x;
            neighbourY += (int)direction.y;

            currentNode = neighbourNode;
            neighbourNode = IsInBounds(neighbourX, neighbourY)
            ? tileNodes[neighbourX, neighbourY]
            : null;
        }

        return line;
    }

    public bool IsInBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    public Node GetRandomEmpty()
    {
        return emptyNodes[Random.Range(0, emptyNodes.Count)];
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
