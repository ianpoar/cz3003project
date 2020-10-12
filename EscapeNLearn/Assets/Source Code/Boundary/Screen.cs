using System.Collections;
using UnityEngine;

/// <summary>
/// UI Boundary Class for all Unity scenes,  allows all scenes to automatically refresh player data.
/// </summary>
public abstract class Screen : MonoBehaviour
{
    private bool run = false;

    /// <summary>
    /// The first method called when the application runs, instantiates static objects and Singleton classes.
    /// </summary>
    protected virtual void Awake()
    {
        if (GameObject.Find("StaticObjects") == null)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load("LoadedPrefabs/StaticObjects") as GameObject);
            obj.name = "StaticObjects";
            GameObject.DontDestroyOnLoad(obj);
            Application.targetFrameRate = 300; // highest frame rate possible
        }
    }

    /// <summary>
    /// The start of a screen, automatically fetches profile data if applicable, usually overriden by derived classes.
    /// </summary>
    protected virtual void Start()
    {
        // Fetch data without displaying notifications
        StartCoroutine(AutoFetchData());
    }

    /// <summary>
    /// A update loop that executes StartAfterDataFetched once after profile data is fetched.
    /// </summary>
    private void Update()
    {
        if (run)
        {
            StartAfterDataFetched();
            run = false;
        }
    }

    /// <summary>
    /// A method that is executed only after profile data is fetched, usually overriden by derived classes to specify different actions to perform in different screens.
    /// </summary>
    protected virtual void StartAfterDataFetched() { }


    /// <summary>
    /// A coroutine that fetches local and online profile data.
    /// </summary>
    IEnumerator AutoFetchData()
    {
        yield return new WaitForSeconds(0.1f);
        
        // Bug fix for de-sync between user account and profile
        if (PlayerPrefs.GetInt(PPConstants.BUGFIX_StoppedAppInProfileCreation) == 1)
        {
            DatabaseMgr.Instance.Logout();
            PlayerPrefs.SetInt(PPConstants.BUGFIX_StoppedAppInProfileCreation, 0);
        }

        if (DatabaseMgr.Instance.IsLoggedIn) // if logged in
        {
            if (DatabaseMgr.Instance.LoginTypes.Contains(LoginTypeConstants.FACEBOOK)) // if facebook login
            {
                DatabaseMgr.Instance.SNSRequestCredential(LoginTypeConstants.FACEBOOK, null, null, true); // perform soft login
            }
            else if (DatabaseMgr.Instance.LoginTypes.Contains(LoginTypeConstants.GOOGLE)) // if google login
            {
                DatabaseMgr.Instance.SNSRequestCredential(LoginTypeConstants.GOOGLE, null, null, true); // perform soft login
            }

            // Auto fetch data
            Debug.Log("AutoFetchData run");
            ProfileMgr.Instance.LoadPlayerProfile(
                delegate () // success
                {
                    Debug.Log("AutoFetchData success");
                    run = true;         
                },
                delegate (string failmsg)
                {
                    Debug.Log("AutoFetchData failed");
                    run = true;    
                });
        }
    }
}
