using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

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
            });
            
        },
        delegate (string failmsg2) // failed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg2);
        });
    }

    public void btn_exit()
    {

    }
       /*public Text Txt_SessionID;
       // Start is called before the first frame update
       private void OnEnable()
       {
           Txt_SessionID.text = ProfileMgr.Instance.currentConnection.id_session;
       }

       public void Btn_GenerateNewQuestion()
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
       }*/
}
