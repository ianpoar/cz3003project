using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI : MonoBehaviour
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
        StartCoroutine(AutoFetchData());
    }

    IEnumerator AutoFetchData()
    {
        yield return new WaitForSeconds(0.5f);
        if (DatabaseMgr.Instance.IsLoggedIn && !DatabaseMgr.Instance.LastFBFetchSuccess)
        {
            NotificationMgr.Instance.NotifyLoad("Fetching data");
            DatabaseMgr.Instance.LoadPlayerProfile(
                delegate () // success
                {
                    NotificationMgr.Instance.StopLoad();
                },
                delegate (string failmsg)
                {
                    NotificationMgr.Instance.StopLoad();
                    NotificationMgr.Instance.Notify(failmsg);
                });
        }
    }
}
