using UnityEngine;

/* Add on to these for project-wide usage */

// FOR DATABASEMGR
// need to populate DBQueryType and SetPath method as well
public static class DBConstants
{
    public const string PATH_PROFILE = "profiles/";
    public const string PATH_SESSION = "sessions/";

    public static string SetPath(DBQueryType type)
    {
        switch (type)
        {
            case DBQueryType.Load_Save_Profile:
                return PATH_PROFILE;
            case DBQueryType.Session:
                return PATH_SESSION;
            default:
                return "unknown/";
        }
    }
}
public enum DBQueryType
{
    Load_Save_Profile,
    Session
}

// SNS Types
public enum SNSType
{
    Facebook
}

// PlayerPref constants
public static class PPConstants
{
    public const string KEY_BGMVOL = "BGMVol";
    public const string KEY_SFXVOL = "SFXVol";
}

// Audio file constants
public static class AudioConstants
{
    public const int SFX_CLICK = 0;
    public const int BGM_CLEARDAY = 0;
    public const int BGM_RESULT = 1;
    public const int BGM_PERCEPTION = 2;
}

// Delegate types
public delegate void SimpleCallback();
public delegate void MessageCallback(string msg);
public delegate void Texture2DCallback(Texture2D msg);