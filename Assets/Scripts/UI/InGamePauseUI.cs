using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// In-game pause menu (shared across levels via Prefab).
/// - Top-left pause button opens pause panel
/// - ESC toggles pause
/// - Settings: Mute/Volume + Brightness toggle/slider (uses PlayerPrefs like SettingsPopupUI)
/// - Exit button returns to MainMenu
/// </summary>
public class InGamePauseUI : MonoBehaviour
{
    [Header("Top-left HUD Button")]
    [SerializeField] private Button btnPause; // left-top pause button

    [Header("Root")]
    [SerializeField] private GameObject pauseUIRoot; // Dimmer + Panel container

    [Header("Buttons")]
    [SerializeField] private Button btnResume;
    [SerializeField] private Button btnClose; // NEW: close button (same as Resume)
    [SerializeField] private Button btnExitToMenu;

    [Header("Volume = Toggle + Slider")]
    [SerializeField] private Toggle toggleMute;
    [SerializeField] private Slider sliderVolume;

    [Header("Brightness = Toggle + Slider")]
    [SerializeField] private Toggle toggleBrightness;
    [SerializeField] private Slider sliderBrightness;
    [SerializeField] private Image brightnessOverlay;
    [Range(0f, 1f)]
    [SerializeField] private float maxDarkness = 0.7f;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;
    private bool suppressEvents;

    // PlayerPrefs keys (keep same keys as SettingsPopupUI so both menus share same settings)
    private const string PrefMute = "settings.mute";
    private const string PrefVol = "settings.volume";
    private const string PrefBrightnessOn = "settings.brightness.on";
    private const string PrefBrightness = "settings.brightness";

    private void Awake()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseUIRoot) pauseUIRoot.SetActive(false);

        // Wire buttons
        if (btnPause) btnPause.onClick.AddListener(Pause);
        if (btnResume) btnResume.onClick.AddListener(Resume);
        if (btnClose) btnClose.onClick.AddListener(Resume); // NEW
        if (btnExitToMenu) btnExitToMenu.onClick.AddListener(ExitToMenu);

        // Slider ranges
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

        // Bind events
        if (toggleMute) toggleMute.onValueChanged.AddListener(OnMuteToggleChanged);
        if (sliderVolume) sliderVolume.onValueChanged.AddListener(OnVolumeSliderChanged);

        if (toggleBrightness) toggleBrightness.onValueChanged.AddListener(OnBrightnessToggleChanged);
        if (sliderBrightness) sliderBrightness.onValueChanged.AddListener(OnBrightnessSliderChanged);

        LoadPrefs();
        ApplyAll(); // apply volume + brightness at level start
    }

    private void OnDestroy()
    {
        if (btnPause) btnPause.onClick.RemoveListener(Pause);
        if (btnResume) btnResume.onClick.RemoveListener(Resume);
        if (btnClose) btnClose.onClick.RemoveListener(Resume); // NEW
        if (btnExitToMenu) btnExitToMenu.onClick.RemoveListener(ExitToMenu);

        if (toggleMute) toggleMute.onValueChanged.RemoveListener(OnMuteToggleChanged);
        if (sliderVolume) sliderVolume.onValueChanged.RemoveListener(OnVolumeSliderChanged);

        if (toggleBrightness) toggleBrightness.onValueChanged.RemoveListener(OnBrightnessToggleChanged);
        if (sliderBrightness) sliderBrightness.onValueChanged.RemoveListener(OnBrightnessSliderChanged);
    }

    private void Update()
    {
        // ESC toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    // ---------------- Pause Flow ----------------

    public void Pause()
    {
        isPaused = true;
        if (pauseUIRoot) pauseUIRoot.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        if (pauseUIRoot) pauseUIRoot.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ExitToMenu()
    {
        // must restore timeScale before switching scenes
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ---------------- Volume ----------------

    private void OnMuteToggleChanged(bool mute)
    {
        if (suppressEvents) return;

        SaveMute(mute);
        ApplyVolume();
        PlayerPrefs.Save();
    }

    private void OnVolumeSliderChanged(float v)
    {
        if (suppressEvents) return;

        v = Mathf.Clamp01(v);
        SaveVolume(v);

        // drag to 0 => auto mute; drag >0 => unmute
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
        PlayerPrefs.Save();
    }

    private void ApplyVolume()
    {
        // Prefer AudioManager so BGM + ambience update immediately
        if (AudioManager.I != null)
        {
            AudioManager.I.ApplyPrefs();
            return;
        }

        // Fallback if AudioManager doesn't exist (e.g., playing a level scene directly)
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
        PlayerPrefs.Save();
    }

    private void OnBrightnessSliderChanged(float v)
    {
        if (suppressEvents) return;

        v = Mathf.Clamp01(v);
        SaveBrightness(v);

        // auto turn on when user drags the slider
        if (toggleBrightness && !toggleBrightness.isOn)
        {
            suppressEvents = true;
            toggleBrightness.isOn = true;
            suppressEvents = false;
            SaveBrightnessOn(true);
        }

        ApplyBrightness();
        PlayerPrefs.Save();
    }

    private void ApplyBrightness()
    {
        if (!brightnessOverlay) return;

        bool on = toggleBrightness == null || toggleBrightness.isOn;
        float value = sliderBrightness ? sliderBrightness.value : 1f;

        if (!on)
        {
            SetOverlayAlpha(0f);
            return;
        }

        float alpha = (1f - value) * Mathf.Clamp01(maxDarkness);
        SetOverlayAlpha(alpha);
    }

    private void SetOverlayAlpha(float a)
    {
        var c = brightnessOverlay.color;
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
    }
}