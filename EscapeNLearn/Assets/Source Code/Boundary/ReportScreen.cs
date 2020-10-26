using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;


/// <summary>
/// UI Boundary Class for the Report scene, handles all UI-related events in the scene.
/// </summary>
public class ReportScreen : Screen
{
   public PieGraph ob;
   public Text txt_header;
   public Text txt_info;
   public Text txt_info2;
   public Text txt_correct;
   public Text txt_wrong;

   /// <summary>
    /// Start of the Report screen.
    /// </summary>
    protected override void Start()
   {
        AudioMgr.Instance.PlayBGM(AudioConstants.BGM_RESULT);
        txt_info.text = "";
        txt_info2.text = "";
        txt_correct.text= "";
        txt_wrong.text= "";

        if (SessionMgr.Instance.currentConnection == null)
          {
            Debug.Log("no connection detected");
            return;
          }

        switch (SessionMgr.Instance.CurrentReportType)
        {
            case ReportType.INDIVIDUAL:
                txt_header.text = "Student Report";
                GenerateIndividualReport();
                break;
            case ReportType.LEVEL:
                txt_header.text = "Level Report";
                GenerateLevelReport();
                break;
            case ReportType.SESSION:
                txt_header.text = "Session Report";
                GenerateSessionReport();
                break;
        }
   }

    /// <summary>
    /// A helper function that records wrongly answered questions.
    /// </summary>
    void AddWrongAnswer(Dictionary<string, int> dic, string question)
    {
        int output;
        if (dic.TryGetValue(question, out output))
        {
            output++;
            dic[question] = output;
        }
        else
        {
            output = 1;
            dic.Add(question, output);
        }
    }

    /// <summary>
    /// A method to generate and display the players's overall individual report for a session.
    /// </summary>
    public void GenerateIndividualReport()
    {
        // Gather data
        int totalAnsCount = 0;
        int totalWrongCount = 0;
        int totalRightCount = 0;
        int totalElapsedTime = 0;

        List<Report> SessionReports = SessionMgr.Instance.SessionReports;
        Connection c = SessionMgr.Instance.currentConnection;
        Dictionary<string, int> wrongAnswers = new Dictionary<string, int>(); // to keep track of wrong answers

        for (int i = 0; i < SessionReports.Count; i++) // for each report
        {
            Report r = SessionReports[i];
            List<Answer> answers = r.answers.list;
            totalAnsCount += answers.Count; // increment total answer count
            totalElapsedTime += r.time_elapsed;

            for (int j = 0; j < answers.Count; j++) // for each answer
            {
                Answer a = answers[j];
                if (a.answer != a.correctanswer) // answer is wrong
                {
                    totalWrongCount++;
                    AddWrongAnswer(wrongAnswers, a.question); // record
                }
                else
                {
                    totalRightCount++;

                }
            }
        }

        // Format display
        txt_info.text += "Player ID: " + c.id_player;
        txt_info.text += "\n\nSession Name: " + c.session_name;
        txt_info.text += "\n\nSession ID: " + c.id_session;
        txt_info.text += "\n\nLevel Cleared: " + c.level_cleared;
        txt_info.text += "\n\nTotal Playtime (s): " + totalElapsedTime;
        txt_info.text += "\n\nTotal Questions Answered: " + totalAnsCount;
        //txt_info.text += "\n\nTotal No. of Answered Questions:\n" + totalAnsCount + " (" + totalRightCount + " correct, " + totalWrongCount + " wrong)";
        txt_info2.text += "Top 3 Wrong Questions: ";
        txt_correct.text += "Correct Answers: "+totalRightCount;
        txt_wrong.text += "Wrong Answers: "+totalWrongCount;
        foreach (KeyValuePair<string, int> item in wrongAnswers.OrderByDescending(key => key.Value).Take(3))
        {
            txt_info2.text += "\n" + item.Key + " (Count: " + item.Value + ")";
        }
        ob.MakeGraph(totalAnsCount, totalWrongCount);
    }

