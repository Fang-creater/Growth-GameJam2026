using UnityEngine;
using UnityEngine.UI;

public class AlbumPopupUI : MonoBehaviour
{
    [SerializeField] private MainMenuUI mainMenuUI;

    [SerializeField] private Image photoImage;
    [SerializeField] private Button btnPrev;
    [SerializeField] private Button btnNext;
    [SerializeField] private Button btnClose;

    [Header("Photos (Sprites)")]
    [SerializeField] private Sprite[] photos;

    private int index;

    private void Awake()
    {
        if (btnPrev) btnPrev.onClick.AddListener(Prev);
        if (btnNext) btnNext.onClick.AddListener(Next);
        if (btnClose) btnClose.onClick.AddListener(() => mainMenuUI.ClosePopup());

        index = 0;
        Refresh();
    }

    private void Prev()
    {
        if (photos == null || photos.Length == 0) return;
        index = (index - 1 + photos.Length) % photos.Length;
        Refresh();
    }

    private void Next()
    {
        if (photos == null || photos.Length == 0) return;
        index = (index + 1) % photos.Length;
        Refresh();
    }

    private void Refresh()
    {
        if (!photoImage) return;

        if (photos == null || photos.Length == 0)
        {
            photoImage.enabled = false;
            return;
        }

        photoImage.enabled = true;
        photoImage.sprite = photos[index];
        photoImage.preserveAspect = true;
    }
}