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

    List<GameObject> list = new List<GameObject>();
    List<GameObject> q_list = new List<GameObject>();
    int questionsLoaded = 0;

    private void OnEnable()
    {
        SetupQuestionListView();
    }

    private void OnDisable()
    {
        ClearQuestionListObjects();
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
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        if (this.q_key != null)
        {
            NotificationMgr.Instance.NotifyLoad("Deleting Question");
            ProfileMgr.Instance.DeleteMyQuestion(this.q_key,
            delegate () // success
            {
                this.ql.list.Remove(this.q_key);
                this.ql.size -= 1;
                this.q = null;
                this.q_key = null;

                // update questionlist
                ProfileMgr.Instance.UpdateMyQuestionList(this.ql_key, this.ql,
                delegate ()
                {
                    ClearQuestionObjects();
                    NotificationMgr.Instance.StopLoad();
                    NotificationMgr.Instance.Notify("Question deleted and removed from Question List.",
                    delegate ()
                    {
                        panel_editquestion.SetActive(false);
                        ViewQuestionList(this.ql, this.ql_key, false);
                    });
                },
                delegate (string failmsg) // failed
                {
                    NotificationMgr.Instance.StopLoad();
                    NotificationMgr.Instance.Notify(failmsg);
                });
            },
            delegate (string failmsg) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });
        }
        toggleEditQuestionPanel();
    }

    public void Btn_SaveEditQuestion()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        if (this.q != null)
        {
            Question q = new Question();
            q.id_owner = ProfileMgr.Instance.localProfile.id_player;
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

            NotificationMgr.Instance.NotifyLoad("Updating Question");     
            ProfileMgr.Instance.UpdateMyQuestion(this.q_key, q,
            delegate ()
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify("Question updated.",
                delegate ()
                {
                    panel_editquestion.SetActive(false);
                    ClearQuestionObjects();
                    ViewQuestionList(this.ql, this.ql_key, false);
                });

            },
            delegate (string failmsg) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
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
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        // May change if need be
        SetupQuestionListView();
    }

    public void BtnCreateNewQuestion() {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        if (this.ql != null)
        {
            Question q = new Question();
            q.id_owner = ProfileMgr.Instance.localProfile.id_player;
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
            ProfileMgr.Instance.CreateNewQuestion(q,
            delegate (string key)
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify("Question created.",
                delegate ()
                {
                    panel_createquestion.SetActive(false);
                    updateQuestionListDB(key); ;
                });

            },
            delegate (string failmsg) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });
            
        }
        else
        {
            NotificationMgr.Instance.Notify("Please retry again. The server is busy.");
        }
        toggleCreateQuestionPanel();
    }

    public void ViewQuestionList(QuestionList ql, string key, bool playsound)
    {
        if (playsound)
            AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        questionsLoaded = 0;
        List<string> questions_keys = ql.list;
        SetupDisplayQuestionListDetailView(ql);
        this.ql = ql;
        this.ql_key = key;
        int count = questions_keys.Count;
        if (count > 0){
            NotificationMgr.Instance.NotifyLoad("Fetching questions in question list '" + this.ql.name + "'");
            foreach (string qkey in questions_keys)
            {
                DatabaseMgr.Instance.DBFetch(DBQueryConstants.QUERY_QUESTIONS + '/' + qkey,
                    delegate(string result)
                {
                    this.questionsLoaded++;
                    SpawnQuestions(result, qkey);

                    if (this.questionsLoaded == count)
                    {
                        NotificationMgr.Instance.StopLoad();
                    }
                });

            }
        }
        else
        {
            NotificationMgr.Instance.Notify("There are no questions in this list.");
        }
    }


    public void CreateQuestionList()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        QuestionList ql = new QuestionList();
        ql.id_owner = ProfileMgr.Instance.localProfile.id_player;
        ql.name = questionlistname.text;
        ql.size = 0;

        if (ql.name == "")
        {
            NotificationMgr.Instance.Notify("Name of Question List cannot be empty.");
            return;
        }

        NotificationMgr.Instance.NotifyLoad("Creating Question List");
        ProfileMgr.Instance.CreateNewQuestionList(ql,
            delegate ()
            {
                NewQuestionListWindow.SetActive(false);
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify("Question List created.");
            },
            delegate (string failmsg) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });
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
        FetchQuestionList();
    }

    private void SetupDisplayQuestionListDetailView(QuestionList ql)
    {
        title.text = ql.name;
        btn_backbutton.SetActive(true);
        createquestionlistButton.SetActive(false);
        createquestionButton.SetActive(true);
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

    private void SpawnQuestions(string result, string key)
    {
        Question question = JsonUtility.FromJson<Question>(result);
        GameObject obj = Instantiate(QuestionUIItem, Panel.transform.position, Quaternion.identity);
        QuestionUIItem script = obj.GetComponent<QuestionUIItem>();
        script.transform.SetParent(Panel);
        script.transform.localScale = QuestionUIItem.transform.localScale;
        script.Init(key, question.correctanswer.ToString("D"), question.question, question, this);

        q_list.Add(obj);
    }

    private void FetchQuestionList()
    {
        NotificationMgr.Instance.NotifyLoad("Fetching Question Lists");
        ProfileMgr.Instance.FetchMyQuestionLists(
        delegate (string result)
        {
            NotificationMgr.Instance.StopLoad();
            SpawnQuestionList(result);
        },
        delegate (string failmsg)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });
    }

    private void SpawnQuestionList(string result)
    {
        Dictionary<string, object> results = Json.Deserialize(result) as Dictionary<string, object>;

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

            NotificationMgr.Instance.NotifyLoad("Updating Question List");
            ProfileMgr.Instance.UpdateMyQuestionList(this.ql_key, this.ql,
            delegate ()
            {
                NotificationMgr.Instance.StopLoad();
                ClearQuestionObjects();
                ViewQuestionList(this.ql, this.ql_key, false);
            },
            delegate (string failmsg) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });
        }
    }
}
