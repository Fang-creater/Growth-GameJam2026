using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsAudioUI : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolumeParam = "MasterVolume";
    [SerializeField] private Slider masterVolumeSlider; // 0~1

    private const string PrefMasterVol = "pref_master_volume";

    private void Start()
    {
        float v01 = PlayerPrefs.GetFloat(PrefMasterVol, 1f);
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = v01;

        ApplyVolume(v01);
    }

    public void OnMasterVolumeChanged(float v01)
    {
        PlayerPrefs.SetFloat(PrefMasterVol, v01);
        PlayerPrefs.Save();
        ApplyVolume(v01);
    }

    private void ApplyVolume(float v01)
    {
        if (audioMixer == null) return;
        float db = (v01 <= 0.0001f) ? -80f : Mathf.Log10(v01) * 20f;
        audioMixer.SetFloat(masterVolumeParam, db);
    }
}