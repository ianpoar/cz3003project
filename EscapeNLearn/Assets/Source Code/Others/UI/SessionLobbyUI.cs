using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.MiniJSON;

/// <summary>
/// A UI class that provides session lobby functionalities, encapsulated in the menu scene in Unity.
/// </summary>
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
            nameof(Session.id_owner), id,
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

    public void JoinSession(string sessionid, string ownerid, string sessionname)
    {
        Debug.Log("Join session " + sessionid);
        NotificationMgr.Instance.NotifyLoad("Joining session");
        Connection c = new Connection();

        c.id_player = ProfileMgr.Instance.localProfile.id_player;
        c.id_session = sessionid;
        c.id_owner = ownerid;
        c.session_name = sessionname;
        c.level_cleared = 0;

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
            script.Init(pair.Key, session, null, this);

            list.Add(obj);
        }
    }
}