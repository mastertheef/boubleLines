using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : PopupBase {

    [SerializeField] private Toggle MusicToggle;
    [SerializeField] private Toggle SoundToggle;

    Settings currentSettings;
    SoundController soundController;
    PopupController popupController;

    private void Awake()
    {
        currentSettings = FileController.GetSettings();
        soundController = FindObjectOfType<SoundController>();
        popupController = FindObjectOfType<PopupController>();
        soundController.MuteMusic(!currentSettings.MusicOn);
        soundController.MuteSound(!currentSettings.SoundOn);
    }

    public override void Show()
    {
        var settings = FileController.GetSettings();
        base.Show();
    }
    
    public void SoundToggleValueChanged()
    {
        soundController.MuteSound(SoundToggle.isOn);
        currentSettings.SoundOn = !SoundToggle.isOn;
        FileController.SetSettings(currentSettings);
    }

    public void MusicToggleValueChanged()
    {
        soundController.MuteMusic(MusicToggle.isOn);
        currentSettings.MusicOn = !MusicToggle.isOn;
        FileController.SetSettings(currentSettings);
    }

    public void ShowHelp()
    {
        popupController.Show(Popups.Help);
    }
}
