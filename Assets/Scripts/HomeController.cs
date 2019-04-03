using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeController : MonoBehaviour {

    [SerializeField] private Text coinsText;

	// Use this for initialization
	void Start () {
        UpdateCoins();
    }
	
	public void StartGame()
    {
        //SceneManager.LoadScene("GameScene");
        GameObject.Find("SceneFader").GetComponent<SceneFader>().FadeTo("GameScene");
    }

    public void ContactDeveloper()
    {
        Application.OpenURL("https://docs.google.com/forms/d/1Wqfq4vemidne2QwA8QyrtYdOF1mCJnqLw1PF-G_nKe8");
    }

    public void UpdateCoins()
    {
        coinsText.text = FileController.GetCoins().ToString();
    }

    public void RateUs()
    {
#if unity_android
        Application.OpenURL("market://details?id=com.QualityGeek.BubbleLines");
#endif
    }
}
