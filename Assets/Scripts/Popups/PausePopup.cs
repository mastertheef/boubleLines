using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePopup : PopupBase {

    [SerializeField] private Button HomeButton;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Toggle SoundToggle;
    [SerializeField] private Toggle MusicToggle;

    Settings currentSettings;
    SoundController soundController;
    GameController gameController;

    private void Awake()
    {
        currentSettings = FileController.GetSettings();
        UpdateToggle(SoundToggle, currentSettings.SoundOn);
        UpdateToggle(MusicToggle, currentSettings.MusicOn);
    }

    // Use this for initialization
    void Start () {
        base.Start();
       
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
	
    private void UpdateToggle(Toggle toggle, bool value)
    {
        var offImage = toggle.transform.Find("Background").Find("OffImage").GetComponent<Image>();
        if (offImage != null)
        {
            offImage.enabled = !value;
        }
    }

    public void SoundToggleValueChanged()
    {
        UpdateToggle(SoundToggle, SoundToggle.isOn);
        currentSettings.SoundOn = SoundToggle.isOn;
        FileController.SetSettings(currentSettings);
    }

    public void MusicToggleValueChanged()
    {
        UpdateToggle(MusicToggle, MusicToggle.isOn);
        currentSettings.MusicOn = MusicToggle.isOn;
        FileController.SetSettings(currentSettings);
        soundController.SetBubbleVolume(MusicToggle.isOn ? 1 : 0);
    }

    public void Restart()
    {
        gameController.RestartGame();
        ClosePopup();
    }
}
