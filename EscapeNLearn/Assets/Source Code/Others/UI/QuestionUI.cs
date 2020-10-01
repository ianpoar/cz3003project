using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Facebook.MiniJSON;
using System.Net.Mail;
using System.Linq;

public class QuestionUI : MonoBehaviour
{
    [SerializeField]
    public InputField questionField;
    [SerializeField]
    public InputField answer1Field;
    [SerializeField]
    public InputField answer2Field;
    [SerializeField]
    public InputField answer3Field;
    [SerializeField]
    public InputField answer4Field;
    [SerializeField]
    public Dropdown correctAnswerDD;

    [SerializeField]
    public GameObject panel_createquestion;

    [SerializeField]
    public InputField questionlistname;

    public GameObject NewQuestionListWindow;

    public GameObject QuestionListUIItem;

    public GameObject QuestionUIItem;

    public Transform Panel;

    string tempstr = null;

    List<GameObject> list = new List<GameObject>();
    List<GameObject> q_list = new List<GameObject>();

    private void OnEnable()
    {
        tempstr = null;
        FetchQuestionList();
    }

    private void OnDisable()
    {
        ClearQuestionListObjects();
    }

    private void Update()
    {
        // To address bug of Instantiate not working in delegates...
        if (tempstr != null)
        {
            SpawnQuestionList(tempstr);
            tempstr = null;
        }
    }

    void ClearQuestionListObjects()
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }
        list.Clear();
    }

    public void Btn_ShowNewQuestionListWindow(bool flag)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        NewQuestionListWindow.SetActive(flag);
    }

    public void Btn_CreateQuestion(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        panel_createquestion.SetActive(!panel_createquestion.activeSelf); // show create question

    }

    public void BtnCreateNewQuestion() {

        Question q = new Question();
        q.id_owner = ProfileMgr.Instance.localProfile.id_account;
        q.question = questionField.text;
        q.answer1 = answer1Field.text;
        q.answer2 = answer2Field.text;
        q.answer3 = answer3Field.text;
        q.answer4 = answer4Field.text;
        q.correctanswer = correctAnswerDD.value;

        if (q.question == "" || q.answer1 == "" || q.answer2 == "" || q.answer3 == "" || q.answer4 == ""){ 
            NotificationMgr.Instance.Notify("Question field and answer fields cannot be empty.");
            return;
        }

        if (q.correctanswer == 0)
        {
            NotificationMgr.Instance.Notify("Please select the correct answer.");
            return;
        }

        NotificationMgr.Instance.NotifyLoad("Creating question.");
        
        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_QUESTIONS + "/", q,
        delegate (string key)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify("Question created.", 
                delegate ()
            {
                panel_createquestion.SetActive(false);
                NotificationMgr.Instance.Notify("The key is :" + key);
            });
            
        },
        delegate (string failmsg2) // failed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg2);
        });
    }

    public void ViewQuestionList(QuestionList ql)
    {
        List<string> questions_keys = ql.list;
        
        if (questions_keys.Count > 0){
            NotificationMgr.Instance.NotifyLoad("Fetching Questions in " + ql.name);
            string tempresult = null;
            foreach (string key in questions_keys)
            {

                DatabaseMgr.Instance.DBFetch(DBQueryConstants.QUERY_QUESTIONLISTS,
                    delegate(string result)
                {
                    NotificationMgr.Instance.StopLoad();
                    tempresult = result;
                },
                    delegate (string failmsg)
                    {
                        NotificationMgr.Instance.StopLoad();
                        NotificationMgr.Instance.Notify(failmsg);
                        tempresult = null;
                    });
                if (tempresult != null)
                {
                    Dictionary<string, object> results = Json.Deserialize(tempresult) as Dictionary<string, object>;
                    string questiondata = Json.Serialize(results.Values.First());
                    Question question = JsonUtility.FromJson<Question>(questiondata);

                    GameObject obj = Instantiate(QuestionUIItem, Panel.transform.position, Quaternion.identity);
                    QuestionUIItem script = obj.GetComponent<QuestionUIItem>();
                    script.transform.SetParent(Panel);
                    script.transform.localScale = QuestionUIItem.transform.localScale;
                    script.Init(question.correctanswer.ToString("D"), question.question, question, this);

                    q_list.Add(obj);
                }
                
            }
            NotificationMgr.Instance.StopLoad();
        }
    }

    public void FetchQuestionList()
    {
        NotificationMgr.Instance.NotifyLoad("Fetching Question List");
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_QUESTIONLISTS,
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
            tempstr = null;
        });
    }

    public void SpawnQuestionList(string result)
    {
        Debug.Log(result);
        Dictionary<string, object> results = Json.Deserialize(result) as Dictionary<string, object>;
        Debug.Log(results.Count);

        // Todo - get name and other details of session

        foreach (KeyValuePair<string, object> pair in results)
        {
            string questionlistdata = Json.Serialize(pair.Value);
            QuestionList questionlist = JsonUtility.FromJson<QuestionList>(questionlistdata);

            GameObject obj = Instantiate(QuestionListUIItem, Panel.transform.position, Quaternion.identity);

            QuestionListUIItem script = obj.GetComponent<QuestionListUIItem>();
            script.transform.SetParent(Panel);
            script.transform.localScale = QuestionListUIItem.transform.localScale;
            script.Init(questionlist.size.ToString("D"), questionlist.name, questionlist, this);

            list.Add(obj);
        }
    }

    public void CreateQuestionList()
    {
        QuestionList ql = new QuestionList();
        ql.id_owner = ProfileMgr.Instance.localProfile.id_account;
        ql.name = questionlistname.text;
        ql.size = 0;
        // ql.list = new List<string>();
        // ql.list.Add('-1');

        if (ql.name == ""){
            NotificationMgr.Instance.Notify("Name of Question List cannot be empty.");
            return;
        }

        NotificationMgr.Instance.NotifyLoad("Creating Question List");

        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_QUESTIONLISTS + "/", ql,
            delegate (string key)
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify("Question List created.",
                    delegate ()
                    {
                        NewQuestionListWindow.SetActive(false);
                        //questionlistname.text = "";
                    });

            },
            delegate (string failmsg2) // failed
                    {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg2);
            });
    }
}
