using UnityEngine;
using System.Collections.Generic;
using Facebook.MiniJSON;

public class ProfileMgr : MonoBehaviour // Singleton class
{
    // Singleton implementation
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

    // Save player data to firebase db
    // Can be directly called from UI classes, pass in success and failure delegate methods to specify your desired action for each case
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

    // Load player data from firebase db
    // Can be directly called from UI classes, pass in success and failure delegate methods to specify your desired action for each case
    public void LoadPlayerProfile(SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        DatabaseMgr.Instance.DBFetch(DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id,
        delegate (string result) // success
        {

            localProfile = JsonUtility.FromJson<Profile>(result);
            // load connections
            DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_CONNECTIONS,
            "id_player", localProfile.id_account, 1,
            delegate (string result2)
            {
                Dictionary<string, object> dic = Json.Deserialize(result2) as Dictionary<string, object>;
                foreach (KeyValuePair<string, object> pair in dic)
                {
                    Connection c = JsonUtility.FromJson<Connection>(Json.Serialize(pair.Value));
                   
                    if (c != null)
                    {
                        Debug.Log("ConnectionID: " + pair.Key + ", SessionID: " + c.id_session + ", PlayerID: " + c.id_player);
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
