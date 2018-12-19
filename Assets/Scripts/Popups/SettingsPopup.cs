using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : PopupBase {

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;

	public void SaveSettings()
    {
        var settings = new Settings
        {
            BubbleVolume = soundSlider.value,
            MusicVolume = musicSlider.value
        };
        FileController.SetSettings(settings);
        ClosePopup();
    }

    public override void Show()
    {
        var settings = FileController.GetSettings();
        musicSlider.value = settings.MusicVolume;
        soundSlider.value = settings.BubbleVolume;
        base.Show();
    }
}
