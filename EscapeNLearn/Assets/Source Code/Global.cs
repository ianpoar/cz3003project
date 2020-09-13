using UnityEngine.UI;
using UnityEngine;

/* Add on to these for project-wide usage */

public static class DBConstants // Database
{
    public const string PATH_PROFILE = "profiles/";
    public const string PATH_SESSION = "sessions/";
}

public static class PPConstants // PlayerPref
{
    public const string KEY_BGMVOL = "BGMVol";
    public const string KEY_SFXVOL = "SFXVol";
}

public static class AudioConstants
{
    public const int SFX_CLICK = 0;
    public const int BGM_CLEARDAY = 0;
    public const int BGM_RESULT = 1;
    public const int BGM_PERCEPTION = 2;
}


public delegate void SimpleCallback();
public delegate void MessageCallback(string msg);
public delegate void Texture2DCallback(Texture2D msg);