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
/// Session Manager Subsystem Interface, a Control Class that handles all report related processes. 
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
    public Connection passedInConnection = null;

    public ReportType CurrentReportType = ReportType.INDIVIDUAL;

    /// <summary>
    /// Loads all reports based on the individual.
    /// </summary>
    public void LoadIndividualSessionReports(Connection c, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        passedInConnection = c;
        SessionReports.Clear();
        CurrentReportType = ReportType.INDIVIDUAL;

        // Fetch all session reports
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_REPORTS, "id_session", c.id_session, 100,
         delegate (string result)
         {
             Dictionary<string, object> dic = Json.Deserialize(result) as Dictionary<string, object>;
             foreach (KeyValuePair<string, object> pair in dic)
             {
                 Report r = JsonUtility.FromJson<Report>(Json.Serialize(pair.Value));

                 if (r != null)
                 {
                    if (r.id_account == c.id_player) // only add student's session reports
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
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_REPORTS, "id_session", id_session, 100,
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
