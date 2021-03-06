﻿using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Canvas GUI;
    private int score;
    private int combo;

    public int diagonalModifier = 2;
    public int colorLineModifier = 3;
    public Text ScoreText;
    public Text HighScoreText;
    private BackColorController colorController;
    

    public int Score { get { return score; } }

	// Use this for initialization
	void Start () {
        score = 0;
        combo = 0;
        ScoreText.text = string.Format("{0}", score);
        HighScoreText.text = FileController.GetHighScore().ToString();
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
                combo++;
                colorBonus = lines.horizontal[0].ball.GetComponentInChildren<Ball>().Color == colorController.CurrentColor
                    ? colorLineModifier
                    : 1;
                addScore += comboModifier(combo) * colorBonus * lines.horizontal.Count;
                colorBonusApplied = colorBonus > 1;
                ShowFloatingScore(lines.horizontal, addScore / lines.horizontal.Count);
            }

            if (lines.vertical.Count >= minLength)
            {
                combo++;
                colorBonus = lines.vertical[0].ball.GetComponentInChildren<Ball>().Color == colorController.CurrentColor
                    ? colorLineModifier
                    : 1;
                addScore += colorBonus * comboModifier(combo) * lines.vertical.Count;
                colorBonusApplied = colorBonus > 1;
                ShowFloatingScore(lines.vertical, addScore / lines.vertical.Count);
            }

            if (lines.leftDiag.Count >= minLength)
            {
                combo++;
                colorBonus = lines.leftDiag[0].ball.GetComponentInChildren<Ball>().Color == colorController.CurrentColor
                   ? colorLineModifier
                   : 1;
                addScore += colorBonus * diagonalModifier * comboModifier(combo) * lines.leftDiag.Count;
                colorBonusApplied = colorBonus > 1;
                ShowFloatingScore(lines.leftDiag, addScore / lines.leftDiag.Count);
            }

            if (lines.rightDiag.Count >= minLength)
            {
                combo++;
                colorBonus = lines.rightDiag[0].ball.GetComponentInChildren<Ball>().Color == colorController.CurrentColor
                  ? colorLineModifier
                  : 1;
                addScore += colorBonus * diagonalModifier * comboModifier(combo) * lines.rightDiag.Count;
                colorBonusApplied = colorBonus > 1;
                ShowFloatingScore(lines.rightDiag, addScore / lines.rightDiag.Count);
            }

            ///Debug.Log(combo +" " + comboModifier(combo));
            
            if (colorBonusApplied)
            {
                // show nice message
                colorController.ChangeColor();
            }
        }
        else
        {
            combo = 0;
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
            ScoreText.text = string.Format("{0}", score);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SubtractScore(int subtract)
    {
        StartCoroutine(UpdateScoreVisual(subtract));
    }

    public void ResetScore()
    {
        score = 0;
        HighScoreText.text = FileController.GetHighScore().ToString();
        ScoreText.text = string.Format("{0}", score);
    }

    private void ShowFloatingScore(List<Node> line, int number)
    {
        foreach (var node in line)
        {
            AddFinalBonusScore(node, number);
        }
    }

    public void AddFinalBonusScore(Node node, int number, bool addScore = false)
    {
        var position = Camera.main.WorldToScreenPoint(node.tile.transform.position);
        var foatingText = Instantiate(floatingTextPrefab);
        foatingText.transform.SetParent(GUI.transform);
        foatingText.transform.position = position;
        foatingText.GetComponentInChildren<Text>().text = string.Format("+{0}", number);
        Destroy(foatingText, 1f);

        if (addScore)
        {
            score += number;
            ScoreText.text = score.ToString();
        }
    }

    private int comboModifier(int combo)
    {
        return (combo) * 2 + 2;
    }
}
