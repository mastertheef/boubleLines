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

        foreach (var record in records)
        {
            var line = Instantiate(ElementPrefab, RecordsPanel);
            line.SetValues(record.PlayerName, record.HighScore);
        }
    }

    public void HideRecords()
    {
        RecordsCanvas.gameObject.SetActive(false);
    }
}
