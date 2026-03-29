using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUI_Image : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "Level01";
    [SerializeField] private bool useSceneFlow = true;

    [Header("Progress Bar")]
    [SerializeField] private Image progressFill;             // Image Type=Filled
    [SerializeField] private float fillSmoothSpeed = 1.2f;   // 建议 1~6 比较像“速度”
    [SerializeField] private bool unscaledTime = true;       // 防止 timeScale 影响

    [Header("Percent (Optional Image)")]
    [SerializeField] private Image percentImage;
    [SerializeField] private Sprite percent25;
    [SerializeField] private Sprite percent50;
    [SerializeField] private Sprite percent75;
    [SerializeField] private Sprite percent100;

    [Header("Top Icon Swing")]
    [SerializeField] private RectTransform topIcon;
    [SerializeField] private float iconSwingAngle = 8f;
    [SerializeField] private float iconSwingSpeed = 2.5f;

    [Header("Loading Text Blink (Image)")]
    [SerializeField] private CanvasGroup loadingTextGroup;
    [SerializeField] private float textBlinkSpeed = 2.5f;
    [SerializeField] private float textMinAlpha = 0.25f;
    [SerializeField] private float textMaxAlpha = 1f;

    [Header("Bottom Flicker (optional)")]
    [Tooltip("底部用一个 Image 来轮播 Sprite；不想用就留空。")]
    [SerializeField] private Image bottomImage;
    [SerializeField] private Sprite[] bottomSprites;
    [SerializeField] private float bottomSwapInterval = 0.12f;
    [SerializeField] private bool bottomRandom = true;

    private Coroutine flickerCo;

    private void Start()
    {
        // 这里不强制改 timeScale 也行；你要保留就保留
        Time.timeScale = 1f;

        if (!progressFill)
            Debug.LogWarning("LoadingUI_Image: progressFill is not assigned (拖你的 ProgressFill Image 到这里).", this);

        string target = sceneToLoad;
        if (useSceneFlow)
            target = string.IsNullOrEmpty(SceneFlow.NextSceneName) ? sceneToLoad : SceneFlow.NextSceneName;

        if (bottomImage && bottomSprites != null && bottomSprites.Length > 0)
            flickerCo = StartCoroutine(BottomFlickerRoutine());

        StartCoroutine(LoadRoutine(target));
    }

    private void OnDisable()
    {
        if (flickerCo != null)
        {
            StopCoroutine(flickerCo);
            flickerCo = null;
        }
    }

    private void Update()
    {
        float dt = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float tTime = unscaledTime ? Time.unscaledTime : Time.time;

        if (topIcon)
        {
            float z = Mathf.Sin(tTime * iconSwingSpeed) * iconSwingAngle;
            topIcon.localRotation = Quaternion.Euler(0, 0, z);
        }

        if (loadingTextGroup)
        {
            float t = (Mathf.Sin(tTime * textBlinkSpeed) + 1f) * 0.5f;
            loadingTextGroup.alpha = Mathf.Lerp(textMinAlpha, textMaxAlpha, t);
        }
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float shown = 0f;

        while (!op.isDone)
        {
            float real01 = Mathf.Clamp01(op.progress / 0.9f);

            float dt = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            // 让 shown 以“速度”的方式追上 real01（比 MoveTowards 更像平滑条）
            shown = Mathf.Lerp(shown, real01, 1f - Mathf.Exp(-fillSmoothSpeed * dt));

            if (progressFill)
                progressFill.fillAmount = shown;

            UpdatePercentSprite(shown);

            if (real01 >= 1f && shown >= 0.99f)
            {
                if (progressFill) progressFill.fillAmount = 1f;
                if (percentImage && percent100) percentImage.sprite = percent100;
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator BottomFlickerRoutine()
    {
        int i = 0;

        while (true)
        {
            if (!bottomImage || bottomSprites == null || bottomSprites.Length == 0)
                yield break;

            if (bottomRandom)
            {
                bottomImage.sprite = bottomSprites[Random.Range(0, bottomSprites.Length)];
            }
            else
            {
                bottomImage.sprite = bottomSprites[i];
                i = (i + 1) % bottomSprites.Length;
            }

            yield return new WaitForSecondsRealtime(bottomSwapInterval);
        }
    }

    private void UpdatePercentSprite(float progress01)
    {
        if (!percentImage) return;

        if (progress01 < 0.25f)
        {
            if (percent25) percentImage.sprite = percent25;
        }
        else if (progress01 < 0.5f)
        {
            if (percent50) percentImage.sprite = percent50;
        }
        else if (progress01 < 0.75f)
        {
            if (percent75) percentImage.sprite = percent75;
        }
        else
        {
            if (percent100) percentImage.sprite = percent100;
        }
    }
}
