using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeController : MonoBehaviour {

    [SerializeField] private Text highScoreText;
    [SerializeField] private Text coinsText;

	// Use this for initialization
	void Start () {
        highScoreText.text = string.Format("High Score: {0}", FileController.GetHighScore());
        coinsText.text = string.Format("x {0}", FileController.GetCoins());
    }
	
	public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
