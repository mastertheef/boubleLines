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
    public int lineLength = 3;
    public int startingBallCount = 3;
    public int ballsPerTurn = 2;

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


                    if (startNode == null && tile.node.ball != null)
                    {
                        startNode = tile.node;
                    }
                    else if (startNode != null && tile.node.ball == null)
                    {
                        endNode = tile.node;

                        var path = pathFinder.FindPath(startNode, endNode);

                        if (path.Contains(startNode) && path.Contains(endNode))
                        {
                            tileGraph.MoveBall(startNode, endNode);
                            GenerateBalls(ballsPerTurn);
                            tileGraph.AnalizeAndDestroyBalls(lineLength);
                            tileGraph.InitNeighbours();
                        }
                        

                        //path.ForEach(x => x.tile.GetComponent<SpriteRenderer>().color = Color.red);

                        startNode = null;
                        endNode = null;
                        pathFinder.Init(tileGraph);
                        
                    }
                }
            }
        }
    }

    private bool addBall(Vector2 coords)
    {
        if (!tileGraph.nodesWithBalls.Any(x => x.x == coords.x && x.y == coords.y))
        {
            var ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            ball.transform.position = new Vector2(tileSize.x * coords.x - offset, tileSize.y * coords.y - offset);
            ball.GetComponent<Ball>().Color = possibleColors[Random.Range(0, possibleColors.Length)];
            tileGraph.AddBall((int)coords.x, (int)coords.y, ball.GetComponent<Ball>());
            return true;
        }

        return false;
    }
}
