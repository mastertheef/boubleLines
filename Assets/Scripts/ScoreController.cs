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


	// Use this for initialization
	void Start () {
        score = 0;
        combo = 1;
        ScoreText.text = string.Format("Score: {0}", score);
    }
	
	public void AddScore(FoundLines lines, int minLength)
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
                addScore += combo * lines.horizontal.Count;
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

        score += addScore;
        ScoreText.text = string.Format("Score: {0}", score);
    }
}
