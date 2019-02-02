using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BonusController : MonoBehaviour {

    private TileGraph tileGraph;
    private HistoryController historyController;
    private SoundController soundController;
    private PathFinder pathFinder;
    public int coins = 0;
    [SerializeField] private int explodePrice = 50;
    [SerializeField] private int shufflePrice = 100;
    [SerializeField] private int stepBackPrice = 50;
    

    public bool isExploding = false;
    public Text coinsText;
    public Button explodeButton;
    public Button stepbackButton;
    public Button shuffleButton;
    public GameObject pinPrefab;
    private List<Pin> shownPins;
    bool pinsShown = false;


    public int Coins {
        get
        {
            return coins;
        }
        set
        {
            coins = value;
            coinsText.text = coins.ToString();
        }
    }

    public int ExplodePrice
    {
        get
        {
            return explodePrice;
        }
        set
        {
            explodePrice = value;
            explodeButton.GetComponentInChildren<Text>().text = explodePrice.ToString();
        }
    }

    public int ShufflePrice
    {
        get
        {
            return explodePrice;
        }
        set
        {
            shufflePrice = value;
            shuffleButton.GetComponentInChildren<Text>().text = shufflePrice.ToString();
        }
    }

    public int StepBackPrice
    {
        get
        {
            return stepBackPrice;
        }
        set
        {
            stepBackPrice = value;
            stepbackButton.GetComponentInChildren<Text>().text = stepBackPrice.ToString();
        }
    }


    // Use this for initialization
    void Start () {
        tileGraph = GameObject.Find("TileGraph").GetComponent<TileGraph>();
        historyController = GameObject.Find("HistoryController").GetComponent<HistoryController>();
        pathFinder = GameObject.Find("PathFinder").GetComponent<PathFinder>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();

        Coins = FileController.GetCoins();
        ExplodePrice = explodePrice;
        ShufflePrice = shufflePrice;
        StepBackPrice = stepBackPrice;

        shownPins = new List<Pin>();
    }
	
	// Update is called once per frame
	void Update () {
        // show pins on balls
        if (isExploding && !pinsShown)
        {
            foreach (var node in tileGraph.nodesWithBalls)
            {
                var pin = Instantiate(pinPrefab, node.tile.transform.position, Quaternion.identity);
                shownPins.Add(pin.GetComponent<Pin>());
            }
            pinsShown = true;
        }
       

        if (isExploding && Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.transform != null && hit.transform.CompareTag("Tile"))
            {
                var tile = hit.transform.gameObject.GetComponent<TileView>();
                
                var ballsToDestroy = new List<Node>();
                if (tile.node.ball != null)
                {
                    ballsToDestroy.Add(tile.node);
                    foreach (var dir in TileGraph.directions)
                    {
                        int newX = tile.node.x + (int)dir.x;
                        int newY = tile.node.y + (int)dir.y;
                        if (tileGraph.IsInBounds(newX, newY))
                        {
                            var neighbour = tileGraph.tileNodes[newX, newY];
                            if (neighbour.ball != null)
                            {
                                ballsToDestroy.Add(neighbour);
                            }
                        }
                    }

                    tileGraph.DestroyBalls(ballsToDestroy, tile.node);
                    soundController.PlayDestroy();
                    Coins -= ExplodePrice;
                    ExplodePrice *= 2;
                    UpdateButtons();

                    var shuffleMove = new MoveState();
                    tileGraph.nodesWithBalls.ForEach(x => shuffleMove.Appeared.Add(new BallState(x)));
                    historyController.Reset(shuffleMove);
                }
                isExploding = false;
                clearPins(true);
            }
        }
	}

    public void Explode()
    {
        if (coins >= ExplodePrice)
        {
            if (isExploding && pinsShown)
            {
                clearPins(false);
            }
            isExploding = !isExploding;
        }
    }

    public MoveState StepBack()
    {
        if (StepBackPrice <= coins && historyController.hasPrevious())
        {
            Coins -= StepBackPrice;
            StepBackPrice *= 2;
            UpdateButtons();
            return historyController.ReverseMove();
        }
        return null;
    } 

    public bool Shuffle()
    {
        if (coins >= ShufflePrice)
        {
            Coins -= ShufflePrice;
            UpdateButtons();
            shuffleButton.enabled = false;
            return true;
        }

        return false;
    }

    public void UpdateButtons()
    {
        explodeButton.enabled = coins >= ExplodePrice;
        shuffleButton.enabled = coins >= ShufflePrice;
        stepbackButton.enabled = coins >= StepBackPrice;
    }

    private void clearPins(bool force)
    {
        if (!force)
        {
            shownPins.ForEach(x => x.Hide());
        }
        else
        {
            shownPins.ForEach(x => Destroy(x.gameObject));
        }
        shownPins.Clear();
        pinsShown = false;
    }
}
