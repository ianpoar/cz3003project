using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.MiniJSON;

/// <summary>
/// A UI class encapsulated in MenuScreen in the Unity scene, handles UI functionality of the session lobby panel.
/// </summary>
public class SessionLobbyUI : MonoBehaviour
{
    public GameObject SessionUIItem;
    public Transform Panel;
    public InputField input_InstructorName;
    public GameObject panel_LevelSelect;

    List<GameObject> list = new List<GameObject>();

    /// <summary>
    /// This method executes when the session lobby panel is displayed.
    /// </summary>
    private void OnEnable()
    {
        GenerateAllSessionObjects();
    }

    /// <summary>
    /// This method executes when the session lobby panel is hidden.
    /// </summary>
    private void OnDisable()
    {
        ClearSessionObjects();
    }

    /// <summary>
    /// This method executes when the session lobby panel is hidden.
    /// </summary>
    public void Btn_HideSessionLobby(bool playsound)
    {
        if (playsound)
            AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// A handler for when the search button is pressed.
    /// </summary>
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

    /// <summary>
    /// A method that requests for all session data from SessionMgr.
    /// </summary>
    void GenerateAllSessionObjects()
    {
        NotificationMgr.Instance.NotifyLoad("Fetching sessions");
        SessionMgr.Instance.FetchSessions(
         delegate (string result)
         {
             NotificationMgr.Instance.StopLoad();
             SpawnSessionObjects(result);
         },
         delegate (string failmsg)
         {
             NotificationMgr.Instance.StopLoad();
             NotificationMgr.Instance.Notify(failmsg);
         });
    }

    /// <summary>
    /// A method that requests for session data that belongs to a specific instructor from SessionMgr.
    /// </summary>
    void GenerateSearchSessionObjects(string id)
    {
        NotificationMgr.Instance.NotifyLoad("Fetching sessions");
        SessionMgr.Instance.FetchSessions(
        delegate (string result)
        {
            NotificationMgr.Instance.StopLoad();
            SpawnSessionObjects(result);
        },
        delegate (string failmsg)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        }, id);
    }

    /// <summary>
    /// A method to clear displayed session objects.
    /// </summary>
    void ClearSessionObjects()
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }
        list.Clear();
    }

    /// <summary>
    /// A method that allows the player to join a session when a session object is clicked.
    /// </summary>
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

        SessionMgr.Instance.JoinSession(c,
        delegate () // success
        {
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

    /// <summary>
    /// A method that spawns session objects to be displayed.
    /// </summary>
    void SpawnSessionObjects(string result)
    {
        Dictionary<string, object> results = Json.Deserialize(result) as Dictionary<string, object>;

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