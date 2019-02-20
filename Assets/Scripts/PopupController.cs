using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum Popups
{
    Settings,
    Pause,
    GameOver,
    NewRecord
}

public class PopupController : MonoBehaviour {

    [SerializeField] private Canvas GUI;
    [SerializeField] private SettingsPopup settingsPopupPrefab;
    [SerializeField] private PausePopup pausePopupPrefab;
    [SerializeField] private GameOverPopup gameOverPopup;
    [SerializeField] private NewRecordPopup newRecordPopup;

    [SerializeField] private Image popupDarken;
    private Stack<PopupBase> popups;
    private Dictionary<Popups, PopupBase> popupPool;

    private void Awake()
    {
        popups = new Stack<PopupBase>();
        popupPool = new Dictionary<Popups, PopupBase>();
    }

    private void Start()
    {
        PopupBase.OnPopupClose += OnPopupClose;
        if (settingsPopupPrefab != null)
            popupPool.Add(Popups.Settings, Instantiate(settingsPopupPrefab, GUI.transform));

        if (pausePopupPrefab != null) 
            popupPool.Add(Popups.Pause, Instantiate(pausePopupPrefab, GUI.transform));

        if (gameOverPopup != null)
            popupPool.Add(Popups.GameOver, Instantiate(gameOverPopup, GUI.transform));

        if (gameOverPopup != null)
            popupPool.Add(Popups.NewRecord, Instantiate(newRecordPopup, GUI.transform));
    }

    public void Show(PopupBase popup)
    {
        if (popups.Any())
        {
            var current = popups.Peek();
            current.Hide();
        }
        popup.Show();
        popups.Push(popup);
        popupDarken.enabled = true;
        popupDarken.GetComponent<CanvasRenderer>().SetAlpha(0f);
        popupDarken.CrossFadeAlpha(1f, 0.1f, false);
    }

    public void Close()
    {
        if (popups.Any())
        {
            var current = popups.Pop();
            current.Hide();

            var prev = popups.Any()? popups.Peek(): null;
            if (prev != null)
            {
                prev.Show();
            }
            else
            {
                StartCoroutine(HideDarken());
            }
        }
        else
        {
            StartCoroutine(HideDarken());
        }
    }

    public void OnPopupClose(PopupBase popup)
    {
        Close();
    }

    public void ShowSettings()
    {
        PopupBase popup;
        popup = popupPool[Popups.Settings];
        Show(popup);
    }

    public void ShowPause()
    {
        PopupBase popup;
        popup = popupPool[Popups.Pause];
        Show(popup);
    }

    public void ShowGameOver()
    {
        PopupBase popup;
        popup = popupPool[Popups.GameOver];
        Show(popup);
    }

    public void Show(Popups popupType)
    {
        PopupBase popup;
        popup = popupPool[popupType];
        Show(popup);
    }

    private IEnumerator HideDarken()
    {
        popupDarken.GetComponent<CanvasRenderer>().SetAlpha(1f);
        popupDarken.CrossFadeAlpha(0f, 0.1f, false);
        yield return new WaitForSeconds(1);
        popupDarken.enabled = false;
       
    }

    private void OnDestroy()
    {
        PopupBase.OnPopupClose -= OnPopupClose;
    }
}
