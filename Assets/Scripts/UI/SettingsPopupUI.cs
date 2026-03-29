using UnityEngine;
using UnityEngine.UI;

public class SettingsPopupUI : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnReset; // 可选

    [Header("Volume = Toggle + Slider")]
    [SerializeField] private Toggle toggleMute;     // 静音开关
    [SerializeField] private Slider sliderVolume;   // 0~1

    [Header("Brightness = Toggle + Slider")]
    [SerializeField] private Toggle toggleBrightness;  // 是否启用亮度遮罩（开=按slider生效，关=恢复默认亮度）
    [SerializeField] private Slider sliderBrightness;  // 0~1（0最暗，1最亮）
    [SerializeField] private Image brightnessOverlay;  // 全屏黑色遮罩 Image
    [Range(0f, 1f)]
    [SerializeField] private float maxDarkness = 0.7f; // 最暗时 overlay alpha

    // PlayerPrefs keys
    private const string PrefMute = "settings.mute";
    private const string PrefVol = "settings.volume";
    private const string PrefBrightnessOn = "settings.brightness.on";
    private const string PrefBrightness = "settings.brightness";

    // 防止 Toggle/Slider 互相触发导致“打架”
    private bool suppressEvents;

    private void Awake()
    {
        if (btnClose) btnClose.onClick.AddListener(() => mainMenuUI.ClosePopup());
        if (btnReset) btnReset.onClick.AddListener(ResetToDefault);

        // 配置 slider 范围
        if (sliderVolume)
        {
            sliderVolume.minValue = 0f;
            sliderVolume.maxValue = 1f;
            sliderVolume.wholeNumbers = false;
        }
        if (sliderBrightness)
        {
            sliderBrightness.minValue = 0f;
            sliderBrightness.maxValue = 1f;
            sliderBrightness.wholeNumbers = false;
        }

        // 绑定事件
        if (toggleMute) toggleMute.onValueChanged.AddListener(OnMuteToggleChanged);
        if (sliderVolume) sliderVolume.onValueChanged.AddListener(OnVolumeSliderChanged);

        if (toggleBrightness) toggleBrightness.onValueChanged.AddListener(OnBrightnessToggleChanged);
        if (sliderBrightness) sliderBrightness.onValueChanged.AddListener(OnBrightnessSliderChanged);

        LoadPrefs();
        ApplyAll();
    }

    private void OnDestroy()
    {
        if (toggleMute) toggleMute.onValueChanged.RemoveListener(OnMuteToggleChanged);
        if (sliderVolume) sliderVolume.onValueChanged.RemoveListener(OnVolumeSliderChanged);

        if (toggleBrightness) toggleBrightness.onValueChanged.RemoveListener(OnBrightnessToggleChanged);
        if (sliderBrightness) sliderBrightness.onValueChanged.RemoveListener(OnBrightnessSliderChanged);
    }

    // ---------------- Volume ----------------

    private void OnMuteToggleChanged(bool isMute)
    {
        if (suppressEvents) return;

        SaveMute(isMute);
        ApplyVolume();

        // 可选：如果静音，slider 仍保留之前值（不强制改成0）
        // 如果你想静音时 slider 自动变 0，取消下面注释：
        /*
        suppressEvents = true;
        if (sliderVolume) sliderVolume.value = isMute ? 0f : Mathf.Max(sliderVolume.value, 0.2f);
        suppressEvents = false;
        */
    }

    private void OnVolumeSliderChanged(float v)
    {
        if (suppressEvents) return;

        v = Mathf.Clamp01(v);
        SaveVolume(v);

        // 常见逻辑：只要你把音量拖到 0，就自动静音；拖大于0就取消静音
        if (toggleMute)
        {
            bool shouldMute = v <= 0.001f;
            if (toggleMute.isOn != shouldMute)
            {
                suppressEvents = true;
                toggleMute.isOn = shouldMute;
                suppressEvents = false;
                SaveMute(shouldMute);
            }
        }

        ApplyVolume();
    }

    private void ApplyVolume()
    {
        bool mute = toggleMute && toggleMute.isOn;
        float v = sliderVolume ? sliderVolume.value : 1f;
        AudioListener.volume = mute ? 0f : Mathf.Clamp01(v);
    }

    // ---------------- Brightness ----------------

    private void OnBrightnessToggleChanged(bool on)
    {
        if (suppressEvents) return;

        SaveBrightnessOn(on);
        ApplyBrightness();
    }

    private void OnBrightnessSliderChanged(float v)
    {
        if (suppressEvents) return;

        v = Mathf.Clamp01(v);
        SaveBrightness(v);

        // 如果你希望：拖动亮度时自动打开亮度开关
        if (toggleBrightness && !toggleBrightness.isOn)
        {
            suppressEvents = true;
            toggleBrightness.isOn = true;
            suppressEvents = false;
            SaveBrightnessOn(true);
        }

        ApplyBrightness();
    }

    private void ApplyBrightness()
    {
        if (!brightnessOverlay) return;

        bool on = toggleBrightness == null || toggleBrightness.isOn;
        float value = sliderBrightness ? sliderBrightness.value : 1f;

        // 关掉亮度调节 => 恢复默认（不变暗）
        if (!on)
        {
            SetOverlayAlpha(0f);
            return;
        }

        // value=1 => alpha=0（最亮）
        // value=0 => alpha=maxDarkness（最暗）
        float alpha = (1f - value) * maxDarkness;
        SetOverlayAlpha(alpha);
    }

    private void SetOverlayAlpha(float a)
    {
        Color c = brightnessOverlay.color;
        c.r = 0f; c.g = 0f; c.b = 0f;
        c.a = Mathf.Clamp01(a);
        brightnessOverlay.color = c;
    }

    // ---------------- Prefs ----------------

    private void LoadPrefs()
    {
        suppressEvents = true;

        if (toggleMute) toggleMute.isOn = PlayerPrefs.GetInt(PrefMute, 0) == 1;
        if (sliderVolume) sliderVolume.value = PlayerPrefs.GetFloat(PrefVol, 1f);

        if (toggleBrightness) toggleBrightness.isOn = PlayerPrefs.GetInt(PrefBrightnessOn, 1) == 1;
        if (sliderBrightness) sliderBrightness.value = PlayerPrefs.GetFloat(PrefBrightness, 1f);

        suppressEvents = false;
    }

    private void SaveMute(bool v) => PlayerPrefs.SetInt(PrefMute, v ? 1 : 0);
    private void SaveVolume(float v) => PlayerPrefs.SetFloat(PrefVol, v);
    private void SaveBrightnessOn(bool v) => PlayerPrefs.SetInt(PrefBrightnessOn, v ? 1 : 0);
    private void SaveBrightness(float v) => PlayerPrefs.SetFloat(PrefBrightness, v);

    private void ApplyAll()
    {
        ApplyVolume();
        ApplyBrightness();
        PlayerPrefs.Save();
    }

    private void ResetToDefault()
    {
        suppressEvents = true;

        if (toggleMute) toggleMute.isOn = false;
        if (sliderVolume) sliderVolume.value = 1f;

        if (toggleBrightness) toggleBrightness.isOn = true;
        if (sliderBrightness) sliderBrightness.value = 1f;

        suppressEvents = false;

        SaveMute(false);
        SaveVolume(1f);
        SaveBrightnessOn(true);
        SaveBrightness(1f);

        ApplyAll();
    }
}