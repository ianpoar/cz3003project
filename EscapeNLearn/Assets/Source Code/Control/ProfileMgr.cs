using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class Profile
{
    public int number = 0;
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
