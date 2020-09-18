using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Screen : MonoBehaviour
{
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

    IEnumerator AutoFetchData()
    {
        yield return new WaitForSeconds(0.5f);
        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            Debug.Log("AutoFetchData run");
            ProfileMgr.Instance.LoadPlayerProfile(
                delegate () // success
                {
                    Debug.Log("AutoFetchData success");
                },
                delegate (string failmsg)
                {
                    Debug.Log("AutoFetchData failed");
                });
        }
    }
}
