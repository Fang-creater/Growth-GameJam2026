using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClickCycleImage : MonoBehaviour
{
    [Header("Target Image (if empty, use self Image)")]
    [SerializeField] private Image targetImage;

    [Header("Sprites to cycle")]
    [SerializeField] private Sprite[] sprites;

    [Header("Click FX")]
    [SerializeField] private float punchScale = 1.08f;
    [SerializeField] private float punchDuration = 0.12f;

    private Button btn;
    private RectTransform rt;
    private Vector3 baseScale;
    private int index;

    private void Awake()
    {
        btn = GetComponent<Button>();
        rt = GetComponent<RectTransform>();
        baseScale = rt.localScale;

        if (!targetImage) targetImage = GetComponent<Image>();

        btn.onClick.AddListener(OnClick);

        // 初始化显示第一张
        index = 0;
        ApplySprite();
    }

    private void OnEnable()
    {
        // 防止重复启用后比例漂移
        if (rt) rt.localScale = baseScale;
    }

    private void OnClick()
    {
        // 切到下一张（循环）
        if (sprites != null && sprites.Length > 0)
        {
            index = (index + 1) % sprites.Length;
            ApplySprite();
        }

        // 点击回弹特效
        StopAllCoroutines();
        StartCoroutine(Punch());
    }

    private void ApplySprite()
    {
        if (!targetImage) return;
        if (sprites == null || sprites.Length == 0) return;

        targetImage.sprite = sprites[index];
        targetImage.preserveAspect = true;
        targetImage.SetNativeSize(); // 如果你不想改变大小，删掉这一行
    }

    private IEnumerator Punch()
    {
        float t = 0f;
        Vector3 up = baseScale * punchScale;

        // 快速变大
        while (t < punchDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / punchDuration;
            rt.localScale = Vector3.Lerp(baseScale, up, k);
            yield return null;
        }

        // 快速回到原大小
        t = 0f;
        while (t < punchDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / punchDuration;
            rt.localScale = Vector3.Lerp(up, baseScale, k);
            yield return null;
        }

        rt.localScale = baseScale;
    }
}