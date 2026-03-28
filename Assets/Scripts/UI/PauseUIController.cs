using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUIController : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject pauseUIRoot; // PauseUI（包含 Dimmer + Panel）

    [Header("Overlay (Brightness)")]
    [SerializeField] private Image brightnessOverlay; // 全屏Image：BrightnessOverlay（黑色，RaycastTarget关掉）

    [Header("Buttons")]
    [SerializeField] private Button btnResume;
    [SerializeField] private Button btnBackToMenu;
    [SerializeField] private Button btnHelp;
    [SerializeField] private Button btnSave;

    [Header("Sliders")]
    [SerializeField] private Slider sliderVolume;       // 0~1
    [SerializeField] private Slider sliderBrightness;   // 0~1（0最暗，1最亮）

    [Header("Brightness Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float maxDarkness = 0.7f; // 最大变暗强度：0.7 表示最暗时遮罩alpha=0.7

    private bool isPaused;

    private void Awake()
    {
        // 初始化暂停状态
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseUIRoot) pauseUIRoot.SetActive(false);

        // 绑定按钮
        if (btnResume) btnResume.onClick.AddListener(Resume);
        if (btnBackToMenu) btnBackToMenu.onClick.AddListener(BackToMenu);

        if (btnHelp) btnHelp.onClick.AddListener(OnHelp);
        if (btnSave) btnSave.onClick.AddListener(OnSave);

        // 绑定滑条
        if (sliderVolume) sliderVolume.onValueChanged.AddListener(OnVolumeChanged);
        if (sliderBrightness) sliderBrightness.onValueChanged.AddListener(OnBrightnessChanged);

        // 设置滑条默认值（可按需改）
        if (sliderVolume)
        {
            sliderVolume.minValue = 0f;
            sliderVolume.maxValue = 1f;
            sliderVolume.value = AudioListener.volume; // 使用当前全局音量作为初始
        }

        if (sliderBrightness)
        {
            sliderBrightness.minValue = 0f;
            sliderBrightness.maxValue = 1f;
            sliderBrightness.value = 1f; // 默认最亮（不加暗）
        }

        // 应用一次初始亮度（避免刚进场 overlay 状态不对）
        OnBrightnessChanged(sliderBrightness ? sliderBrightness.value : 1f);
    }

    private void Update()
    {
        // ESC 切换暂停
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

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

    private void BackToMenu()
    {
        // 切场景前必须恢复 timeScale
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnHelp()
    {
        Debug.Log("Help clicked (TODO: open help popup)");
    }

    private void OnSave()
    {
        Debug.Log("Save clicked (TODO: save system)");
    }

    private void OnVolumeChanged(float value)
    {
        // 快速全局音量（后面要更专业可以接 AudioMixer）
        AudioListener.volume = Mathf.Clamp01(value);
    }

    private void OnBrightnessChanged(float value)
    {
        // value: 0~1（0最暗，1最亮）
        if (!brightnessOverlay) return;

        // 把亮度值映射为黑色遮罩的透明度：
        // value=1 => alpha=0（不变暗）
        // value=0 => alpha=maxDarkness（最暗）
        float alpha = (1f - Mathf.Clamp01(value)) * Mathf.Clamp01(maxDarkness);

        Color c = brightnessOverlay.color;
        c.a = alpha;
        brightnessOverlay.color = c;
    }
}