using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TitleMushroomClick : MonoBehaviour
{
    [Header("Mushroom Popup")]
    [SerializeField] private GameObject mushroomPopup;     // MushroomPopup
    [SerializeField] private CanvasGroup mushroomGroup;    // MushroomPopup ÉÏµÄ CanvasGroup
    [SerializeField] private RectTransform mushroomRT;     // MushroomPopup µÄ RectTransform

    [Header("Popup Anim")]
    [SerializeField] private float showTime = 1.2f;
    [SerializeField] private float fadeInTime = 0.15f;
    [SerializeField] private float fadeOutTime = 0.2f;
    [SerializeField] private float popScale = 1.15f;

    [Header("Title Click FX")]
    [SerializeField] private float punchScale = 1.06f;
    [SerializeField] private float punchDuration = 0.10f;

    private Button btn;
    private RectTransform titleRT;
    private Vector3 titleBaseScale;
    private Vector3 mushroomBaseScale;

    private void Awake()
    {
        btn = GetComponent<Button>();
        titleRT = GetComponent<RectTransform>();
        titleBaseScale = titleRT.localScale;

        if (mushroomPopup && !mushroomGroup) mushroomGroup = mushroomPopup.GetComponent<CanvasGroup>();
        if (mushroomPopup && !mushroomRT) mushroomRT = mushroomPopup.GetComponent<RectTransform>();

        if (mushroomRT) mushroomBaseScale = mushroomRT.localScale;

        btn.onClick.AddListener(OnClick);

        HideImmediate();
    }

    private void OnEnable()
    {
        if (titleRT) titleRT.localScale = titleBaseScale;
    }

    private void OnClick()
    {
        StopAllCoroutines();
        StartCoroutine(TitlePunch());

        if (mushroomPopup) StartCoroutine(ShowMushroom());
    }

    private IEnumerator TitlePunch()
    {
        float t = 0f;
        Vector3 up = titleBaseScale * punchScale;

        while (t < punchDuration)
        {
            t += Time.unscaledDeltaTime;
            titleRT.localScale = Vector3.Lerp(titleBaseScale, up, t / punchDuration);
            yield return null;
        }

        t = 0f;
        while (t < punchDuration)
        {
            t += Time.unscaledDeltaTime;
            titleRT.localScale = Vector3.Lerp(up, titleBaseScale, t / punchDuration);
            yield return null;
        }

        titleRT.localScale = titleBaseScale;
    }

    private IEnumerator ShowMushroom()
    {
        mushroomPopup.SetActive(true);

        if (mushroomGroup) mushroomGroup.alpha = 0f;
        if (mushroomRT) mushroomRT.localScale = mushroomBaseScale;

        // fade + pop in
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeInTime;

            if (mushroomGroup) mushroomGroup.alpha = Mathf.Lerp(0f, 1f, k);
            if (mushroomRT) mushroomRT.localScale = Vector3.Lerp(mushroomBaseScale, mushroomBaseScale * popScale, k);

            yield return null;
        }

        if (mushroomGroup) mushroomGroup.alpha = 1f;

        // hold
        yield return new WaitForSecondsRealtime(showTime);

        // fade out
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeOutTime;

            if (mushroomGroup) mushroomGroup.alpha = Mathf.Lerp(1f, 0f, k);
            yield return null;
        }

        HideImmediate();
    }

    private void HideImmediate()
    {
        if (mushroomPopup) mushroomPopup.SetActive(false);
        if (mushroomGroup) mushroomGroup.alpha = 0f;
        if (mushroomRT) mushroomRT.localScale = mushroomBaseScale;
    }
}