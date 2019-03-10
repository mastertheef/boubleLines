using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeController : MonoBehaviour {

    [SerializeField] private Text coinsText;

	// Use this for initialization
	void Start () {
        coinsText.text = FileController.GetCoins().ToString();
    }
	
	public void StartGame()
    {
        //SceneManager.LoadScene("GameScene");
        GameObject.Find("SceneFader").GetComponent<SceneFader>().FadeTo("GameScene");
    }

    public void RateUs()
    {
#if unity_android
        Application.OpenURL("market://details?id=com.QualityGeek.BubbleLines");
#endif
    }
}
