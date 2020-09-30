using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.MiniJSON;

public class SessionLobbyUI : MonoBehaviour
{
    public GameObject SessionUIItem;
    public Transform Panel;
    public InputField input_InstructorName;
    public GameObject panel_LevelSelect;

    string tempstr = null;

    List<GameObject> list = new List<GameObject>();
    private void OnEnable()
    {
        tempstr = null;
        GenerateAllSessionObjects();
    }

    private void OnDisable()
    {
        ClearSessionObjects();
    }

    private void Update()
    {
        // To address bug of Instantiate not working in delegates...
        if (tempstr != null)
        {
            SpawnSessionObjects(tempstr);
            tempstr = null;
        }
    }

    public void Btn_HideSessionLobby(bool playsound)
    {
        if (playsound)
            AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        this.gameObject.SetActive(false);
    }


    public void Btn_Search()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        ClearSessionObjects();
        if (input_InstructorName.text != "")
        {
            GenerateSearchSessionObjects(input_InstructorName.text);
        }
        else
        {
            GenerateAllSessionObjects();
        }
    }

    void GenerateAllSessionObjects()
    {
        NotificationMgr.Instance.NotifyLoad("Fetching sessions");
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_SESSIONS,
            null, null,
            100,
        delegate (string result)
        {
            NotificationMgr.Instance.StopLoad();
            tempstr = result;
        },
        delegate (string failmsg)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });
    }

    void GenerateSearchSessionObjects(string id)
    {
        NotificationMgr.Instance.NotifyLoad("Fetching sessions");
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_SESSIONS,
            "id_owner", id,
            100,
        delegate (string result)
        {
            NotificationMgr.Instance.StopLoad();
            tempstr = result;
        },
        delegate (string failmsg)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });
    }


    void ClearSessionObjects()
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }
        list.Clear();
    }

    public void JoinSession(string sessionid)
    {
        Debug.Log("Join session " + sessionid);
        NotificationMgr.Instance.NotifyLoad("Joining session");
        Connection c = new Connection();

        c.id_player = ProfileMgr.Instance.localProfile.id_account;
        c.id_session = sessionid;

        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_CONNECTIONS,
        c,
        delegate (string key)
        {
            ProfileMgr.Instance.currentConnection = c;
            ProfileMgr.Instance.connectionID = key;
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify("Joined session successfully.",
            delegate
            {
                panel_LevelSelect.SetActive(true);
                Btn_HideSessionLobby(false);
            });
        },
        delegate (string failmsg)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });
    }

    void SpawnSessionObjects(string result)
    {
        Debug.Log(result);
        Dictionary<string, object> results = Json.Deserialize(result) as Dictionary<string, object>;
        Debug.Log(results.Count);

        foreach (KeyValuePair<string, object> pair in results)
        {
            string sessiondata = Json.Serialize(pair.Value);
            Session session = JsonUtility.FromJson<Session>(sessiondata);

            GameObject obj = Instantiate(SessionUIItem, Panel.transform.position, Quaternion.identity);

            SessionUIItem script = obj.GetComponent<SessionUIItem>();
            script.transform.SetParent(Panel);
            script.transform.localScale = SessionUIItem.transform.localScale;
            script.Init(pair.Key, session.session_name, null, this);

            list.Add(obj);
        }
    }
}

public class Connection
{
    public string id_session;
    public string id_player;
}