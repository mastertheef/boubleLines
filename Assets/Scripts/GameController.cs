using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Models;

public class GameController : MonoBehaviour
{
    public TileMatrix tileMatrix;
    public TileGraph tileGraph;
    public GameObject tileViewPrefab;
    public GameObject ballPrefab;

    public PathFinder pathFinder;
    public SoundController soundController;
    public ScoreController scoreController;
    public BonusController bonusController;
    public HistoryController historyController;
    public int lineLength = 5;
    public int startingBallCount = 3;
    public int ballsPerTurn = 2;
    public float ballSpeed = 5;

    public Color[] possibleColors;
    public float offset = 4.5f;

    private Vector3 tileSize;
    private Node startNode, endNode = null;
    private bool gameOver = false;
    private Settings currentSettings;

    int[,] currentMatrix;
    // Use this for initialization
    void Start()
    {
        gameOver = false;

        currentSettings = FileController.GetSettings();
        soundController.SetBubbleVolume(currentSettings.BubbleVolume);

        currentMatrix = tileMatrix.MakeStartMap();

        tileGraph.Init(currentMatrix);
        tileGraph.OnBallMoved += OnBallMoved;

        var graphView = tileGraph.gameObject.GetComponent<TileGraphView>();
        graphView.tileViewPrefab = tileViewPrefab;
        tileSize = tileViewPrefab.GetComponent<SpriteRenderer>().bounds.size;

        graphView.Init(tileGraph, offset, tileSize);

        var move = new MoveState();

        GenerateBalls(startingBallCount, move);
        historyController.AddMove(move);
        tileGraph.InitNeighbours();

        pathFinder.Init(tileGraph);
    }

    private void GenerateBalls(int count, MoveState move)
    {
        int width = currentMatrix.GetLength(0);
        int height = currentMatrix.GetLength(1);
        int emptyCount = width * height - tileGraph.nodesWithBalls.Count;
        int ballsGenerated = 0;
        var result = new List<Node>();

        while (ballsGenerated < count && emptyCount != 0)
        {
            var coords = new Vector2(Random.Range(0, width), Random.Range(0, height));
            if (addBall(coords))
            {
                ballsGenerated++;
                emptyCount--;

                if (move != null)
                {
                    move.Appeared.Add(new BallState(tileGraph.tileNodes[(int)coords.x, (int)coords.y]));
                }
                var lines = tileGraph.FindLines(tileGraph.tileNodes[(int)coords.x, (int)coords.y], lineLength);

                if (lines.HaveLines(lineLength))
                {
                    int scoreAdded = scoreController.AddScore(lines, lineLength);
                    tileGraph.DestroyBalls(lines, tileGraph.tileNodes[(int)coords.x, (int)coords.y]);
                    soundController.PlayDestroy();
                    if (move != null)
                    {
                        move.AddDestroyedAfterAppear(lines.GetAll(lineLength));
                        move.ScoreAdded += scoreAdded;
                    }
                }
            }
        }
    }

    public void StepBack()
    {
        var prevMove = bonusController.StepBack();
        if (prevMove == null)
        {
            return;
        }

        // reappear all destroyed after appeart
        if (prevMove.DestroyedAfterAppear.Any())
        {
            foreach (var ballState in prevMove.DestroyedAfterAppear)
            {
                addBall(new Vector2(ballState.x, ballState.y), ballState.Color);
            }
        }

        tileGraph.InitNeighbours();
        // remove all appeared
        List<Node> appeared = new List<Node>();
        prevMove.Appeared.ForEach(x => appeared.Add(tileGraph.tileNodes[x.x, x.y]));
        tileGraph.DestroyBalls(appeared, appeared.First());
        tileGraph.InitNeighbours();

        // reappear ll destroyed after move
        if (prevMove.DestroyedAfterMove.Any())
        {
            foreach (var ballState in prevMove.DestroyedAfterMove)
            {
                addBall(new Vector2(ballState.x, ballState.y), ballState.Color);
            }
        }
        tileGraph.InitNeighbours();
        // move the ball back
        var path = pathFinder.FindPath(tileGraph.tileNodes[prevMove.End.x, prevMove.End.y], tileGraph.tileNodes[prevMove.Start.x, prevMove.Start.y]);
        StartCoroutine(tileGraph.MoveBall(path, ballSpeed, false));
        scoreController.SubtractScore(prevMove.ScoreAdded);

        //tileGraph.InitNeighbours();
        pathFinder.Reset();
        Deselect();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver && Input.GetMouseButtonDown(0))
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

