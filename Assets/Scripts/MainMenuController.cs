using System.Collections;
using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelLevelSelect;
    [SerializeField] private GameObject panelSettings;

    [Header("Toast (TMP)")]
    [SerializeField] private TMP_Text toastText;
    [SerializeField] private float toastSeconds = 1.5f;

    [Header("Level Scene Names (leave empty for now)")]
    [SerializeField] private string level1Scene = "";
    [SerializeField] private string level2Scene = "";
    [SerializeField] private string level3Scene = "";

    [Header("Default Start")]
    [Range(1, 3)]
    [SerializeField] private int defaultLevel = 1;

    private const string PrefLastLevel = "pref_last_level"; // 1/2/3
    private Coroutine _toastCo;

    private void Start()
    {
        ShowPanelMain();
        SetToast("");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // -------- Panel switch --------
    public void ShowPanelMain()
    {
        SetPanel(panelMain, true);
        SetPanel(panelLevelSelect, false);
        SetPanel(panelSettings, false);
    }

    public void ShowPanelLevelSelect()
    {
        SetPanel(panelMain, false);
        SetPanel(panelLevelSelect, true);
        SetPanel(panelSettings, false);
    }

    public void ShowPanelSettings()
    {
        SetPanel(panelMain, false);
        SetPanel(panelLevelSelect, false);
        SetPanel(panelSettings, true);
    }

    private void SetPanel(GameObject go, bool on)
    {
        if (go != null) go.SetActive(on);
    }

    // -------- Main buttons --------
    public void OnClickStartGame()
    {
        int last = PlayerPrefs.GetInt(PrefLastLevel, 0);
        if (last >= 1 && last <= 3)
        {
            TryStartLevel(last);
            return;
        }

        TryStartLevel(defaultLevel);
    }

    public void OnClickLoad()
    {
        int last = PlayerPrefs.GetInt(PrefLastLevel, 0);
        if (last < 1 || last > 3)
        {
            ShowToast("没有找到存档/进度（目前只做菜单）。");
            return;
        }

        TryStartLevel(last);
    }

    public void OnClickOpenLevelSelect() => ShowPanelLevelSelect();
    public void OnClickOpenSettings() => ShowPanelSettings();
    public void OnClickBack() => ShowPanelMain();

    // -------- Level select buttons --------
    public void OnClickLevel1() => TryStartLevel(1);
    public void OnClickLevel2() => TryStartLevel(2);
    public void OnClickLevel3() => TryStartLevel(3);

    // -------- Quit --------
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        ShowToast("编辑器模式下不会退出。");
        Debug.Log("Quit called (Editor).");
#else
        Application.Quit();
#endif
    }

    private void TryStartLevel(int levelIndex)
    {
        // 先记录玩家选的关卡，回档功能就能在菜单阶段演示
        PlayerPrefs.SetInt(PrefLastLevel, levelIndex);
        PlayerPrefs.Save();

        string sceneName = levelIndex switch
        {
            1 => level1Scene,
            2 => level2Scene,
            3 => level3Scene,
            _ => ""
        };

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            ShowToast($"已选择关卡 {levelIndex}（场景未制作，之后再接入加载）。");
            return;
        }

        // 你之后要接真正加载时，把这里改成 SceneManager.LoadScene(sceneName)
        ShowToast($"将要进入：{sceneName}（等你做好关卡场景再启用加载）");
    }

    // -------- Toast --------
    private void ShowToast(string msg)
    {
        if (_toastCo != null) StopCoroutine(_toastCo);
        _toastCo = StartCoroutine(ToastRoutine(msg));
    }

    private IEnumerator ToastRoutine(string msg)
    {
        SetToast(msg);
        yield return new WaitForSeconds(toastSeconds);
        SetToast("");
    }

    private void SetToast(string msg)
    {
        if (toastText != null) toastText.text = msg ?? "";
    }
}