using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {

    [SerializeField] private Canvas GameOverGUI;
    [SerializeField] private Text TotalCoinsText;
    [SerializeField] private Text ScoreText;
    [SerializeField] private Text HighScoreText;
    [SerializeField] private Text RewardText;

    private ScoreController scoreController;
    private BackColorController backColorController;
    private GameController gameController;
    private int reward;

    // Use this for initialization
    void Start () {
        GameOverGUI.enabled = false;
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        backColorController = GameObject.Find("BackColorController").GetComponent<BackColorController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
	}
	
    public void GameOver()
    {
        StartCoroutine(ColorMatchBonus());
    }

    private IEnumerator ColorMatchBonus()
    {
        var currentColor = backColorController.CurrentColor;
        var colorMatchNodes = gameController.tileGraph.nodesWithBalls.Where(x => x.ball.GetComponentInChildren<Ball>().Color == currentColor).ToList();

        for(int i = colorMatchNodes.Count - 1; i>=0; i--)
        {
            gameController.tileGraph.DestroySIngleBall(colorMatchNodes[i]);
            scoreController.AddFinalBonusScore(colorMatchNodes[i], 7, true);
            yield return new WaitForSeconds(0.2f);
        }

        ShowGameOverGUI();
    }

    private void ShowGameOverGUI()
    {
        int score = scoreController.Score;
        int highScore = FileController.GetHighScore();
        int coins = FileController.GetCoins();
        reward = score / 10;
        TotalCoinsText.text = coins.ToString();
        ScoreText.text = score.ToString();
        HighScoreText.text = highScore.ToString();
        RewardText.text = reward.ToString();
        GameOverGUI.enabled = true;

        if (score > highScore)
        {
            FileController.AddRecord(new ScoreRecord("Player", score)); // need popup for that
        }

        FileController.SetCoins(coins + reward);
    }
}
