using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.MiniJSON;
using System.Linq;

public class SessionUI : MonoBehaviour
{
    public GameObject SessionUIItem;
    public Transform Panel;
    public GameObject NewSessionWindow;
    public InputField input_SessionName;
    public Dropdown dropdown_l1q;
    public Dropdown dropdown_l2q;
    public Dropdown dropdown_l3q;

    string tempstr = null;
    string questionlists = null;

    List<Dictionary<string, QuestionList>> qllist = new List<Dictionary<string, QuestionList>>();

    List<GameObject> list = new List<GameObject>();
    private void OnEnable()
    {
        tempstr = null;
        GenerateSessionObjects();
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

    void GenerateSessionObjects()
    {
        NotificationMgr.Instance.NotifyLoad("Fetching sessions");
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_SESSIONS,
            "id_owner",
            ProfileMgr.Instance.localProfile.id_account,
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

    public void Btn_ShowNewSessionWindow(bool flag)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        this.getQuestionPools();
        dropdown_l1q.options.Clear();
        dropdown_l2q.options.Clear();
        dropdown_l3q.options.Clear();
        this.qllist.Clear();

        Dictionary<string, object> questionlists_dict = Json.Deserialize(questionlists) as Dictionary<string, object>;
        foreach (KeyValuePair<string, object> pair in questionlists_dict)
        {
            string questionlistdata = Json.Serialize(pair.Value);
            QuestionList questionlist = JsonUtility.FromJson<QuestionList>(questionlistdata);
            Dictionary<string, QuestionList> item = new Dictionary<string, QuestionList>(){
                { pair.Key, questionlist}
            };
            this.qllist.Add(item);
            dropdown_l1q.options.Add(new Dropdown.OptionData(questionlist.name));
            dropdown_l2q.options.Add(new Dropdown.OptionData(questionlist.name));
            dropdown_l3q.options.Add(new Dropdown.OptionData(questionlist.name));

        }
        NewSessionWindow.SetActive(flag);
    }

    private void getQuestionPools()
    {
        NotificationMgr.Instance.NotifyLoad("Loading Question List");
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_QUESTIONLISTS,
            "id_owner",
            ProfileMgr.Instance.localProfile.id_account,
            100,
        delegate (string result)
        {
            NotificationMgr.Instance.StopLoad();
            questionlists = result;
        },
        delegate (string failmsg)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
            questionlists = null;
        });
    }

    void HideNewSessionWindow()
    {
        NewSessionWindow.SetActive(false);
    }

    public void EditSession(string id)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        Debug.Log("Edit session " + id);
    }

    void SpawnSessionObjects(string result)
    {
        Debug.Log(result);
        Dictionary<string, object> results = Json.Deserialize(result) as Dictionary<string, object>;
        Debug.Log(results.Count);

        // Todo - get name and other details of session

        foreach (KeyValuePair<string, object> pair in results)
        {
            string sessiondata = Json.Serialize(pair.Value);
            Session session = JsonUtility.FromJson<Session>(sessiondata);

            GameObject obj = Instantiate(SessionUIItem, Panel.transform.position, Quaternion.identity);

            SessionUIItem script = obj.GetComponent<SessionUIItem>();
            script.transform.SetParent(Panel);
            script.transform.localScale = SessionUIItem.transform.localScale;
            script.Init(pair.Key, session.session_name, this, null);

            list.Add(obj);
        }
    }

    public void Btn_CreateNewSession()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        string sessName = input_SessionName.text;
        int l1q = dropdown_l1q.value;
        int l2q = dropdown_l2q.value;
        int l3q = dropdown_l3q.value;

        NotificationMgr.Instance.NotifyLoad("Creating session");

        Session s = new Session();
        s.session_name = sessName;
        s.id_owner = ProfileMgr.Instance.localProfile.id_account;
        s.id_l1queslist = this.qllist[l1q].Keys.First();
        s.id_l2queslist = this.qllist[l2q].Keys.First();
        s.id_l3queslist = this.qllist[l3q].Keys.First();

        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_SESSIONS + "/", s,
        delegate (string key)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify("Session created.",
                delegate ()
                {
                    HideNewSessionWindow();
                    ClearSessionObjects();
                    GenerateSessionObjects();
                });

        },
        delegate (string failmsg2) // failed
                {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg2);
        });
    }
}