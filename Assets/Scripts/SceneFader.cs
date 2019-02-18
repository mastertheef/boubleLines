using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    [SerializeField] Image FadeImage;
    [SerializeField] float Duration = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        FadeIn();
    }
    
    private void FadeIn()
    {
        FadeImage.GetComponent<CanvasRenderer>().SetAlpha(1f);
        FadeImage.CrossFadeAlpha(0f, Duration, false);
    }

    private IEnumerator fadeTo(string sceneName)
    {
        FadeImage.GetComponent<CanvasRenderer>().SetAlpha(0f);
        FadeImage.CrossFadeAlpha(1f, Duration, false);

        yield return new WaitForSeconds(Duration);

        SceneManager.LoadScene(sceneName);
    }

    public void FaedTo(string sceneName)
    {
        StartCoroutine(fadeTo(sceneName));
    }
}
