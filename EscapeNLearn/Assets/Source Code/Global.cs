using UnityEngine;
using Firebase.Auth;

/* Add on to these for project-wide usage */

// FOR DATABASEMGR
// need to populate DBQueryType and SetPath method as well
public static class DBQueryConstants
{
    public const string QUERY_PROFILES = "profiles";
    public const string QUERY_SESSIONS = "sessions";
}

public static class LoginTypeConstants
{
    public const string FACEBOOK = "facebook.com";
    public const string EMAIL = "password";
}

// Scene constants
public static class SceneConstants
{
    public const string SCENE_SPLASH = "Splash";
    public const string SCENE_LOGIN = "Login";
    public const string SCENE_MENU = "Menu";
}

// PlayerPref constants
public static class PPConstants
{
    public const string KEY_BGMVOL = "BGMVol";
    public const string KEY_SFXVOL = "SFXVol";
    public const string BUGFIX_StoppedAppInProfileCreation = "bugfix1";
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
public delegate void SpriteCallback(Sprite sprite);
public delegate void CredentialCallback(Credential cred);