using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] private AudioClip bgm;
    [Range(0f, 1f)][SerializeField] private float bgmVolume = 1f;
    [SerializeField] private bool bgmLoop = true;

    [Header("Ambience")]
    [SerializeField] private AudioClip ambience;
    [Range(0f, 1f)][SerializeField] private float ambienceVolume = 1f;
    [SerializeField] private bool ambienceLoop = true;

    private void Start()
    {
        if (!AudioManager.I)
        {
            Debug.LogWarning("No AudioManager found in scene. Please add AudioManager to your initial scene (e.g., MainMenu).");
            return;
        }

        AudioManager.I.ApplyPrefs();
        AudioManager.I.PlayBgm(bgm, bgmVolume, bgmLoop);
        AudioManager.I.PlayAmbience(ambience, ambienceVolume, ambienceLoop);
    }
}