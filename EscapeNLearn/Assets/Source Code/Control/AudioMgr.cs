using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio Subsystem Interface, a Control Class that handles all audio in the game.
/// </summary>
public class AudioMgr : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static AudioMgr Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // References
    [SerializeField]
    private AudioSource bgmSource;
    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private List<AudioClip> bgmClips;
    [SerializeField]
    private List<AudioClip> sfxClips;

    private void Start()
    {
        // Read volume if exists, otherwise set default vol of 1 and save

        if (PlayerPrefs.HasKey(PPConstants.KEY_BGMVOL))
        {
            bgmSource.volume = PlayerPrefs.GetFloat(PPConstants.KEY_BGMVOL);
        }
        else
        {
            float defaultVol = 1.0f;
            bgmSource.volume = defaultVol;
            PlayerPrefs.SetFloat(PPConstants.KEY_BGMVOL, defaultVol);
        }

        if (PlayerPrefs.HasKey(PPConstants.KEY_SFXVOL))
        {
            sfxSource.volume = PlayerPrefs.GetFloat(PPConstants.KEY_SFXVOL);
        }
        else
        {
            float defaultVol = 1.0f;
            sfxSource.volume = defaultVol;
            PlayerPrefs.SetFloat(PPConstants.KEY_SFXVOL, defaultVol);
        }
    }

    /// <summary>
    /// Set background music volume.
    /// </summary>
    public void SetBGMVol(float vol)
    {
        bgmSource.volume = vol;
    }

    /// <summary>
    /// Set sound effects volume.
    /// </summary>
    public void SetSFXVol (float vol)
    {
        sfxSource.volume = vol;
    }

    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    public void PlaySFX(int id)
    {
        AudioClip clip = sfxClips[id];
        if (clip != null)
            sfxSource.PlayOneShot(clip);
        else
            Debug.Log("No sfx clip with id " + id);
    }

    /// <summary>
    /// Plays background music.
    /// </summary>
    public void PlayBGM(int id)
    {
        StopBGM();

        AudioClip clip = bgmClips[id];
        if (clip == null)
        {
            Debug.Log("No bgm clip with id " + id);
            return;
        }

        bgmSource.loop = true;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    /// <summary>
    /// Stops background music.
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }
}
