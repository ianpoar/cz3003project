using Firebase;
using Firebase.Database;
using UnityEngine;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

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
            Debug.Log("Firebase Init success");
        });

        // init facebook sdk
        FB.Init(delegate ()
        {
            Debug.Log("FB Init Success");
        });
    }

    // Private variables
    private FirebaseDatabase _database;
    private APILinker _apiLinker;

    // Public variables, can be accessed from UI classes
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

    // Logs out from firebase account, can be directly called from other classes
    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    // Call SNS api to perform SNS login and get credential. Can be directly called from other classes, pass in success and failure delegate methods to specify your desired action for each case
    public void SNSRequestCredential(string provider, CredentialCallback successCallback, MessageCallback failCallback, bool skipAuth = false)
    {
        Debug.Log("SNSRequestCredential");
        switch (provider)
        {
            case LoginTypeConstants.FACEBOOK:
                _apiLinker = new FBAPILinker();
                break;
            default:
                failCallback?.Invoke("Provider error: " + provider);
                return;
        }

        if (!skipAuth)
            _apiLinker.Authenticate(successCallback, failCallback);
    }

    // With SNS credential, login to firebase db. Can be directly called from other classes, pass in success and failure delegate methods to specify your desired action for each case
    public void SNSLoginWithCredential(Credential credential, SimpleCallback successCallback, MessageCallback failCallback)
    {
        StartCoroutine(Sequence_SNSLogin(credential, successCallback, failCallback));
    }

    // Can be directly called from other classes, pass in success and failure delegate methods to specify your desired action for each case
    public void EmailRegister(string email, string pw, SimpleCallback passcb, MessageCallback failcb)
    {
        StartCoroutine(Sequence_EmailRegister(email, pw, passcb, failcb));
    }

    // Can be directly called from other classes, pass in success and failure delegate methods to specify your desired action for each case
    public void EmailLogin(string email, string pw, SimpleCallback passcb, MessageCallback failcb)
    {
        StartCoroutine(Sequence_EmailLogin(email, pw, passcb, failcb));
    }

    // Fetch an entire document from db. Can be directly called from other classes, pass in success and failure delegate methods to specify your desired action for each case
    public void DBFetch(string query, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBFetch(query, successCallback, failCallback));
    }

    // Checks if document exists in db. Can be directly called from other classes, pass in success and failure delegate methods to specify your desired action for each case
    public void DBCheck(string query, string orderbyID, string id, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBCheck(query, orderbyID, id, successCallback, failCallback));
    }

    // Writes an entire document to db. Can be directly called from other classes, pass in success and failure delegate methods to specify your desired action for each case
    public void DBUpdate(string query, object data, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBUpdate( query, data, successCallback, failCallback));
    }

    // Writes only a specific value in a document to db. Can be directly called from other classes, pass in success and failure delegate methods to specify your desired action for each case
    public void DBLightUpdate(string query, string itemToWrite, object value, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBLightUpdate(query, itemToWrite, value, successCallback, failCallback));
    }

    // Can be directly called from other classes, pass in success and failure delegate methods to specify your desired action for each case
    public void FetchProfilePic(string id, SpriteCallback successCallback, MessageCallback failCallback)
    {
        if (_apiLinker == null) // check that api linker exists
            failCallback?.Invoke("apilinker is null");
        else
            _apiLinker.GetProfilePic(id, successCallback, failCallback);
    }

    // Private coroutines used in the above methods

    // For DBFetch
    private IEnumerator Sequence_DBFetch(string query, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        var task = _database.GetReference(query).GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Exception == null)
        {
            // success
            string result = task.Result.GetRawJsonValue();
            if (result != null && result != "{}")
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

    // For DBCheck
    private IEnumerator Sequence_DBCheck(string query, string itemToCheck, string value, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        var task = _database.GetReference(query).OrderByChild(itemToCheck).EqualTo(value).LimitToFirst(1).GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Exception == null)
        {
            // found
            string result = task.Result.GetRawJsonValue();
            if (result != null && result != "{}")
            {
                successCallback?.Invoke(result);
            }
            else
            {
                failCallback?.Invoke("Data exists.");
            }
        }
        else
        {
            // failed
            failCallback?.Invoke(task.Exception.GetBaseException().ToString());
        }
    }

    // For DBUpdate
    private IEnumerator Sequence_DBUpdate(string query, object data, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        var task = _database.GetReference(query).SetRawJsonValueAsync(JsonUtility.ToJson(data));
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

    // For DBLightUpdate
    private IEnumerator Sequence_DBLightUpdate(string query, string itemToWrite, object value, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        var task = _database.GetReference(query).Child(itemToWrite).SetValueAsync(value);
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

    // For SNSLoginWithCredential
    private IEnumerator Sequence_SNSLogin(Credential credential, SimpleCallback successCallback, MessageCallback failCallback)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var task = auth.SignInWithCredentialAsync(credential);
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.Exception == null) // success
        {
            successCallback?.Invoke();
        }
        else // failed
        {
            failCallback?.Invoke(task.Exception.GetBaseException().ToString());
        }
    }

    // For EmailRegister
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

    // For EmailLogin
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
}
