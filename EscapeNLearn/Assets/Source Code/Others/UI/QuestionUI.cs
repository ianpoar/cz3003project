using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Facebook.MiniJSON;
using System.Net.Mail;
using System.Linq;
using System;
using System.IO.Pipes;

/// <summary>
/// A UI class that provides question management functionalities, encapsulated in the menu scene in Unity.
/// </summary>
public class QuestionUI : MonoBehaviour
{
    [SerializeField]
    public InputField editquestionField;
    [SerializeField]
    public InputField editanswer1Field;
    [SerializeField]
    public InputField editanswer2Field;
    [SerializeField]
    public InputField editanswer3Field;
    [SerializeField]
    public InputField editanswer4Field;
    [SerializeField]
    public Dropdown editcorrectAnswerDD;
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

    public GameObject panel_createquestion;

    public GameObject btn_backbutton;

    public GameObject panel_editquestion;

    [SerializeField]
    public InputField questionlistname;

    [SerializeField]
    public Text title;

    public GameObject createquestionButton;

    public GameObject createquestionlistButton;

    [SerializeField]
    public Text createquestion_questionlistname;

    public GameObject NewQuestionListWindow;

    public GameObject QuestionListUIItem;

    public GameObject QuestionUIItem;

    public Transform Panel;

    QuestionList ql = null;
    string ql_key = null;
    Question q = null;
    string q_key = null;

    string tempstr = null;

    List<GameObject> list = new List<GameObject>();
    List<GameObject> q_list = new List<GameObject>();

