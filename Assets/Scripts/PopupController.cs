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
    Pause
}

public class PopupController : MonoBehaviour {

    [SerializeField] private Canvas GUI;
    [SerializeField] private SettingsPopup settingsPopupPrefab;
    [SerializeField] private PausePopup pausePopupPrefab;

    [SerializeField] private Image popupDarken;
    private Stack<PopupBase> popups;
    private Dictionary<Popups, PopupBase> popupPool;

    private void Awake()
    {
        popups = new Stack<PopupBase>();
        PopupBase.OnPopupClose += OnPopupClose;
        popupPool = new Dictionary<Popups, PopupBase>();
    }

    private void Start()
    {
        popupPool.Add(Popups.Settings, Instantiate(settingsPopupPrefab, GUI.transform));
        popupPool.Add(Popups.Pause, Instantiate(pausePopupPrefab, GUI.transform));
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

    private IEnumerator HideDarken()
    {
        yield return new WaitForSeconds(1);
        popupDarken.enabled = false;
    }
}
