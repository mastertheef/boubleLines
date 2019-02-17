using UnityEngine;
using UnityEngine.UI;

public class RecordElement : MonoBehaviour {

    [SerializeField] Text PlayerNameText;
    [SerializeField] Text ScoreText;

	public void SetValues(string name, int score)
    {
        PlayerNameText.text = name;
        ScoreText.text = score > 0
            ? score.ToString()
            : string.Empty;
    }
}
