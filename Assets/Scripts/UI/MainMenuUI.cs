using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button btnStart;

    private void Awake()
    {
        if (btnStart) btnStart.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        Debug.Log("StartGame clicked, loading scene index 1");
        SceneManager.LoadScene(1);
    }
}