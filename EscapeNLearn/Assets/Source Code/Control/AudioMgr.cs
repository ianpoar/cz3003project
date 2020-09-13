using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    // Singleton implementation
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
        //PlayBGM(AudioConstants.BGM_PERCEPTION);
    }

    public void PlaySFX(int id)
    {
        AudioClip clip = sfxClips[id];
        if (clip != null)
            sfxSource.PlayOneShot(clip);
        else
            Debug.Log("No sfx clip with id " + id);
    }

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

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }
}
