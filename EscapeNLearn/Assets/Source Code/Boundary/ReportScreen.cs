using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// UI Boundary Class for the Report scene.
/// </summary>
public class ReportScreen : Screen
{
   public Text txt_header;
   public Text txt_info;
    public Text txt_info2;
   protected override void Start()
   {
        AudioMgr.Instance.PlayBGM(AudioConstants.BGM_RESULT);

        txt_info.text = "";
        txt_info2.text = "";
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

    // helper function
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

    void GenerateIndividualReport()
    {
        // Gather data
        int totalAnsCount = 0;
        int totalWrongCount = 0;
        int totalRightCount = 0;
        int totalElapsedTime = 0;

        List<Report> SessionReports = SessionMgr.Instance.SessionReports;
        Connection c = SessionMgr.Instance.passedInConnection;

        Dictionary<string, int> wrongAnswers = new Dictionary<string, int>(); // to keep track of wrong answers

        for (int i = 0; i < SessionReports.Count; i++) // for reach report
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
        txt_info.text += "\n\nSession Name:\n" + c.session_name;
        txt_info.text += "\n\nSession ID:\n" + c.id_session;
        txt_info.text += "\n\nHighest Game Level Cleared: " + c.level_cleared;
        txt_info.text += "\n\nTotal Playtime (s): " + totalElapsedTime;
        txt_info.text += "\n\nTotal No. of Answered Questions:\n" + totalAnsCount + " (" + totalRightCount + " correct, " + totalWrongCount + " wrong)";
        txt_info2.text += "Top 5 Wronged Questions:";

        foreach (KeyValuePair<string, int> item in wrongAnswers.OrderByDescending(key => key.Value).Take(5))
        {
            txt_info2.text += "\n" + item.Key + " (" + item.Value + " Times)";
        }
    }
    
    void GenerateLevelReport()
    {
        // Gather Data
        Connection c = ProfileMgr.Instance.currentConnection;
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
        txt_info.text += "Session Name:\n" + c.session_name;
        txt_info.text += "\n\nSession ID:\n" + c.id_session;
        txt_info.text += "\n\nLevel: " + r.game_level;
        txt_info.text += "\n\nTime Elapsed (s): " + r.time_elapsed;
        txt_info.text += "\n\nTotal No. of Answered Questions:\n" + r.answer_count + " (" + correctAnswerCount + " correct, " + wrongAnswerCount + " wrong)";
        txt_info2.text += "Top 3 Wronged Questions:";

        foreach (KeyValuePair<string, int> item in wrongAnswers.OrderByDescending(key => key.Value).Take(3))
        {
            txt_info2.text += "\n" + item.Key + " (" + item.Value + " Times)";
        }
    }

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
        txt_info.text += "Session Name:\n" + s.session_name;
        txt_info.text += "\n\nSession ID:\n" + SessionMgr.Instance.passedInSessionID;
        txt_info.text += "\n\nTotal No. of Unique Players: " + uniquePlayers.Count;

        txt_info.text += "\n\nTotal No. of Answered Questions:\n" + totalAnsCount + " (" + totalRightCount + " correct, " + totalWrongCount + " wrong)";
        txt_info2.text += "Top 10 Wronged Questions:";

        foreach (KeyValuePair<string, int> item in wrongAnswers.OrderByDescending(key => key.Value).Take(10))
        {
            txt_info2.text += "\n" + item.Key + " (" + item.Value + " Times)";
        }
    }

    public void Btn_BacktoMenu()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        TransitMgr.Instance.FadeToScene("Menu");
    }
}
