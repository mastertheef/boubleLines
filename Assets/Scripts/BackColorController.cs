using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackColorController : MonoBehaviour {

    [SerializeField] private GameObject background;
    [SerializeField] private GameObject backgroundNext;
    [SerializeField] private Material[] gradients;

    private GameController gameController;
    private Color currentColor, nextColor;
    private Dictionary<Color, Material> gradientDict;

    public Color CurrentColor { get { return currentColor; } }
    
	// Use this for initialization
	void Start () {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        currentColor = gameController.possibleColors[Random.Range(0, gameController.possibleColors.Length)];
        SetNextColor();

        gradientDict = new Dictionary<Color, Material>();
        for (int i = 0; i < gameController.possibleColors.Length; i++)
        {
            gradientDict.Add(gameController.possibleColors[i], gradients[i]);
        }
        backgroundNext.GetComponent<Renderer>().material = gradientDict[nextColor];
        background.GetComponent<Renderer>().material = gradientDict[currentColor];
    }
	
    public void ChangeColor()
    {
        StartCoroutine(ChangeGradient());
    }

    private void SetNextColor()
    {
        do
        {
            nextColor = gameController.possibleColors[Random.Range(0, gameController.possibleColors.Length)];
        }
        while (nextColor == currentColor);
    }

    private IEnumerator ChangeGradient()
    {
        var backMat = background.GetComponent<Renderer>().material;
        while (backMat.color.a > 0)
        {
            backMat.color = new Color(backMat.color.r, backMat.color.g, backMat.color.b, backMat.color.a - Time.deltaTime);
            yield return null;
        }
        currentColor = nextColor;
        SetNextColor();
        background.GetComponent<Renderer>().material = backgroundNext.GetComponent<Renderer>().material;
        backgroundNext.GetComponent<Renderer>().material = gradientDict[nextColor];
    }
}
