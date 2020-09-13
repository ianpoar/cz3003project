using Firebase;
using Firebase.Database;
using UnityEngine;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;

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
        // init firebase sdk
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(continuationAction: task =>
        {
            _database = FirebaseDatabase.DefaultInstance;
            Debug.Log("DatabaseMgr: Firebase init success");
        });

        // init LoginAPIs
        _facebookapi.Initialise();
    }
    // Private variables
    private FirebaseDatabase _database;
    private LoginAPI _currentapi;
    private FacebookLoginAPI _facebookapi = new FacebookLoginAPI();
    private bool lastDBFetchSuccess = false;

    // Public variables
    public string Id { get { return FirebaseAuth.DefaultInstance.CurrentUser.UserId; } private set { } }
    public bool IsEmailVerified { get { return FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified; } private set { } }
    public string Email { get { return FirebaseAuth.DefaultInstance.CurrentUser.Email; } private set { } }
    public bool IsLoggedIn { get { return (FirebaseAuth.DefaultInstance.CurrentUser != null); } private set { } }

    public List <string> LoginTypes {
        get
        {
                List<string> list = new List<string>();
                foreach (IUserInfo info in FirebaseAuth.DefaultInstance.CurrentUser.ProviderData)
                {
                    list.Add(info.ProviderId);
                }
                return list;
        }
        private set { } }

    public bool LastFBFetchSuccess { get { return lastDBFetchSuccess; } private set { } }


    private string SetPath(DBQueryType type)
    {
        switch (type)
        {
            case DBQueryType.Profile:
                return DBConstants.PATH_PROFILE;
            case DBQueryType.Session:
                return DBConstants.PATH_SESSION;
            default:
                return "unknown/";
        }
    }
    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    public void FacebookLogin(SimpleCallback successCallback, MessageCallback failCallback)
    {
        _currentapi = _facebookapi;
        _currentapi.Authenticate(successCallback, failCallback);
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
            successCallback?.Invoke();
        }
        else
        {
            failCallback?.Invoke(task.Exception.GetBaseException().ToString());
        }
    }

    public void DBFetch(DBQueryType type, string query, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBFetch(type, query, successCallback, failCallback));
    }

    public void DBUpdate(DBQueryType type, string query, object data, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBUpdate(type, query, data, successCallback, failCallback));
    }

    private IEnumerator Sequence_DBFetch(DBQueryType type, string query, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        string path = SetPath(type);
        var task = _database.GetReference(path + query).GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Exception == null)
        {
            // success
            string result = task.Result.GetRawJsonValue();
            if (result != null)
            {
                if (!lastDBFetchSuccess)
                    lastDBFetchSuccess = true;

                successCallback?.Invoke(result);
            }
            else
            {
                if (lastDBFetchSuccess)
                    lastDBFetchSuccess = false;

                failCallback?.Invoke("Failed to fetch data.");
            }
        }
        else
        {
            // failed
            failCallback?.Invoke(task.Exception.GetBaseException().ToString());
        }
    }

    private IEnumerator Sequence_DBUpdate(DBQueryType type, string query, object data, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        string path = SetPath(type);

        var task = _database.GetReference(path + query).SetRawJsonValueAsync(JsonUtility.ToJson(data));
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

    public void SNSAuth(Credential credential, SimpleCallback successCallback, MessageCallback failCallback, string snsName)
    {
        StartCoroutine(Sequence_SNSLogin(credential, successCallback, failCallback, snsName));
    }

    private IEnumerator Sequence_SNSLogin(Credential credential, SimpleCallback successCallback, MessageCallback failCallback, string snsName)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var task = auth.SignInWithCredentialAsync(credential);
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
}