    private void OnBallMoved(Node start, Node end)
    {
        var move = new MoveState
        {
            Start = start,
            End = end
        };
        var foundLines = tileGraph.FindLines(end, lineLength);
        int scoreAdded = scoreController.AddScore(foundLines, lineLength);
        if (!foundLines.HaveLines(lineLength))
        {
            soundController.PlayMove();
        }
        else
        {
            move.AddDestroyedAfterMove(foundLines.GetAll(lineLength));
            tileGraph.DestroyBalls(foundLines, end);
            soundController.PlayDestroy();
        }
        
        move.ScoreAdded = scoreAdded;

        var currentMove = historyController.AddMove(move);

        if (!currentMove.Value.wasReversed)
        {
            GenerateBalls(ballsPerTurn, move);
        }
        else
        {
            foreach (var moveBall in currentMove.Value.Appeared)
            {
                addBall(new Vector2(moveBall.x, moveBall.y), moveBall.Color);
            }
        }

        if (!tileGraph.emptyNodes.Any())
        {
            gameOver = true;
            var highScore = FileController.GetHighScore();

            if (scoreController.Score > highScore)
            {
                FileController.AddRecord(new ScoreRecord("Player", scoreController.Score));
            }
        }
        tileGraph.InitNeighbours();
    }

    private bool addBall(Vector2 coords, Color? color = null)
    {
        var parent = GameObject.Find("Boubles");
        var spacing = tileViewPrefab.GetComponent<TileView>().spacing;
        if (!tileGraph.nodesWithBalls.Any(x => x.x == coords.x && x.y == coords.y))
        {
            var ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity, parent.transform);

            ball.transform.position = new Vector2((tileSize.x + spacing) * coords.x - offset, (tileSize.y + spacing) * coords.y - offset);
            ball.GetComponentInChildren<Ball>().Color = (color.HasValue)
                ? color.Value
                : possibleColors[Random.Range(0, possibleColors.Length)];
            tileGraph.AddBall((int)coords.x, (int)coords.y, ball);

            return true;
        }

        return false;
    }

    public void Shuffle()
    {
        Deselect();
        if (!bonusController.Shuffle())
        {
            return;
        }

        var newLocations = new List<Node>();
        var syncMoveDict = new Dictionary<int, bool>();

        for (int i = 0; i < tileGraph.nodesWithBalls.Count; i++)
        {
            syncMoveDict.Add(i, false);
        }

        var balls = tileGraph.nodesWithBalls.Select(x => x.ball).ToList();
        
        tileGraph.emptyNodes.AddRange(tileGraph.nodesWithBalls);
        tileGraph.nodesWithBalls.ForEach(x => x.ball = null);
        tileGraph.nodesWithBalls.Clear();

        for (int i = 0; i < balls.Count; i++)
        {
            var ball = balls[i];
            Node newNode = null;
            do
            {
                int newX = Random.Range(0, tileGraph.Width);
                int newY = Random.Range(0, tileGraph.Height);
                newNode = tileGraph.tileNodes[newX, newY];
            }
            while (newLocations.Any(x => x.x == newNode.x && x.y == newNode.y));
            
            newLocations.Add(newNode);
            StartCoroutine(tileGraph.ShuffleMove(ball, newNode, ballSpeed, syncMoveDict, i));
        }

        StartCoroutine(AfterShuffle(syncMoveDict));
    }

    IEnumerator AfterShuffle(Dictionary<int, bool> sync)
    {
        while (sync.Values.Any(x => x == false))
        {
            yield return false;
        }
        var shuffleLines = new List<Node>();
        foreach (var node in tileGraph.nodesWithBalls)
        {
            var lines = tileGraph.FindLines(node, lineLength);
            if (lines.HaveLines(lineLength))
            {
                shuffleLines.AddRange(lines.GetAll(lineLength));
            }
        }

        shuffleLines = shuffleLines.Distinct().ToList();
        if (shuffleLines.Any())
        {
            tileGraph.DestroyBalls(shuffleLines, shuffleLines.First());
        }

        var shuffleMove = new MoveState();
        tileGraph.nodesWithBalls.ForEach(x => shuffleMove.Appeared.Add(new BallState(x)));
        historyController.Reset(shuffleMove);

        tileGraph.InitNeighbours();
        pathFinder.Reset();
        bonusController.UpdateButtons();
    }

    private bool isSelected()
    {
        return startNode != null;
    }

    private bool hasBall(TileView tile)
    {
        return tile.node.ball != null;
    }

    private void Select(TileView tile)
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
        if (startNode != null && startNode.ball != null)
        {
            startNode.ball.GetComponentInChildren<Ball>().Deselect();
        }

        if (endNode != null && endNode.ball != null)
        {
            endNode.ball.GetComponentInChildren<Ball>().Deselect();
        }
        startNode = null;
    }
}
