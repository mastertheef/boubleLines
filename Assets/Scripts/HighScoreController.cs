using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreController : MonoBehaviour {

    [SerializeField] Canvas RecordsCanvas;
    [SerializeField] RectTransform RecordsPanel;
    [SerializeField] RecordElement ElementPrefab;

	// Use this for initialization
	void Start () {
        RecordsCanvas.gameObject.SetActive(false);
	}
	
	public void ShowRecords()
    {
        RecordsCanvas.gameObject.SetActive(true);
        var records = FileController.GetAllRecords();

        var count = RecordsPanel.childCount;

        for (int i = count - 1; i >=0; i--)
        {
            Destroy(RecordsPanel.GetChild(i).gameObject);
        }

        for (int i = 0; i < FileController.MaxScoreRecords; i++)
        {
            var line = Instantiate(ElementPrefab, RecordsPanel);
            if (i < records.Count)
            {
                line.SetValues(records[i].PlayerName, records[i].HighScore);
            }
            else
            {
                line.SetValues(string.Empty, 0);
            }
        }
    }

    public void HideRecords()
    {
        RecordsCanvas.gameObject.SetActive(false);
    }
}
