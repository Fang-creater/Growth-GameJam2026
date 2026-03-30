using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource ambienceSource;

    // 跟你项目现有设置保持一致
    private const string PrefMute = "settings.mute";
    private const string PrefVol = "settings.volume";

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        ApplyPrefs();
    }

    public void ApplyPrefs()
    {
        bool mute = PlayerPrefs.GetInt(PrefMute, 0) == 1;
        float vol = PlayerPrefs.GetFloat(PrefVol, 1f);

        AudioListener.volume = mute ? 0f : Mathf.Clamp01(vol);
    }

    public void PlayBgm(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (!bgmSource) return;

        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = Mathf.Clamp01(volume);
        bgmSource.loop = loop;

        if (clip) bgmSource.Play();
        else bgmSource.Stop();
    }

    public void PlayAmbience(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (!ambienceSource) return;

        if (ambienceSource.clip == clip && ambienceSource.isPlaying) return;

        ambienceSource.clip = clip;
        ambienceSource.volume = Mathf.Clamp01(volume);
        ambienceSource.loop = loop;

        if (clip) ambienceSource.Play();
        else ambienceSource.Stop();
    }

    public void StopAll()
    {
        if (bgmSource) bgmSource.Stop();
        if (ambienceSource) ambienceSource.Stop();
    }
}