    /// <summary>
    /// A method to generate and display the player's level report for a level that was just played.
    /// </summary>
    public void GenerateLevelReport()
    {
        // Gather Data
        Connection c = SessionMgr.Instance.currentConnection;
        Report r = SessionMgr.Instance.LevelReport;

        int correctAnswerCount = 0;
        int wrongAnswerCount = 0;

        Dictionary<string, int> wrongAnswers = new Dictionary<string, int>(); // to keep track of wrong answers
        List<Answer> answers = r.answers.list;

        for (int j = 0; j < answers.Count; j++) // for each answer
        {
            Answer a = answers[j];
            if (a.answer != a.correctanswer) // answer is wrong
            {
                AddWrongAnswer(wrongAnswers, a.question); // record
                wrongAnswerCount++;
            }
            else
            {
                correctAnswerCount++;
            }
        }

        // Format display
        txt_info.text += "Session Name: " + c.session_name;
        txt_info.text += "\n\nSession ID: " + c.id_session;
        txt_info.text += "\n\nLevel: " + r.game_level;
        txt_info.text += "\n\nTime Elapsed (s): " + r.time_elapsed;
        txt_info.text += "\n\nTotal Questions Answered: " + r.answer_count;
        //txt_info.text += "\n\nTotal Questions Answered: " + r.answer_count + " (" + correctAnswerCount + " correct, " + wrongAnswerCount + " wrong)";
        txt_info2.text += "Top 3 Wrong Questions: ";
        txt_correct.text +="Correct Answers: " + correctAnswerCount;
        txt_wrong.text +="Wrong Answers: "+wrongAnswerCount;

        foreach (KeyValuePair<string, int> item in wrongAnswers.OrderByDescending(key => key.Value).Take(3))
        {
            txt_info2.text += "\n" + item.Key + " (Count: " + item.Value + ")";
        }
        int totalCount = correctAnswerCount+wrongAnswerCount;
        ob.MakeGraph(totalCount, wrongAnswerCount);
    }

    /// <summary>
    /// A method used by instructors to generate and display an overall report of a session.
    /// </summary>
    void GenerateSessionReport()
    {
        // Gather data
        int totalAnsCount = 0;
        int totalWrongCount = 0;
        int totalRightCount = 0;

        List<string> uniquePlayers = new List<string>();
        List<Report> SessionReports = SessionMgr.Instance.SessionReports;
        Session s = SessionMgr.Instance.passedInSession;

        Dictionary<string, int> wrongAnswers = new Dictionary<string, int>(); // to keep track of wrong answers

        for (int i = 0; i < SessionReports.Count; i ++) // for reach report
        {
            Report r = SessionReports[i];
            List<Answer> answers = r.answers.list;
            totalAnsCount += answers.Count; // increment total answer count

            if (!uniquePlayers.Contains(r.id_player))
                uniquePlayers.Add(r.id_player);

            for (int j = 0; j < answers.Count; j++) // for each answer
            {
                Answer a = answers[j];
                if (a.answer != a.correctanswer) // answer is wrong
                {
                    totalWrongCount++;
                    AddWrongAnswer(wrongAnswers, a.question); // record
                }
                else
                {
                    totalRightCount++;
                }
            }
        }

        // Format display
        txt_info.text += "Session Name: " + s.session_name;
        txt_info.text += "\n\nSession ID: " + SessionMgr.Instance.passedInSessionID;
        txt_info.text += "\n\nTotal No. of Unique Players: " + uniquePlayers.Count;
        txt_info.text += "\n\nTotal Questions Answered: " + totalAnsCount;
        txt_info2.text += "Top 3 Wrong Questions: ";
        txt_correct.text +="Correct Answers: "+totalRightCount;
        txt_wrong.text +="Wrong Answers: "+totalWrongCount;

        foreach (KeyValuePair<string, int> item in wrongAnswers.OrderByDescending(key => key.Value).Take(3))
        {
            txt_info2.text += "\n" + item.Key + " (Count: " + item.Value + ")";
        }
        ob.MakeGraph(totalAnsCount, totalWrongCount);
    }

    /// <summary>
    /// A handler for when the back button is pressed, performs a transition back to the menu screen.
    /// </summary>
    public void Btn_BacktoMenu()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        TransitMgr.Instance.FadeToScene("Menu");
    }
}
