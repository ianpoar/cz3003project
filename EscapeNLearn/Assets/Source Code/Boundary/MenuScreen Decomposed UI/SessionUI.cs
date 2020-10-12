using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.MiniJSON;
using System.Linq;

/// <summary>
/// A UI class encapsulated in MenuScreen in the Unity scene, handles UI functionality of the session panel.
/// </summary>
public class SessionUI : MonoBehaviour
{
    public GameObject SessionUIItem;
    public Transform Panel;
    public GameObject NewSessionWindow;
    public InputField input_SessionName;
    public Dropdown dropdown_l1q;
    public Dropdown dropdown_l2q;
    public Dropdown dropdown_l3q;

    string questionlists = null;
    List<Dictionary<string, QuestionList>> qllist = new List<Dictionary<string, QuestionList>>();
    List<GameObject> list = new List<GameObject>();
    Dictionary<string, Session> session_dic = new Dictionary<string, Session>();

    /// <summary>
    /// This method executes when the session panel is displayed.
    /// </summary>
    private void OnEnable()
    {
        GenerateSessionObjects();
    }

    /// <summary>
    /// This method executes when the session panel is hidden.
    /// </summary>
    private void OnDisable()
    {
        ClearSessionObjects();
    }

    /// <summary>
    /// A method that requests for session data from SessionMgr.
    /// </summary>
    void GenerateSessionObjects()
    {
        NotificationMgr.Instance.NotifyLoad("Fetching Sessions");
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
        }, ProfileMgr.Instance.localProfile.id_player);
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
        session_dic.Clear();
    }

    /// <summary>
    /// A handler for when the new session button is pressed.
    /// </summary>
    public void Btn_ShowNewSessionWindow(bool flag)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (flag) // if show new session window, load question list first.
        {
            dropdown_l1q.options.Clear();
            dropdown_l2q.options.Clear();
            dropdown_l3q.options.Clear();
            this.qllist.Clear();

            NotificationMgr.Instance.NotifyLoad("Loading Question Lists");
            ProfileMgr.Instance.FetchMyQuestionLists(
            delegate (string result)
            {
                NotificationMgr.Instance.StopLoad();
                questionlists = result;
                Dictionary<string, object> questionlists_dict = Json.Deserialize(questionlists) as Dictionary<string, object>;
                foreach (KeyValuePair<string, object> pair in questionlists_dict)
                {
                    string questionlistdata = Json.Serialize(pair.Value);
                    QuestionList questionlist = JsonUtility.FromJson<QuestionList>(questionlistdata);
                    Dictionary<string, QuestionList> item = new Dictionary<string, QuestionList>()
                    {
                        { pair.Key, questionlist}
                    };
                    this.qllist.Add(item);
                    dropdown_l1q.options.Add(new Dropdown.OptionData(questionlist.name));
                    dropdown_l2q.options.Add(new Dropdown.OptionData(questionlist.name));
                    dropdown_l3q.options.Add(new Dropdown.OptionData(questionlist.name));

                }

                NewSessionWindow.SetActive(true);
            },
            delegate (string failmsg)
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg + "\nIf you have not created a question list, please do so first.");
                questionlists = null;
            });
        }
        else
        {
            NewSessionWindow.SetActive(false);
        }
    }

    /// <summary>
    /// A handler for when a session object is clicked, proceeds to transit the player (instructor) to view a session report.
    /// </summary>
    public void ViewSessionReport(string id)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        Debug.Log("Get session " + id);
        Session s = null;
        if (session_dic.TryGetValue(id, out s))
        {
            NotificationMgr.Instance.NotifyLoad("Fetching Session Report");
            SessionMgr.Instance.LoadAllSessionReports(id, s,
                delegate ()
                {
                    NotificationMgr.Instance.StopLoad();
                    TransitMgr.Instance.FadeToScene("Report");
                },
                delegate (string failmsg)
                {
                    NotificationMgr.Instance.StopLoad();
                    NotificationMgr.Instance.Notify(failmsg);
                });
        }
        else
        {
            Debug.Log("failed");
        }
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
            script.Init(pair.Key, session, this, null);

            list.Add(obj);
            session_dic.Add(pair.Key, session);
        }
    }

    /// <summary>
    /// A handler for when the create new button is pressed in the create new session panel.
    /// </summary>
    public void Btn_CreateNewSession()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        string sessName = input_SessionName.text;
        int l1q = dropdown_l1q.value;
        int l2q = dropdown_l2q.value;
        int l3q = dropdown_l3q.value;

        Session s = new Session();
        s.session_name = sessName;
        s.id_owner = ProfileMgr.Instance.localProfile.id_player;
        s.id_l1queslist = this.qllist[l1q].Keys.First();
        s.id_l2queslist = this.qllist[l2q].Keys.First();
        s.id_l3queslist = this.qllist[l3q].Keys.First();

        NotificationMgr.Instance.NotifyLoad("Creating Session");
        SessionMgr.Instance.CreateSession(s,
        delegate ()
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify("Session created.",
            delegate ()
            {
                NewSessionWindow.SetActive(false);
                ClearSessionObjects();
                GenerateSessionObjects();
            });
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });
    }
}