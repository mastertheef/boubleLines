using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewRecordPopup : PopupBase
{
    [SerializeField] TMP_InputField playerName;
    [SerializeField] Text highScore;
    int score;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        playerName.text = "Player";
    }

    public override void Show()
    {
        base.Show();
        score = GameObject.Find("ScoreController").GetComponent<ScoreController>().Score;
        highScore.text = score.ToString();
    }

    public void Save()
    {
        FileController.AddRecord(playerName.text, score);
        base.ClosePopup();
    }
}
