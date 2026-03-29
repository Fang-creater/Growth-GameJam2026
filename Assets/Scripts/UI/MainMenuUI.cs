using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnAlbum;
    [SerializeField] private Button btnQuit;

    [Header("Popup Root")]
    [SerializeField] private GameObject dimmer;
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private GameObject albumPopup;

    [Header("Scene Names")]
    [SerializeField] private string levelSelectSceneName = "LevelSelect";

    private void Awake()
    {
        CloseAllPopups();

        if (btnStart) btnStart.onClick.AddListener(OpenLevelSelect);
        if (btnSetting) btnSetting.onClick.AddListener(OpenSettings);
        if (btnAlbum) btnAlbum.onClick.AddListener(OpenAlbum);
        if (btnQuit) btnQuit.onClick.AddListener(QuitGame);
    }

    private void CloseAllPopups()
    {
        if (dimmer) dimmer.SetActive(false);
        if (settingsPopup) settingsPopup.SetActive(false);
        if (albumPopup) albumPopup.SetActive(false);
    }

    private void OpenLevelSelect()
    {
        // 进入选关前把弹窗关干净（可选）
        CloseAllPopups();
        SceneManager.LoadScene(levelSelectSceneName);
    }

    private void OpenSettings()
    {
        CloseAllPopups();
        if (dimmer) dimmer.SetActive(true);
        if (settingsPopup) settingsPopup.SetActive(true);
    }

    private void OpenAlbum()
    {
        CloseAllPopups();
        if (dimmer) dimmer.SetActive(true);
        if (albumPopup) albumPopup.SetActive(true);
    }

    public void ClosePopup()
    {
        CloseAllPopups();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("QuitGame clicked (Editor will not quit).");
#else
        Application.Quit();
#endif
    }
}