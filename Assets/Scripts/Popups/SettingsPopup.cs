using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : PopupBase {

    [SerializeField] private Toggle MusicToggle;
    [SerializeField] private Toggle SoundToggle;

    Settings currentSettings;

    private void Awake()
    {
        currentSettings = FileController.GetSettings();
        UpdateToggle(SoundToggle, currentSettings.SoundOn);
        UpdateToggle(MusicToggle, currentSettings.MusicOn);
    }

    public override void Show()
    {
        var settings = FileController.GetSettings();
        base.Show();
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
    }
}
