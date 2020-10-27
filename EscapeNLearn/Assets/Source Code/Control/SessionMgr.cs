using System.Collections.Generic;
using UnityEngine;
using Facebook.MiniJSON;

public enum ReportType
{
    LEVEL,
    INDIVIDUAL,
    SESSION
}

/// <summary>
/// Session Subsystem Interface, a Control Class that handles all report related processes. 
/// </summary>
public class SessionMgr : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static SessionMgr Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // accessed by report screen
    public List<Report> SessionReports = new List<Report>();
    public Report LevelReport = new Report();
    public Session passedInSession = null;
    public string passedInSessionID;
    public ReportType CurrentReportType = ReportType.INDIVIDUAL;

    // accessed by game screen
    public List<Question> passedInQuestionList = new List<Question>();

    public Connection currentConnection = null;
    public string connectionID = null;
    public Challenge currChallenge = null;
    public string challengeID = null;

    /// <summary>
    /// Joins a session.
    /// </summary>
    public void JoinSession(Connection c, SimpleCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_CONNECTIONS, c,
        delegate (string key)
        {
            currentConnection = c;
            connectionID = key;
            successCallback?.Invoke();
        },
        delegate (string failmsg)
        {
            failCallback?.Invoke(failmsg);
        });
    }

    /// <summary>
    /// Leaves the current connected session.
    /// </summary>
    public void LeaveSession(SimpleCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_CONNECTIONS + "/" + connectionID, null,
          delegate ()
          {
              currentConnection = null;
              successCallback?.Invoke();
          },
          delegate (string fail)
          {
              failCallback?.Invoke(fail);
          });
    }

    /// <summary>
    /// Creates a new session.
    /// </summary>
    public void CreateSession(Session s, SimpleCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_SESSIONS + "/", s,
        delegate (string key)
        {
            successCallback?.Invoke();
        },
        delegate (string failmsg) // failed
        {
             failCallback?.Invoke(failmsg);
         });
    }

    /// <summary>
    /// Fetches a single session from the database and return the result in the callback.
    /// </summary>
    public void FetchSingleSession(string id, MessageCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBFetch(DBQueryConstants.QUERY_SESSIONS + "/" + id,
        delegate (string result)
        {
            successCallback?.Invoke(result);
        },
        delegate (string failmsg)
        {
            failCallback?.Invoke(failmsg);
        });
    }

    /// <summary>
    /// Fetches multiple sessions from the database and return the result in the callback.
    /// </summary>
    public void FetchSessions(MessageCallback successCallback, MessageCallback failCallback, string searchid = null, int max = 100)
    {
        if (searchid == null)
        {
            DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_SESSIONS,
            null, null,
            max,
              delegate (string result)
              {
                  successCallback?.Invoke(result);
              },
              delegate (string failmsg)
              {
                  failCallback?.Invoke(failmsg);
              });
        }
        else
        {
            DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_SESSIONS,
            nameof(Session.id_owner), searchid,
            max,
            delegate (string result)
            {
                successCallback?.Invoke(result);
            },
            delegate (string failmsg)
            {
                failCallback?.Invoke(failmsg);
            });
        }
    }

    /// <summary>
    /// Fetches all questions for the game level of the session.
    /// </summary>
    public void FetchQuestionsForGame(string sessionid, int game_level, SimpleCallback successCallback, MessageCallback failCallback)
    {
        passedInQuestionList.Clear();

        FetchSingleSession(sessionid,
         delegate (string result) // success
         {
             string questionListId = "";

             Session session = JsonUtility.FromJson<Session>(result);
             switch (game_level)
             {
                 case 1:
                     questionListId = session.id_l1queslist;
                     Debug.Log("level 1 qlist loaded");
                     break;
                 case 2:
                     questionListId = session.id_l2queslist;
                     Debug.Log("level 2 qlist loaded");
                     break;
                 case 3:
                     questionListId = session.id_l3queslist;
                     Debug.Log("level 3 qlist loaded");
                     break;
             }

             // with question list id, fetch question list
             DatabaseMgr.Instance.DBFetch(DBQueryConstants.QUERY_QUESTIONLISTS + "/" + questionListId,
             delegate (string qlistresult) // fetched question list
                {
                    QuestionList questionlist = JsonUtility.FromJson<QuestionList>(qlistresult);
                    List<string> questions_keys = questionlist.list;
                    Debug.Log(questionlist.name + ": " + questions_keys.Count + " questions");
                    foreach (string qkey in questions_keys)
                    {
                        DatabaseMgr.Instance.DBFetch(DBQueryConstants.QUERY_QUESTIONS + '/' + qkey,
                            delegate (string qresult)
                            {
                                Question q = JsonUtility.FromJson<Question>(qresult);
                                Debug.Log("Fetch qkey " + qkey + ": " + q.question);
                                passedInQuestionList.Add(q);
                                if (passedInQuestionList.Count == questions_keys.Count)
                                {
                                    successCallback?.Invoke();
                                }
                            });
                    }
                },
             delegate (string failmsg)
             {
                 failCallback?.Invoke(failmsg);
             });
         },
         delegate (string failmsg)
         {
             failCallback?.Invoke(failmsg);
         });
    }

    /// <summary>
    /// Loads all reports based on the individual.
    /// </summary>
    public void LoadIndividualSessionReports(SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        SessionReports.Clear();
        CurrentReportType = ReportType.INDIVIDUAL;

        // Fetch all session reports
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_REPORTS, nameof(Report.id_session), currentConnection.id_session, 100,
         delegate (string result)
         {
             Dictionary<string, object> dic = Json.Deserialize(result) as Dictionary<string, object>;
             foreach (KeyValuePair<string, object> pair in dic)
             {
                 Report r = JsonUtility.FromJson<Report>(Json.Serialize(pair.Value));

                 if (r != null)
                 {
                     if (r.id_player == currentConnection.id_player) // only add student's session reports
                     {
                         SessionReports.Add(r);
                     }
                 }
             }

             successCallback?.Invoke();
         },
         delegate (string failmsg)
         {
             failCallback?.Invoke(failmsg);
         });
    }

    /// <summary>
    /// Loads all reports based on the session.
    /// </summary>
    public void LoadAllSessionReports(string id_session, Session session, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        passedInSessionID = id_session;
        passedInSession = session;
        SessionReports.Clear();
        CurrentReportType = ReportType.SESSION;

        // Fetch all session reports
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_REPORTS, nameof(Report.id_session), id_session, 100,
         delegate (string result)
         {
             Dictionary<string, object> dic = Json.Deserialize(result) as Dictionary<string, object>;
             foreach (KeyValuePair<string, object> pair in dic)
             {
                 Report r = JsonUtility.FromJson<Report>(Json.Serialize(pair.Value));

                 if (r != null)
                 {
                     SessionReports.Add(r);
                 }
             }

             successCallback?.Invoke();
         },
         delegate (string failmsg)
         {
             failCallback?.Invoke(failmsg);
         });
    }

    /// <summary>
    /// Loads a single report.
    /// </summary>
    public void LoadLevelReport(Report report)
    {
        CurrentReportType = ReportType.LEVEL;
        LevelReport = report;
    }
}
