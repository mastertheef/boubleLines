using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

    private int score;
    private int combo;

    public int diagonalModifier = 2;
    public Text ScoreText;
    public Text CoinsText;

    public int Score { get { return score; } }

	// Use this for initialization
	void Start () {
        score = 0;
        combo = 1;
        ScoreText.text = string.Format("Score: {0}", score);
    }
	
	public int AddScore(FoundLines lines, int minLength)
    {
        int addScore = 0;
        if (lines.HaveLines(minLength))
        {

            if (lines.horizontal.Count >= minLength)
            {
                addScore += combo * lines.horizontal.Count;
                combo++;
            }

            if (lines.vertical.Count >= minLength)
            {
                addScore += combo * lines.vertical.Count;
                combo++;
            }

            if (lines.leftDiag.Count >= minLength)
            {
                addScore += diagonalModifier * combo * lines.leftDiag.Count;
                combo++;
            }

            if (lines.rightDiag.Count >= minLength)
            {
                addScore += diagonalModifier * combo * lines.rightDiag.Count;
                combo++;
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
