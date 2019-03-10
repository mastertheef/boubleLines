using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        currentSettings = FileController.GetSettings();
        SoundToggle.isOn = !currentSettings.SoundOn;
        MusicToggle.isOn = !currentSettings.MusicOn;
        
    }

    // Use this for initialization
    void Start () {
        base.Start();
    }

    public void SoundToggleValueChanged()
    {
        currentSettings.SoundOn = !SoundToggle.isOn;
        FileController.SetSettings(currentSettings);
        soundController.MuteSound(SoundToggle.isOn);
    }

    public void MusicToggleValueChanged()
    {
        currentSettings.MusicOn = !MusicToggle.isOn;
        FileController.SetSettings(currentSettings);
        soundController.MuteMusic(MusicToggle.isOn);
    }

    public void Restart()
    {
        gameController.RestartGame();
        ClosePopup();
    }

    public void Home()
    {
        GameObject.Find("SceneFader").GetComponent<SceneFader>().FadeTo("HomeScene");
        //SceneManager.LoadScene("HomeScene");
    }
}
