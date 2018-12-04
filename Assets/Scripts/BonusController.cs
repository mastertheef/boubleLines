using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusController : MonoBehaviour {

    private TileGraph tileGraph;
    private HistoryController historyController;
    public int coins = 0;
    [SerializeField] private int explodePrice = 50;
    [SerializeField] private int sufflePrice = 100;
    [SerializeField] private int stepBackPrice = 50;
    

    public bool isExploding = false;
    public Text coinsText;
    public Button explodeButton;
    

    public int Coins {
        get
        {
            return coins;
        }
        set
        {
            coins = value;
            coinsText.text = string.Format("x {0}", coins);
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
            explodeButton.GetComponentInChildren<Text>().text = string.Format("Explode: {0}", explodePrice);
        }
    }



	// Use this for initialization
	void Start () {
        tileGraph = GameObject.Find("TileGraph").GetComponent<TileGraph>();
        historyController = GameObject.Find("HistoryController").GetComponent<HistoryController>();
        //Coins = 0;
        ExplodePrice = explodePrice;
    }
	
	// Update is called once per frame
	void Update () {
        if (isExploding && Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.transform.tag == "Tile")
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

                    Coins -= explodePrice;
                    ExplodePrice *= 2;
                }
            }

            isExploding = false;
        }
	}

    public void Explode()
    {
        if (coins >= explodePrice)
        {
            isExploding = !isExploding;
        }
    }

    public MoveState StepBack()
    {
        if (stepBackPrice <= coins && historyController.hasPrevious())
        {
            Coins -= stepBackPrice;
            stepBackPrice *= 2;
            return historyController.ReverseMove();
            
        }
        return null;
    } 
}
