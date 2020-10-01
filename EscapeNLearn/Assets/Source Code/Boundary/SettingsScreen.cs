using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScreen : MonoBehaviour
{

    public float BGMvol=1f;
    public float SFXvol=1f;

    void Start()
    {
        
    }

    void Update()
    {
        AudioMgr.Instance.SetBGMVol(BGMvol);
        AudioMgr.Instance.SetSFXVol(SFXvol);
    }

    public void SetBGMVol(float vol)
    {
       BGMvol=vol;
    }
    public void SetSFXVol(float vol)
    {
       SFXvol=vol;
    }
    
}
