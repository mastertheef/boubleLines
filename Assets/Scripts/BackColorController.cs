using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackColorController : MonoBehaviour {

    [SerializeField] private GameObject background;

    private GameController gameController;
    private Material backColor;
    private Color currentColor;

    public Color CurrentColor { get { return currentColor; } }
    
	// Use this for initialization
	void Start () {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        backColor = background.GetComponent<Renderer>().material;
        ChangeColor();
        backColor.color = currentColor;
	}
	
	// Update is called once per frame
	void Update () {
        if (backColor.color != currentColor)
        {
            backColor.color = Color.Lerp(backColor.color, currentColor, Time.deltaTime * 2);
        }
	}

    public void ChangeColor()
    {
        do
        {
            currentColor = gameController.possibleColors[Random.Range(0, gameController.possibleColors.Length)];
        }
        while (currentColor == backColor.color);
    }
}
