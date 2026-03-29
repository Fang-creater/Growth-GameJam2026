using UnityEngine;

public class MenuBackgroundParallax : MonoBehaviour
{
    [SerializeField] private RectTransform background; // BackgroundLong
    [SerializeField] private float maxMoveX = 80f;     // 左右最多移动像素
    [SerializeField] private float maxMoveY = 30f;     // 上下最多移动像素
    [SerializeField] private float smooth = 8f;

    private Vector2 basePos;
    private Vector2 targetPos;

    private void Awake()
    {
        if (!background) background = (RectTransform)transform;
        basePos = background.anchoredPosition;
        targetPos = basePos;
    }

    private void Update()
    {
        Vector2 m = Input.mousePosition;
        float nx = (m.x / Screen.width) * 2f - 1f;   // -1..1
        float ny = (m.y / Screen.height) * 2f - 1f;  // -1..1

        targetPos = basePos + new Vector2(nx * maxMoveX, ny * maxMoveY);
        background.anchoredPosition = Vector2.Lerp(background.anchoredPosition, targetPos, Time.unscaledDeltaTime * smooth);
    }
}