using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUI_Image : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "Level01"; // 不用关卡选择时就写死
    [SerializeField] private bool useSceneFlow = true;       // 用 SceneFlow.NextSceneName

    [Header("Progress Bar")]
    [SerializeField] private Image progressFill;             // Image Type=Filled
    [Range(0f, 1f)]
    [SerializeField] private float fillSmoothSpeed = 0.8f;   // 越大越快贴近真实进度

    [Header("Percent (Optional Image)")]
    [SerializeField] private Image percentImage;             // 用图片显示百分比（可不填）
    [SerializeField] private Sprite percent25;
    [SerializeField] private Sprite percent50;
    [SerializeField] private Sprite percent75;
    [SerializeField] private Sprite percent100;

    [Header("Top Icon Swing")]
    [SerializeField] private RectTransform topIcon;          // 上面小图案
    [SerializeField] private float iconSwingAngle = 8f;
    [SerializeField] private float iconSwingSpeed = 2.5f;

    [Header("Loading Text Blink (Image)")]
    [SerializeField] private CanvasGroup loadingTextGroup;   // 挂在 loading... Image 上的 CanvasGroup
    [SerializeField] private float textBlinkSpeed = 2.5f;
    [SerializeField] private float textMinAlpha = 0.25f;
    [SerializeField] private float textMaxAlpha = 1f;

    private void Start()
    {
        Time.timeScale = 1f;

        string target = sceneToLoad;
        if (useSceneFlow)
        {
            // 需要你项目里有 SceneFlow.NextSceneName
            target = string.IsNullOrEmpty(SceneFlow.NextSceneName) ? sceneToLoad : SceneFlow.NextSceneName;
        }

        StartCoroutine(LoadRoutine(target));
    }

    private void Update()
    {
        // icon 左右摇摆（旋转）
        if (topIcon)
        {
            float z = Mathf.Sin(Time.time * iconSwingSpeed) * iconSwingAngle;
            topIcon.localRotation = Quaternion.Euler(0, 0, z);
        }

        // loading... 闪烁（Image 通过 CanvasGroup 控透明度）
        if (loadingTextGroup)
        {
            float t = (Mathf.Sin(Time.time * textBlinkSpeed) + 1f) * 0.5f; // 0~1
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
            // 真实进度（0~0.9），映射到 0~1
            float target = Mathf.Clamp01(op.progress / 0.9f);

            // 平滑显示进度条
            shown = Mathf.MoveTowards(shown, target, Time.unscaledDeltaTime * fillSmoothSpeed);

            if (progressFill)
                progressFill.fillAmount = shown;

            // 根据进度切换 25/50/75/100 图片（可选）
            UpdatePercentSprite(shown);

            // 到 100% 就激活进入场景
            if (shown >= 0.999f)
            {
                if (progressFill) progressFill.fillAmount = 1f;
                if (percentImage && percent100) percentImage.sprite = percent100;
                op.allowSceneActivation = true;
            }

            yield return null;
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