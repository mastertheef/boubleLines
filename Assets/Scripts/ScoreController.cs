using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

    private int score;
    private int combo;

    public int diagonalModifier = 2;
    public int colorLineModifier = 3;
    public Text ScoreText;
    public Text CoinsText;
    private BackColorController colorController;

    public int Score { get { return score; } }

	// Use this for initialization
	void Start () {
        score = 0;
        combo = 1;
        ScoreText.text = string.Format("Score: {0}", score);
        colorController = GameObject.Find("BackColorController").GetComponent<BackColorController>();
    }
	
	public int AddScore(FoundLines lines, int minLength)
    {
        int addScore = 0;
        int colorBonus = 1;
        bool colorBonusApplied = false;
        if (lines.HaveLines(minLength))
        {

            if (lines.horizontal.Count >= minLength)
            {
                colorBonus = lines.horizontal[0].ball.GetComponentInChildren<Ball>().Color == colorController.CurrentColor
                    ? colorLineModifier
                    : 1;
                addScore += combo * colorBonus * lines.horizontal.Count;
                colorBonusApplied = colorBonus > 1;
                combo++;
            }

            if (lines.vertical.Count >= minLength)
            {
                colorBonus = lines.vertical[0].ball.GetComponentInChildren<Ball>().Color == colorController.CurrentColor
                    ? colorLineModifier
                    : 1;
                addScore += colorBonus * combo * lines.vertical.Count;
                colorBonusApplied = colorBonus > 1;
                combo++;
            }

            if (lines.leftDiag.Count >= minLength)
            {
                colorBonus = lines.leftDiag[0].ball.GetComponentInChildren<Ball>().Color == colorController.CurrentColor
                   ? colorLineModifier
                   : 1;
                addScore += colorBonus * diagonalModifier * combo * lines.leftDiag.Count;
                colorBonusApplied = colorBonus > 1;
                combo++;
            }

            if (lines.rightDiag.Count >= minLength)
            {
                colorBonus = lines.rightDiag[0].ball.GetComponentInChildren<Ball>().Color == colorController.CurrentColor
                  ? colorLineModifier
                  : 1;
                addScore += colorBonus * diagonalModifier * combo * lines.rightDiag.Count;
                colorBonusApplied = colorBonus > 1;
                combo++;
            }
            
            if (colorBonusApplied)
            {
                // show nice message
                colorController.ChangeColor();
            }
        }
        else
        {
            combo = 1;
            addScore = 1;
        }

        StartCoroutine(UpdateScoreVisual(addScore));

        return addScore;
    }

    public IEnumerator UpdateScoreVisual(int addScore)
    {
        int modifier = addScore < 0 ? -1 : 1;
        for (int i = 0; i < Mathf.Abs(addScore); i++)
        {
            score += modifier;
            ScoreText.text = string.Format("Score: {0}", score);
            yield return new WaitForSeconds(0.1f);
        }
    }


    public void SubtractScore(int subtract)
    {
        StartCoroutine(UpdateScoreVisual(subtract));
    }
}
