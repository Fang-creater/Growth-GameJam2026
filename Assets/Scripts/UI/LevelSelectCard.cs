using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Wiring")]
    [SerializeField] private Image cardBG;
    [SerializeField] private Image glow;
    [SerializeField] private Button playButton;
    [SerializeField] private Image lockIcon;

    [Header("Level")]
    [SerializeField] private string sceneName = "Level01";
    [SerializeField] private bool unlocked = true;

    [Header("Hover FX")]
    [SerializeField] private float hoverScale = 1.12f;
    [SerializeField] private float hoverSpeed = 10f;
    [SerializeField] private float bgDimWhenNotHover = 0.78f; // 未hover时暗一点
    [SerializeField] private float bgBrightOnHover = 1.00f;   // hover时正常亮度

    [Header("Glow FX")]
    [SerializeField] private float glowMaxAlpha = 0.55f;
    [SerializeField] private float glowPulseSpeed = 2.0f;

    [Header("Click FX")]
    [SerializeField] private float clickPunchScale = 1.18f;
    [SerializeField] private float clickPunchDuration = 0.10f;

    private RectTransform btnRT;
    private Vector3 btnBaseScale;
    private bool hovering;
    private Coroutine clickCo;

    public string SceneName => sceneName;
    public bool Unlocked => unlocked;

    private void Awake()
    {
        if (playButton) btnRT = playButton.GetComponent<RectTransform>();
        if (btnRT) btnBaseScale = btnRT.localScale;

        ApplyLockedState();

        if (playButton)
            playButton.onClick.AddListener(OnClickPlay);
    }

    private void OnEnable()
    {
        // 防止反复启用后缩放漂移
        if (btnRT) btnRT.localScale = btnBaseScale;
        hovering = false;
        ApplyVisual(instant: true);
    }

    public void SetUnlocked(bool value)
    {
        unlocked = value;
        ApplyLockedState();
        ApplyVisual(instant: true);
    }

    private void ApplyLockedState()
    {
        if (lockIcon) lockIcon.enabled = !unlocked;
        if (playButton) playButton.interactable = unlocked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        ApplyVisual(instant: false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        ApplyVisual(instant: false);
    }

    private void Update()
    {
        // 让 Glow 呼吸（只有 hover 时或解锁时才动，你也可改成一直动）
        if (glow)
        {
            float targetA = 0f;

            if (unlocked && hovering)
            {
                float pulse = (Mathf.Sin(Time.unscaledTime * glowPulseSpeed) + 1f) * 0.5f; // 0..1
                targetA = Mathf.Lerp(glowMaxAlpha * 0.5f, glowMaxAlpha, pulse);
            }

            Color c = glow.color;
            c.a = Mathf.Lerp(c.a, targetA, Time.unscaledDeltaTime * 8f);
            glow.color = c;
        }

        // hover 缩放播放按钮（平滑）
        if (btnRT)
        {
            float s = hovering && unlocked ? hoverScale : 1f;
            Vector3 target = btnBaseScale * s;
            btnRT.localScale = Vector3.Lerp(btnRT.localScale, target, Time.unscaledDeltaTime * hoverSpeed);
        }

        // 背景亮度变化（平滑）
        if (cardBG)
        {
            float v = hovering ? bgBrightOnHover : bgDimWhenNotHover;
            if (!unlocked) v = 0.45f; // 未解锁更暗

            Color c = cardBG.color;
            float current = c.r; // 假设是灰度（我们用统一RGB）
            float next = Mathf.Lerp(current, v, Time.unscaledDeltaTime * 8f);
            cardBG.color = new Color(next, next, next, c.a);
        }
    }

    private void ApplyVisual(bool instant)
    {
        // 这里主要让初始状态正确；动态效果在 Update 里
        if (!instant) return;

        if (cardBG)
        {
            float v = hovering ? bgBrightOnHover : bgDimWhenNotHover;
            if (!unlocked) v = 0.45f;
            cardBG.color = new Color(v, v, v, cardBG.color.a);
        }

        if (glow)
        {
            Color c = glow.color;
            c.a = 0f;
            glow.color = c;
        }

        if (btnRT)
        {
            btnRT.localScale = btnBaseScale;
        }
    }

    private void OnClickPlay()
    {
        if (!unlocked) return;

        // 点击回弹
        if (clickCo != null) StopCoroutine(clickCo);
        clickCo = StartCoroutine(ClickPunch());

        // 交给 LevelSelectUI 去加载（更统一）
        LevelSelectUI.RequestLoad(sceneName);
    }

    private IEnumerator ClickPunch()
    {
        if (!btnRT) yield break;

        float t = 0f;
        Vector3 up = btnBaseScale * clickPunchScale;

        while (t < clickPunchDuration)
        {
            t += Time.unscaledDeltaTime;
            btnRT.localScale = Vector3.Lerp(btnBaseScale, up, t / clickPunchDuration);
            yield return null;
        }

        t = 0f;
        while (t < clickPunchDuration)
        {
            t += Time.unscaledDeltaTime;
            btnRT.localScale = Vector3.Lerp(up, btnBaseScale, t / clickPunchDuration);
            yield return null;
        }

        btnRT.localScale = btnBaseScale;
    }
}