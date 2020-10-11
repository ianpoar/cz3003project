using UnityEngine;
using System.Collections.Generic;
using Facebook.MiniJSON;

/// <summary>
/// Profile Manager Subsystem Interface, a Control Class that handles all profile and session-connection related processes. 
/// </summary>
public class ProfileMgr : MonoBehaviour // Singleton class
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static ProfileMgr Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Public variables
    public Profile localProfile = new Profile();
    public Connection currentConnection = null;
    public string connectionID = null;

    /// <summary>
    /// Saves player profile to the database.
    /// </summary>
    public void SavePlayerProfile(SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        // save profile
        DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id,
        localProfile,
        delegate () // success
        {
            successCallback?.Invoke();
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.Notify(failmsg);
        });
    }

    /// <summary>
    /// Loads player profile and connection information from the database.
    /// </summary>
    public void LoadPlayerProfile(SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        DatabaseMgr.Instance.DBFetch(DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id,
        delegate (string result) // success
        {

            localProfile = JsonUtility.FromJson<Profile>(result);
            // load connections
            DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_CONNECTIONS,
           nameof(Connection.id_player), localProfile.id_player, 1,
            delegate (string result2)
            {
                Dictionary<string, object> dic = Json.Deserialize(result2) as Dictionary<string, object>;
                foreach (KeyValuePair<string, object> pair in dic)
                {
                    Connection c = JsonUtility.FromJson<Connection>(Json.Serialize(pair.Value));
                   
                    if (c != null)
                    {
                        Debug.Log("ConnectionID: " + pair.Key + ", SessionID: " + c.id_session + ", PlayerID: " + c.id_player + ", Level: " + c.level_cleared);
                        connectionID = pair.Key;
                        currentConnection = c;
                    }
                }

                successCallback?.Invoke();
            },
            delegate (string failmsg) // no connections
            {
                successCallback?.Invoke();
            });

        },
        delegate (string failmsg) // failed
        {
            failCallback?.Invoke(failmsg);
        });
    }
}
