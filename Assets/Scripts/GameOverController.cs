using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {

    [SerializeField] private Canvas GameOverGUI;
    [SerializeField] private Canvas PopupGUI;
    [SerializeField] private Image BackGround;
    [SerializeField] private Text TotalCoinsText;
    [SerializeField] private Text ScoreText;
    [SerializeField] private Text HighScoreText;
    [SerializeField] private Text RewardText;
    [SerializeField] private Button DoubleRewardButton;
    [SerializeField] private GameObject ColorMatchBonusPrefab;

    private ScoreController scoreController;
    private BackColorController backColorController;
    private GameController gameController;
    private SoundController soundController;
    private PopupController popupController;
    private int reward;

    // Use this for initialization
    void Start () {
        GameOverGUI.enabled = false;
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        backColorController = GameObject.Find("BackColorController").GetComponent<BackColorController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        popupController = GameObject.Find("PopupController").GetComponent<PopupController>();
        DoubleRewardButton.interactable = FindObjectOfType<AdsController>() != null;
    }
	
    public void GameOver()
    {
        popupController.Show(Popups.GameOver);
    }

    public void StartGameOverRoutine()
    {
        StartCoroutine(ColorMatchBonus());
    }

    public void DoubleReward()
    {
        int score = scoreController.Score;
        reward *= 2;
        RewardText.text = string.Format("Reward: {0}", reward); // maybe some animation ...
        int coins = FileController.GetCoins();
        FileController.SetCoins(coins + reward);
        DoubleRewardButton.enabled = false;
    }

    private IEnumerator ColorMatchBonus()
    {
        yield return new WaitForSeconds(1.5f);
        popupController.DarkenScreen();
        var colorMatchText = Instantiate(ColorMatchBonusPrefab, PopupGUI.transform);
        yield return new WaitForSeconds(2f);
        popupController.UnDarkenScreen();
        Destroy(colorMatchText);
        var currentColor = backColorController.CurrentColor;
        var colorMatchNodes = gameController.tileGraph.nodesWithBalls.Where(x => x.ball.GetComponentInChildren<Ball>().Color == currentColor).ToList();

        for (int i = colorMatchNodes.Count - 1; i >= 0; i--)
        {
            gameController.tileGraph.DestroySIngleBall(colorMatchNodes[i]);
            soundController.PlayDestroy();
            scoreController.AddFinalBonusScore(colorMatchNodes[i], 7, true);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);

        ShowGameOverGUI();
    }

    private void ShowGameOverGUI()
    {
        BackGround.GetComponent<CanvasRenderer>().SetAlpha(0f);
        BackGround.CrossFadeAlpha(1f, 0.2f, false);
        int score = scoreController.Score;
        int highScore = FileController.GetHighScore();
        int coins = FileController.GetCoins();
        reward = score / 10 + 1;
        TotalCoinsText.text = coins.ToString();
        ScoreText.text = score.ToString();
        HighScoreText.text = string.Format("Record: {0}", highScore);
        RewardText.text = string.Format("Reward: {0}", reward);
        GameOverGUI.enabled = true;

        if (score > highScore)
        {
            popupController.Show(Popups.NewRecord);
        }

        FileController.SetCoins(coins + reward);
    }
}