    private void OnEnable()
    {
        SetupQuestionListView();
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

    private void SetupQuestionListView()
    {
        title.text = "Your Question List";
        this.ql = null;
        this.ql_key = null;
        ClearQuestionObjects();
        createquestionlistButton.SetActive(true);
        createquestionButton.SetActive(false);
        btn_backbutton.SetActive(false);
        tempstr = null;
        FetchQuestionList();
    }

    private void SetupDisplayQuestionListDetailView(QuestionList ql)
    {
        title.text = ql.name;
        btn_backbutton.SetActive(true);
        createquestionlistButton.SetActive(false);
        createquestionButton.SetActive(true);
        tempstr = null;
        ClearQuestionListObjects();
    }

    void ClearQuestionListObjects()
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }
        list.Clear();
    }

    void ClearQuestionObjects()
    {
        foreach (GameObject obj in q_list)
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
        questionField.text = "";
        answer1Field.text = "";
        answer2Field.text = "";
        answer3Field.text = "";
        answer4Field.text = "";
        correctAnswerDD.value = 0;

        if (this.ql != null)
        {
            createquestion_questionlistname.text = ql.name;
        }
        toggleCreateQuestionPanel();
    }

    public void Btn_CloseEdit(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        editquestionField.text = "";
        editanswer1Field.text = "";
        editanswer2Field.text = "";
        editanswer3Field.text = "";
        editanswer4Field.text = "";
        editcorrectAnswerDD.value = 0;

        toggleEditQuestionPanel();
    }

    private void toggleEditQuestionPanel()
    {
        bool state = !panel_editquestion.activeSelf;
        btn_backbutton.SetActive(!state);
        panel_editquestion.SetActive(state);
    }

    private void toggleCreateQuestionPanel()
    {
        bool state = !panel_createquestion.activeSelf;
        btn_backbutton.SetActive(!state);
        panel_createquestion.SetActive(state);
    }

    public void EditQuestion(Question q, string key)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        this.q = q;
        this.q_key = key;
        editquestionField.text = q.question;
        editanswer1Field.text = q.answer1;
        editanswer2Field.text = q.answer2;
        editanswer3Field.text = q.answer3;
        editanswer4Field.text = q.answer4;
        editcorrectAnswerDD.value = q.correctanswer;

        toggleEditQuestionPanel();
    }

    public void Btn_DeleteQuestion()
    {
        if (this.q_key != null)
        {
            DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_QUESTIONS + "/" + this.q_key, null,
            delegate ()
            {
                NotificationMgr.Instance.StopLoad();
                tempstr = null;
                NotificationMgr.Instance.Notify("Question deleted.",
                delegate ()
                {
                    panel_editquestion.SetActive(false);
                });

            },
            delegate (string failmsg2) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg2);
            });
            this.ql.list.Remove(this.q_key);
            this.ql.size -= 1;
            this.q = null;
            this.q_key = null;


            DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_QUESTIONLISTS + "/" + this.ql_key, this.ql,
            delegate ()
            {
                NotificationMgr.Instance.StopLoad();
                ClearQuestionObjects();
                ViewQuestionList(this.ql, this.ql_key);
            },
            delegate (string failmsg2) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg2);
                // Suppose to delete the question if unable to add it in
            });
        }
        toggleEditQuestionPanel();
    }

    public void Btn_SaveEditQuestion()
    {
        if (this.q != null)
        {
            Question q = new Question();
            q.id_owner = ProfileMgr.Instance.localProfile.id_account;
            q.question = editquestionField.text;
            q.answer1 = editanswer1Field.text;
            q.answer2 = editanswer2Field.text;
            q.answer3 = editanswer3Field.text;
            q.answer4 = editanswer4Field.text;
            q.correctanswer = editcorrectAnswerDD.value;

            // Maybe can compare the values in this.q and q before deciding to update db

            if (q.question == "" || q.answer1 == "" || q.answer2 == "" || q.answer3 == "" || q.answer4 == "")
            {
                NotificationMgr.Instance.Notify("Question field and answer fields cannot be empty.");
                return;
            }

            if (q.correctanswer == 0)
            {
                NotificationMgr.Instance.Notify("Please select the correct answer.");
                return;
            }

            NotificationMgr.Instance.NotifyLoad("Updating question.");

            DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_QUESTIONS + "/" + this.q_key, q,
            delegate ()
            {
                NotificationMgr.Instance.StopLoad();
                tempstr = null;
                NotificationMgr.Instance.Notify("Question updated.",
                delegate ()
                {
                    panel_editquestion.SetActive(false);
                    ClearQuestionObjects();
                    ViewQuestionList(this.ql, this.ql_key);
                });

            },
            delegate (string failmsg2) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg2);
            });
            this.q = null;
            this.q_key = null;
        }
        else
        {
            NotificationMgr.Instance.Notify("Please retry again. The server is busy.");
        }
        toggleEditQuestionPanel();
    }


    public void BtnBackToUI()
    {
        // May change if need be
        SetupQuestionListView();
    }

    private void updateQuestionListDB(string question_id)
    {
        if (question_id != null)
        {
            if (this.ql.list == null)
            {
                this.ql.list = new List<string>();
            }
            this.ql.list.Add(question_id);
            this.ql.size += 1;

            DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_QUESTIONLISTS + "/" + this.ql_key, this.ql,
            delegate ()
            {
                NotificationMgr.Instance.StopLoad();
                ClearQuestionObjects();
                ViewQuestionList(this.ql, this.ql_key);
            },
            delegate (string failmsg2) // failed
                {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg2);
                // Suppose to delete the question if unable to add it in
                });
        }
    }

    public void BtnCreateNewQuestion() {
        if (this.ql != null)
        {
            Question q = new Question();
            q.id_owner = ProfileMgr.Instance.localProfile.id_account;
            q.question = questionField.text;
            q.answer1 = answer1Field.text;
            q.answer2 = answer2Field.text;
            q.answer3 = answer3Field.text;
            q.answer4 = answer4Field.text;
            q.correctanswer = correctAnswerDD.value;
            
            
            if (q.question == "" || q.answer1 == "" || q.answer2 == "" || q.answer3 == "" || q.answer4 == "")
            {
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
                tempstr = null;
                NotificationMgr.Instance.Notify("Question created.",
                delegate ()
                {
                    panel_createquestion.SetActive(false);
                    updateQuestionListDB(key); ;
                });

            },
            delegate (string failmsg2) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg2);
            });
            
        }
        else
        {
            NotificationMgr.Instance.Notify("Please retry again. The server is busy.");
        }
        toggleCreateQuestionPanel();
    }

    public void ViewQuestionList(QuestionList ql, string key)
    {
        List<string> questions_keys = ql.list;
        SetupDisplayQuestionListDetailView(ql);
        this.ql = ql;
        this.ql_key = key;
        if (questions_keys.Count > 0){
            NotificationMgr.Instance.NotifyLoad("Fetching Questions in " + ql.name);
            
            foreach (string qkey in questions_keys)
            {

                DatabaseMgr.Instance.DBFetch(DBQueryConstants.QUERY_QUESTIONS + '/' + qkey,
                    delegate(string result)
                {
                    NotificationMgr.Instance.StopLoad();
                    SpawnQuestions(result, qkey);
                },
                    delegate (string failmsg)
                    {
                        NotificationMgr.Instance.StopLoad();
                        NotificationMgr.Instance.Notify(failmsg);
                    });

            }
            NotificationMgr.Instance.StopLoad();
        }
        else
        {
            NotificationMgr.Instance.Notify("There is no question in this list.");
        }
    }

    public void SpawnQuestions(string result, string key)
    {

        Question question = JsonUtility.FromJson<Question>(result);
        GameObject obj = Instantiate(QuestionUIItem, Panel.transform.position, Quaternion.identity);
        QuestionUIItem script = obj.GetComponent<QuestionUIItem>();
        script.transform.SetParent(Panel);
        script.transform.localScale = QuestionUIItem.transform.localScale;
        script.Init(key, question.correctanswer.ToString("D"), question.question, question, this);

        q_list.Add(obj);
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
            script.Init(pair.Key, questionlist.size.ToString("D"), questionlist.name, questionlist, this);

            list.Add(obj);
        }
    }

    public void CreateQuestionList()
    {
        QuestionList ql = new QuestionList();
        ql.id_owner = ProfileMgr.Instance.localProfile.id_account;
        ql.name = questionlistname.text;
        ql.size = 0;

        if (ql.name == ""){
            NotificationMgr.Instance.Notify("Name of Question List cannot be empty.");
            return;
        }

        NotificationMgr.Instance.NotifyLoad("Creating Question List");

        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_QUESTIONLISTS + "/", ql,
            delegate (string key)
            {
                NewQuestionListWindow.SetActive(false);
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify("Question List created.");
            },
            delegate (string failmsg2) // failed
                    {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg2);
            });
    }
}
