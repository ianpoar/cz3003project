using UnityEngine;
// Data saved/loaded from firebase db
public class Profile
{
    public string name = "Unknown";
    public string accountType = "Student";
    public int accountExp = 0;
    public int currency_normal = 0;
    public int currency_premium = 0;
    public string id_facebook = "Unknown";
    public string id_google = "Unknown";
    public string id_account = "Unknown";
}

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
            successCallback?.Invoke();
        },
        delegate (string failmsg) // failed
        {
            failCallback?.Invoke(failmsg);
        });
    }
}
