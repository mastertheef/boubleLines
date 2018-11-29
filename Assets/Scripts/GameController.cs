using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    public TileMatrix tileMatrix;
    public TileGraph tileGraph;
    public GameObject tileViewPrefab;
    public GameObject ballPrefab;

    public PathFinder pathFinder;
    public SoundController soundController;
    public ScoreController scoreController;
    public int lineLength = 5;
    public int startingBallCount = 3;
    public int ballsPerTurn = 2;
    public float ballSpeed = 5;

    public Color[] possibleColors;

    private Node startNode, endNode = null;

    public float offset = 4.5f;
    Vector3 tileSize;

    int[,] currentMatrix;
    // Use this for initialization
    void Start()
    {
        currentMatrix = tileMatrix.MakeStartMap();
        
        tileGraph.Init(currentMatrix);
        tileGraph.OnBallMoved += OnBallMoved;

        var graphView = tileGraph.gameObject.GetComponent<TileGraphView>();
        graphView.tileViewPrefab = tileViewPrefab;
        tileSize = tileViewPrefab.GetComponent<SpriteRenderer>().bounds.size;
        
        graphView.Init(tileGraph, offset, tileSize);
        GenerateBalls(startingBallCount);
        tileGraph.InitNeighbours();

        pathFinder.Init(tileGraph);
    }

    private void GenerateBalls(int count)
    {
        int width = currentMatrix.GetLength(0);
        int height = currentMatrix.GetLength(1);
        int emptyCount = width * height - tileGraph.nodesWithBalls.Count;
        int ballsGenerated = 0;

        while (ballsGenerated < count && emptyCount != 0)
        {
            var coords = new Vector2(Random.Range(0, width), Random.Range(0, height));
            if (addBall(coords))
            {
                ballsGenerated++;
                emptyCount--;

                var lines = tileGraph.FindLines(tileGraph.tileNodes[(int)coords.x, (int)coords.y], lineLength);
                if (lines.HaveLines(lineLength))
                {
                    tileGraph.DestroyBalls(lines, tileGraph.tileNodes[(int)coords.x, (int)coords.y]);
                    soundController.PlayDestroy();
                    scoreController.AddScore(lines, lineLength);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.transform.tag == "Tile")
                {
                    var tile = hit.transform.gameObject.GetComponent<TileView>();

                    if (hasBall(tile))
                    {
                        Select(tile);
                    }
                    else if (isSelected())
                    {
                        endNode = tile.node;

                        var path = pathFinder.FindPath(startNode, endNode);

                        if (path.Contains(startNode) && path.Contains(endNode))
                        {
                            StartCoroutine(tileGraph.MoveBall(path, ballSpeed));
                        }


                        //path.ForEach(x => x.tile.GetComponent<SpriteRenderer>().color = Color.red);

                        Deselect();
                        endNode = null;
                        pathFinder.Init(tileGraph);

                    }
                }
            }
        }
    }

    private void OnBallMoved(Node target)
    {
        var foundLines = tileGraph.FindLines(target, lineLength);
        if (!foundLines.HaveLines(lineLength))
        {
            soundController.PlayMove();
        }
        else
        {
            tileGraph.DestroyBalls(foundLines, target);
            soundController.PlayDestroy();
        }
        scoreController.AddScore(foundLines, lineLength);
        GenerateBalls(ballsPerTurn);
        tileGraph.InitNeighbours();
        
    }
   
    private bool addBall(Vector2 coords)
    {
        var parent = GameObject.Find("Boubles");
        if (!tileGraph.nodesWithBalls.Any(x => x.x == coords.x && x.y == coords.y))
        {
            var ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity, parent.transform);
            
            ball.transform.position = new Vector2(tileSize.x * coords.x - offset, tileSize.y * coords.y - offset);
            ball.GetComponentInChildren<Ball>().Color = possibleColors[Random.Range(0, possibleColors.Length)];
            tileGraph.AddBall((int)coords.x, (int)coords.y, ball);

            return true;
        }

        return false;
    }

    private bool isSelected()
    {
        return startNode != null;
    }

    private bool hasBall(TileView tile)
    {
        return tile.node.ball != null;
    }

    private void  Select(TileView tile)
    {
        if (startNode != null)
        {
            startNode.ball.GetComponentInChildren<Ball>().Deselect();
        }

        startNode = tile.node;
        startNode.ball.GetComponentInChildren<Ball>().Select();
    }

    private void Deselect()
    {
        if (endNode != null && endNode.ball != null)
        {
            endNode.ball.GetComponentInChildren<Ball>().Deselect();
        }
        startNode = null;
    }
}
