using Firebase;
using Firebase.Database;
using UnityEngine;
using Firebase.Auth;
using System.Collections;

public enum DBQueryType
{
    Profile,
    Session
}

public class DatabaseMgr : MonoBehaviour
{
    // Singleton implementation
    public static DatabaseMgr Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(continuationAction: task =>
        {
            _database = FirebaseDatabase.DefaultInstance;
            Debug.Log("DatabaseMgr: Firebase init success");
        });
    }
    // Private variables
    private FirebaseDatabase _database;
    private const string PATH_PROFILE = "profiles/";
    private const string PATH_SESSION = "sessions/";

    // Public variables
    public string Email { get { return ((FirebaseAuth.DefaultInstance.CurrentUser != null) && FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified) ? FirebaseAuth.DefaultInstance.CurrentUser.Email : "None"; } private set { } }
    public string Id { get { return FirebaseAuth.DefaultInstance.CurrentUser != null ? FirebaseAuth.DefaultInstance.CurrentUser.UserId : "None"; } private set { } }
    public bool IsEmailVerified { get { return (FirebaseAuth.DefaultInstance.CurrentUser != null && FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified); } private set { } }
    public bool IsSignedIn      { get { return (FirebaseAuth.DefaultInstance.CurrentUser != null); } private set{}}

    private string SetPath(DBQueryType type)
    {
        switch (type)
        {
            case DBQueryType.Profile:
                return PATH_PROFILE;
            case DBQueryType.Session:
                return PATH_SESSION;
            default:
                return "unknown/";
        }
    }
    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    public void EmailRegister(string email, string pw, SimpleCallback passcb, MessageCallback failcb)
    {
        StartCoroutine(Sequence_EmailRegister(email, pw, passcb, failcb));
    }

    public void EmailLogin(string email, string pw, SimpleCallback passcb, MessageCallback failcb)
    {
        StartCoroutine(Sequence_EmailLogin(email, pw, passcb, failcb));
    }

    private IEnumerator Sequence_EmailRegister(string email, string pw, SimpleCallback successCallback, MessageCallback failCallback)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var task = auth.CreateUserWithEmailAndPasswordAsync(email, pw);
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.Exception == null)
        {
            // success
            // send verification email
            var task2 = auth.CurrentUser.SendEmailVerificationAsync();
            yield return new WaitUntil(predicate: () => task2.IsCompleted);
            successCallback?.Invoke();
        }
        else
        {
            failCallback?.Invoke(task.Exception.GetBaseException().ToString());
        }
    }

    private IEnumerator Sequence_EmailLogin(string email, string pw, SimpleCallback successCallback, MessageCallback failCallback)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var task = auth.SignInWithEmailAndPasswordAsync(email, pw);
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.Exception == null)
        {
            // success
            successCallback?.Invoke();
        }
        else
        {
            failCallback?.Invoke(task.Exception.GetBaseException().ToString());
        }
    }

    public void DBFetch(DBQueryType type, string id, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBFetch(type, id, successCallback, failCallback));
    }

    public void DBUpdate(DBQueryType type, string id, object data, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBUpdate(type, id, data, successCallback, failCallback));
    }

    private IEnumerator Sequence_DBFetch(DBQueryType type, string id, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        string path = SetPath(type);
        var task = _database.GetReference(path + id).GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Exception == null)
        {
            // success
            string result = task.Result.GetRawJsonValue();
            if (result != null)
            {
                successCallback?.Invoke(result);
            }
            else
            {
                failCallback?.Invoke("Failed to fetch data.");
            }
        }
        else
        {
            // failed
            failCallback?.Invoke(task.Exception.GetBaseException().ToString());
        }
    }

    private IEnumerator Sequence_DBUpdate(DBQueryType type, string id, object data, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        string path = SetPath(type);

        var task = _database.GetReference(path + id).SetRawJsonValueAsync(JsonUtility.ToJson(data));
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.Exception == null)
        {
            // success
            successCallback?.Invoke();
        }
        else
        {
            // failed
            failCallback?.Invoke(task.Exception.GetBaseException().ToString());
        }
    }

    public void SavePlayerProfile(SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        // save profile
        DatabaseMgr.Instance.DBUpdate(DBQueryType.Profile, DatabaseMgr.Instance.Id,
        ProfileMgr.Instance.localProfile,
        delegate () // success
        {
            successCallback?.Invoke();
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.Notify(failmsg);
        });
                            
    }

    public void LoadPlayerProfile(SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        DatabaseMgr.Instance.DBFetch(DBQueryType.Profile, DatabaseMgr.Instance.Id,
        delegate (string result) // success
        {
            ProfileMgr.Instance.localProfile = JsonUtility.FromJson<Profile>(result);
            successCallback?.Invoke();
        },
        delegate (string failmsg) // failed
        {
            failCallback?.Invoke(failmsg);
        });
    }
}
