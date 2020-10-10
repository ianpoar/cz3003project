using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.MiniJSON;

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

    List<GameObject> list = new List<GameObject>();
    Dictionary<string, Session> session_dic = new Dictionary<string, Session>();

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
        session_dic.Clear();
    }

    public void Btn_ShowNewSessionWindow(bool flag)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        NewSessionWindow.SetActive(flag);
    }

    void HideNewSessionWindow()
    {
        NewSessionWindow.SetActive(false);
    }

    public void ViewSessionReport(string id)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        Debug.Log("Get session " + id);
        Session s = null;
        if (session_dic.TryGetValue(id, out s))
        {
            NotificationMgr.Instance.NotifyLoad("Fetching session reports");
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
            script.Init(pair.Key, session, this, null);

            list.Add(obj);
            session_dic.Add(pair.Key, session);
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
        s.id_l1queslist = l1q.ToString();
        s.id_l2queslist = l2q.ToString();
        s.id_l3queslist = l3q.ToString();

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