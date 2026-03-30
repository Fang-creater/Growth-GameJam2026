using UnityEngine;
using UnityEngine.SceneManagement;
using Regrowth;

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

        int maxUnlocked = LevelProgress.GetMaxUnlocked();

        if (cards != null && cards.Length > 0)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (!cards[i]) continue;

                // LevelSelectCard.SceneName 例如 "Level1"
                string scenePath = $"Assets/Scenes/{cards[i].SceneName}.unity";
                int buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);

                // buildIndex == -1 表示这个场景没加入 Build Settings，会保持锁住
                bool unlocked = buildIndex >= 0 && buildIndex <= maxUnlocked;
                cards[i].SetUnlocked(unlocked);
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

        // 你当前项目里 LoadingController 用的是 SceneLoadRequest.TargetSceneName
        // 但 LevelSelectUI 这里写的是 SceneFlow.NextSceneName（两套方案混在一起）
        // 为了确保能正常从 Loading 进入关卡，这里两个都赋值一次（兼容）
        SceneFlow.NextSceneName = targetScene;
        SceneLoadRequest.TargetSceneName = targetScene;

        SceneManager.LoadScene(instance.loadingSceneName);
    }

    // 如果你有返回按钮，OnClick 直接绑这个
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(backSceneName);
    }
}