﻿using UnityEngine;
using Firebase.Auth;

/* Add on to these for project-wide usage */

/// <summary>
/// Database query constants.
/// </summary>
public static class DBQueryConstants
{
    public const string QUERY_PROFILES = "profiles";
    public const string QUERY_SESSIONS = "sessions";
    public const string QUERY_CONNECTIONS = "connections";
    public const string QUERY_QUESTIONS = "questions";
    public const string QUERY_QUESTIONLISTS = "questionlists";
    public const string QUERY_REPORTS = "reports";
    public const string QUERY_CHALLENGES = "challenges";
}

/// <summary>
/// Login type constants.
/// </summary>
public static class LoginTypeConstants
{
    public const string FACEBOOK = "facebook.com";
    public const string EMAIL = "password";
    public const string GOOGLE = "google.com";
}

/// <summary>
/// PlayerPref (local savedata) constants.
/// </summary>
public static class PPConstants
{
    public const string KEY_BGMVOL = "BGMVol";
    public const string KEY_SFXVOL = "SFXVol";
    public const string BUGFIX_StoppedAppInProfileCreation = "bugfix1";
}

/// <summary>
/// Audio file constants.
/// </summary>
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