using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {

    [SerializeField] private Canvas GameOverGUI;
    [SerializeField] private Text TotalCoinsText;
    [SerializeField] private Text ScoreText;
    [SerializeField] private Text HighScoreText;
    [SerializeField] private Text RewardText;

    private ScoreController scoreController;
    private int reward;

    // Use this for initialization
    void Start () {
        GameOverGUI.enabled = false;
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
	}
	
    public void GameOver()
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
