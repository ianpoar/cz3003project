using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class Profile
{
    public string name = "Unknown";
    public string accountType = "Student";
    public int accountExp = 0;
    public int currency_normal = 0;
    public int currency_premium = 0;
}

public class ProfileMgr : MonoBehaviour // Singleton class
{
    // Singleton implementation
    public static ProfileMgr Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Public variables
    public Profile localProfile = new Profile();
}
