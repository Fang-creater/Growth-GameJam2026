using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    private static LevelSelectUI instance;

    [Header("Scene Names")]
    [SerializeField] private string loadingSceneName = "Loading";
    [SerializeField] private string backSceneName = "MainMenu";

    [Header("Optional Unlock Control")]
    [SerializeField] private LevelSelectCard[] cards;

    private void Awake()
    {
        instance = this;

        // 这里先给一个示例：只解锁第一关，其它锁住
        // 以后你做存档后，把 unlocked 从存档读出来即可
        if (cards != null && cards.Length > 0)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (!cards[i]) continue;
                cards[i].SetUnlocked(i == 0); // 仅第0个解锁
            }
        }
    }

    public static void RequestLoad(string targetScene)
    {
        if (instance == null)
        {
            Debug.LogWarning("LevelSelectUI not found in scene.");
            return;
        }

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("Target scene is empty.");
            return;
        }

        SceneFlow.NextSceneName = targetScene;
        SceneManager.LoadScene(instance.loadingSceneName);
    }

    // 如果你有返回按钮，OnClick 直接绑这个
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(backSceneName);
    }
}