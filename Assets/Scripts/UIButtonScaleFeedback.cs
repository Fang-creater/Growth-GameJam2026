using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UIButtonScaleFeedback : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float downScale = 0.98f;
    [SerializeField] private float tweenTime = 0.08f;

    private RectTransform _rt;
    private Vector3 _baseScale;
    private Coroutine _co;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _baseScale = _rt.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData) => TweenTo(_baseScale * hoverScale);
    public void OnPointerExit(PointerEventData eventData) => TweenTo(_baseScale);
    public void OnPointerDown(PointerEventData eventData) => TweenTo(_baseScale * downScale);
    public void OnPointerUp(PointerEventData eventData) => TweenTo(_baseScale * hoverScale);

    private void TweenTo(Vector3 target)
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(TweenRoutine(target));
    }

    private IEnumerator TweenRoutine(Vector3 target)
    {
        Vector3 start = _rt.localScale;
        float t = 0f;

        while (t < tweenTime)
        {
            t += Time.unscaledDeltaTime;
            float p = tweenTime <= 0f ? 1f : Mathf.Clamp01(t / tweenTime);
            _rt.localScale = Vector3.Lerp(start, target, p);
            yield return null;
        }

        _rt.localScale = target;
    }
}