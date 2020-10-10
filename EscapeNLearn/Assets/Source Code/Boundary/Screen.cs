using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Screen : MonoBehaviour
{
    private bool run = false;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if (GameObject.Find("StaticObjects") == null)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load("LoadedPrefabs/StaticObjects") as GameObject);
            obj.name = "StaticObjects";
            GameObject.DontDestroyOnLoad(obj);
        }
    }

    protected virtual void Start()
    {
        // Fetch data without displaying notifications
        StartCoroutine(AutoFetchData());
    }

    private void Update()
    {
        if (run)
        {
            StartAfterDataFetched();
            run = false;
        }
    }

    protected virtual void StartAfterDataFetched() { }

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
