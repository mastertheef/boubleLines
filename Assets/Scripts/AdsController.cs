using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsController : MonoBehaviour
{
    private const string storeId = "3050305";
    private const string rewardedVideo = "rewardedVideo";
    private const string banner = "banner";

    private GameOverController gameoverController;

    [SerializeField] bool testMode = true;
    [SerializeField] Text adIdText;

    // Start is called before the first frame update
    void Start()
    {
        gameoverController = GameObject.Find("GameOverController").GetComponent<GameOverController>();
        Monetization.Initialize(storeId, testMode);
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        StartCoroutine(ShowBanner());
    }

    public IEnumerator ShowBanner()
    {
        while (!Advertisement.IsReady(banner))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.Show(banner);
    }

    public void ShowDoubleRewardVideo()
    {
        StartCoroutine(DoubleRewardVideo());
    }

    private IEnumerator DoubleRewardVideo()
    {
        while (!Monetization.IsReady(rewardedVideo))
        {
            yield return null;
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(rewardedVideo) as ShowAdPlacementContent;

        if (ad != null)
        {
            ad.Show(DoubleRewardVideoFinished);
        }
    }

    private void DoubleRewardVideoFinished(UnityEngine.Monetization.ShowResult result)
    {
        if (result == UnityEngine.Monetization.ShowResult.Finished)
        {
            gameoverController.DoubleReward();
        }
    }
}
