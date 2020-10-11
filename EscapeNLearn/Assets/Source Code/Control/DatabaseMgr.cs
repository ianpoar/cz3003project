using Firebase;
using Firebase.Database;
using UnityEngine;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

/// <summary>
/// Database Subsystem Interface, a Control Class that handles all communication with the database in the game, implemented with the data access object pattern in mind.
/// </summary>
public class DatabaseMgr : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
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
    public List<string> LoginTypes
    {
        get
        {
            List<string> list = new List<string>();
            foreach (IUserInfo info in FirebaseAuth.DefaultInstance.CurrentUser.ProviderData)
            {
                if (info.ProviderId != "firebase") // don't need to know this, since all accounts are firebase
                    list.Add(info.ProviderId);
            }
            return list;
        }
        private set { }
    }

    /// <summary>
    /// Logs out from the game.
    /// </summary>
    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    /// <summary>
    /// Links SNS credentials to account.
    /// </summary>
    public void LinkCredentials(Credential cred, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_LinkCredentials(cred, successCallback, failCallback));
    }

    /// <summary>
    /// Unlinks SNS credentials from account.
    /// </summary>
    public void UnlinkCredentials(string provider, SimpleCallback successCallback, MessageCallback failCallback)
    {
        StartCoroutine(Sequence_UnlinkCredentials(provider, successCallback, failCallback));
    }

    /// <summary>
    ///  Call SNS API to perform SNS login and get credentials.
    /// </summary>
    public void SNSRequestCredential(string provider, CredentialCallback successCallback, MessageCallback failCallback, bool skipAuth = false)
    {
        Debug.Log("SNSRequestCredential");
        switch (provider)
        {
            case LoginTypeConstants.FACEBOOK:
                _apiLinker = new FBAPILinker();
                break;
            case LoginTypeConstants.GOOGLE:
                _apiLinker = new GoogleAPILinker();
                break;
            default:
                failCallback?.Invoke("Provider error: " + provider);
                return;
        }

        if (!skipAuth)
            _apiLinker.Authenticate(successCallback, failCallback);
    }

    /// <summary>
    /// Perform account login with SNS credentials.
    /// </summary>
    public void SNSLoginWithCredential(Credential credential, SimpleCallback successCallback, MessageCallback failCallback)
    {
        StartCoroutine(Sequence_SNSLogin(credential, successCallback, failCallback));
    }

    /// <summary>
    /// Perform account registration via email.
    /// </summary>
    public void EmailRegister(string email, string pw, SimpleCallback passcb, MessageCallback failcb)
    {
        StartCoroutine(Sequence_EmailRegister(email, pw, passcb, failcb));
    }

    /// <summary>
    /// Perform account login via email.
    /// </summary>
    public void EmailLogin(string email, string pw, SimpleCallback passcb, MessageCallback failcb)
    {
        StartCoroutine(Sequence_EmailLogin(email, pw, passcb, failcb));
    }

    /// <summary>
    /// Fetch an entire document from the database.
    /// </summary>
    public void DBFetch(string query, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBFetch(query, successCallback, failCallback));
    }

    /// <summary>
    /// Fetch multiple documents from the database.
    /// </summary>
    /// <param name="targetKey"> Specify a key to only fetch documents with properties that match. Fetches everything if left null. </param>
    /// /// <param name="targetValue"> Specify the value to match with the target key. </param>
    public void DBFetchMulti(string query, string targetKey, string targetValue, int first, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBFetchMulti(query, targetKey, targetValue, first, successCallback, failCallback));
    }

    /// <summary>
    /// Updates an entire document to the database.
    /// </summary>
    public void DBUpdate(string query, object data, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBUpdate(query, data, successCallback, failCallback));
    }

    /// <summary>
    /// Pushes an entire document to the database, returning the generated key in the succsess callback.
    /// </summary>
    public void DBPush(string query, object data, MessageCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBPush(query, data, successCallback, failCallback));
    }

    /// <summary>
    /// Updates only a specific property in a document in the database.
    /// </summary>
    /// <param name="itemToWrite"> Specify a key for the item to be written to. </param>
    /// /// <param name="value"> Specify the item to be writen. </param>
    public void DBLightUpdate(string query, string itemToWrite, object value, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        StartCoroutine(Sequence_DBLightUpdate(query, itemToWrite, value, successCallback, failCallback));
    }

    /// <summary>
    /// Fetch a profile picture.
    /// </summary>
    public void FetchProfilePic(string id, SpriteCallback successCallback, MessageCallback failCallback)
    {
        if (_apiLinker == null) // check that api linker exists
            failCallback?.Invoke("apilinker is null");
        else
            _apiLinker.GetProfilePic(id, successCallback, failCallback);
    }

    // Private coroutines used in the above methods

    /// <summary>
    /// Coroutine for DBFetch.
    /// </summary>
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

    /// <summary>
    /// Coroutine for DBFetchMulti.
    /// </summary>
    private IEnumerator Sequence_DBFetchMulti(string query, string orderbyID, string id, int first, MessageCallback successCallback, MessageCallback failCallback)
    {
        if (orderbyID == null || id == null) // fetch all
        {
            var task = FirebaseDatabase.DefaultInstance
                .GetReference(query).LimitToFirst(first)
                .GetValueAsync();
            yield return new WaitUntil(predicate: () => task.IsCompleted);
            if (task.IsFaulted)
                    {
                        failCallback?.Invoke(task.Exception.GetBaseException().ToString());
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        string result = snapshot.GetRawJsonValue();
                        if (result == null || result == "{}")
                        {
                            failCallback?.Invoke("No data found.");
                        }
                        else
                        {
                            successCallback?.Invoke(result);
                        }
                    }
        }
        else
        {
            var task = FirebaseDatabase.DefaultInstance
                .GetReference(query).OrderByChild(orderbyID).EqualTo(id).LimitToFirst(first)
                .GetValueAsync();
            yield return new WaitUntil(predicate: () => task.IsCompleted);
                    if (task.IsFaulted)
                    {
                        failCallback?.Invoke(task.Exception.GetBaseException().ToString());
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        string result = snapshot.GetRawJsonValue();
                        if (result == null || result == "{}")
                        {
                            failCallback?.Invoke("No data found.");
                        }
                        else
                        {
                            successCallback?.Invoke(result);
                        }
                    }
        }
    }

    /// <summary>
    /// Coroutine for DBUpdate.
    /// </summary>
    private IEnumerator Sequence_DBUpdate(string query, object data, SimpleCallback successCallback = null, MessageCallback failCallback = null)
    {
        if (data != null)
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

        else
        {
            var task = _database.GetReference(query).RemoveValueAsync();
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

    }

    /// <summary>
    /// Coroutine for DBLightUpdate.
    /// </summary>
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

    /// <summary>
    /// Coroutine for SNSLoginWithCredential.
    /// </summary>
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

    /// <summary>
    /// Coroutine for EmailRegister.
    /// </summary>
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

    /// <summary>
    /// Coroutine for EmailLogin.
    /// </summary>
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

    /// <summary>
    /// Coroutine for DBPush.
    /// </summary>
    private IEnumerator Sequence_DBPush(string query, object data, MessageCallback successCallback, MessageCallback failCallback)
    {
        string key = FirebaseDatabase.DefaultInstance
           .GetReference(query).Push().Key;

        var task = FirebaseDatabase.DefaultInstance
           .GetReference(query + "/" + key).SetRawJsonValueAsync(JsonUtility.ToJson(data));
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.IsFaulted)
               {
                   failCallback?.Invoke(task.Exception.GetBaseException().ToString());
               }
               else if (task.IsCompleted)
               {
                   if (task.Exception == null)
                   {
                       successCallback?.Invoke(key);
                   }
                   else
                   {
                       failCallback?.Invoke(task.Exception.GetBaseException().ToString());
                   }
               }
    }

    /// <summary>
    /// Coroutine for LinkCredentials.
    /// </summary>
    private IEnumerator Sequence_LinkCredentials(Credential cred, SimpleCallback successCallback, MessageCallback failCallback)
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser old_user = auth.CurrentUser;
        // auth.CurrentUser.UnlinkAsync("facebook.com");
        var task = auth.CurrentUser.LinkWithCredentialAsync(cred);
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.IsCanceled)
        {
            Debug.LogError("LinkWithCredentialAsync was canceled.");
            failCallback?.Invoke("Link cancelled");
        }
        else if (task.IsFaulted)
        {
            Debug.LogError("LinkWithCredentialAsync encountered an error: " + task.Exception);
            failCallback?.Invoke("Link unsuccsessful, error:" + task.Exception.ToString());
        }
        else
        {
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            successCallback?.Invoke();
        }
        // link here
    }

    /// <summary>
    /// Coroutine for UnLinkCredentials.
    /// </summary>
    private IEnumerator Sequence_UnlinkCredentials(string provider, SimpleCallback successCallback, MessageCallback failCallback)
    {
        var task = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UnlinkAsync(provider);
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.IsCanceled)
        {
            failCallback?.Invoke("Cancelled");
        }
        else if (task.IsFaulted)
        {
            failCallback?.Invoke(task.Exception.ToString());
        }
        else if (task.IsCompleted)
        {
            successCallback?.Invoke();
        }
    }
}
