using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Subsystem Interface, a Control Class that handles all gameplay related processes.
/// </summary>
public abstract class GameMgr : MonoBehaviour
{
    // reference
    public Color AddHPColor;
    public Color ReduceHpColor;
    public GameObject spawnTextPrefab;

    // protected, accessed by child classes
    protected int score = 0;
    protected int PlayerMaxHP = 100;
    protected int PlayerHP = 100;
    protected GameScreen ScreenRef;
    protected int GameLevel = 0; // change in derived classes

    // private
    private int secondsElapsed = 0;
    private bool IsPaused = false;
    private List<Question> QuestionList = new List<Question>();
    private List<int> ShownQuestions = new List<int>();
    private int currQuesIterator = 0;
    string currNPCName;
    private int currNoOfQues = 0;
    private Report report = new Report();

    /// <summary>
    /// Starts the game.
    /// </summary>
    public virtual void StartGame(GameScreen reference)
    {
        ScreenRef = reference;

        QuestionList.Clear();
        foreach (Question q in SessionMgr.Instance.passedInQuestionList)
        {
            QuestionList.Add(q);
        }

        // init report
        report.game_level = GameLevel;
        report.id_session = SessionMgr.Instance.currentConnection.id_session;
        report.id_player = ProfileMgr.Instance.localProfile.id_player;

        // start timer
        StartCoroutine(Timer());

    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame(bool flag)
    {
        IsPaused = flag;
    }

    /// <summary>
    /// Displays questions in-game.
    /// </summary>
    protected void ShowQuestions(string npcname, int listnumber, int numberOfQuestions)
    {
        // randomly sort list
        for (int i = 0; i < QuestionList.Count; i++)
        {
            Question temp = QuestionList[i];
            int randomIndex = Random.Range(i, QuestionList.Count);
            QuestionList[i] = QuestionList[randomIndex];
            QuestionList[randomIndex] = temp;
        }

        currQuesIterator = 0;
        currNPCName = npcname;
        currNoOfQues = numberOfQuestions;

        if (QuestionList == null)
        {
            Debug.Log("null list");
            return;
        }

        Question ques = QuestionList[currQuesIterator];
        if (ques != null)
        {
            ScreenRef.ShowNPCQuestion(currNPCName, ques);
        }
        else
        {
            Debug.Log("null ques");
        }
    }

    /// <summary>
    /// A handler for when all in-game questions have been completed.
    /// </summary>
    protected virtual void QuestionsComplete()
    {
        ScreenRef.Btn_CancelQuestion(false);
    }

    /// <summary>
    /// A handler for when the player answers a question.
    /// </summary>
    public void AnswerQuestion(int choice)
    {
        Question ques = QuestionList[currQuesIterator];

        // Save answered question to report
        if (ques != null)
        {
            Answer a = new Answer();
            a.question = ques.question;
            a.answer = choice;
            a.answer1 = ques.answer1;
            a.answer2 = ques.answer2;
            a.answer3 = ques.answer3;
            a.answer4 = ques.answer4;
            a.correctanswer = ques.correctanswer;
            report.answers.list.Add(a);
            report.answer_count++;
        }

        if (choice == ques.correctanswer)
        {
            currQuesIterator++;
            if (currQuesIterator < currNoOfQues && currQuesIterator < QuestionList.Count)
            {
                ques = QuestionList[currQuesIterator];
                if (ques != null)
                {
                    ScreenRef.ShowNPCQuestion(currNPCName, ques);
                }
                else
                {
                    // Ended
                    QuestionsComplete();
                }
            }
            else
            {
                // Ended
                QuestionsComplete();
            }
        }
        else
        {
            ScreenRef.ShowWrongAnswer();
        }
    }

    /// <summary>
    /// Coroutine for the in-game timer.
    /// </summary>
    private IEnumerator Timer()
    {
        bool time = true;
        while (time)
        {
            yield return new WaitForSeconds(1);
            secondsElapsed += 1;
            ScreenRef.SetTimer(secondsElapsed);
        }
    }

    /// <summary>
    /// A handler for when the game level is cleared.
    /// </summary>
    protected void GameClear()
    {
        PauseGame(true);
        StopAllCoroutines();

        // send report to database
        SendReportToDB();

        // update connection game level
        DatabaseMgr.Instance.DBLightUpdate(DBQueryConstants.QUERY_CONNECTIONS + "/" + SessionMgr.Instance.connectionID, nameof(Connection.level_cleared), GameLevel);
        SessionMgr.Instance.currentConnection.level_cleared = GameLevel;

        // show results screen
        ScreenRef.ShowResultsScreen(report);
    }

    /// <summary>
    /// A handler for when the player has failed the game level.
    /// </summary>
    protected void GameFailed()
    {
        PauseGame(true);
        StopAllCoroutines();

        // send report to database
        SendReportToDB();

        // show results screen
        ScreenRef.ShowGameFailed(report);
    }

    /// <summary>
    /// Sends a gameplay report to the database.
    /// </summary>
    public void SendReportToDB()
    {
        report.time_elapsed = secondsElapsed;
        report.score = score;
        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_REPORTS, report);
    }
}
