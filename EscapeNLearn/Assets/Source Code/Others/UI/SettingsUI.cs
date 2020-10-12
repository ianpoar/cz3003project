using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI class encapsulated in MenuScreen in the Unity scene, handles UI functionality of the settings panel.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    public Slider SFXSlider;
    public Slider BGMSlider;
    float BGMvol=1f;
    float SFXvol=1f;

    void OnEnable()
    {
        if (PlayerPrefs.HasKey(PPConstants.KEY_BGMVOL))
        {
            BGMvol = PlayerPrefs.GetFloat(PPConstants.KEY_BGMVOL);
        }

        if (PlayerPrefs.HasKey(PPConstants.KEY_SFXVOL))
        {
            SFXvol = PlayerPrefs.GetFloat(PPConstants.KEY_SFXVOL);
        }

        SFXSlider.value = SFXvol;
        BGMSlider.value = BGMvol;
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(PPConstants.KEY_BGMVOL, BGMvol);
        PlayerPrefs.SetFloat(PPConstants.KEY_SFXVOL, SFXvol);
    }

    public void SetBGMVol(float vol)
    {
        BGMvol=vol;
        AudioMgr.Instance.SetBGMVol(BGMvol);
    }
    public void SetSFXVol(float vol)
    {
       SFXvol=vol;
        AudioMgr.Instance.SetSFXVol(SFXvol);
    }
    
}
