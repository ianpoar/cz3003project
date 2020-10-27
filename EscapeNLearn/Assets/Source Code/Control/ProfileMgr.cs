using UnityEngine;
using System.Collections.Generic;
using Facebook.MiniJSON;

/// <summary>
/// Profile Subsystem Interface, a Control Class that handles all profile and session-connection related processes. 
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
                        SessionMgr.Instance.connectionID = pair.Key;
                        SessionMgr.Instance.currentConnection = c;
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

    /// <summary>
    /// Fetches the player (instructor)'s question lists from the database and returns the result in a callback.
    /// </summary>
    public void FetchMyQuestionLists(MessageCallback successCallback, MessageCallback failCallback, int max = 100)
    {
        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_QUESTIONLISTS,
                nameof(QuestionList.id_owner),
                localProfile.id_player,
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

    /// <summary>
    /// Creates a new question list for the player (instructor).
    /// </summary>
    public void CreateNewQuestionList(QuestionList ql, SimpleCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_QUESTIONLISTS + "/", ql,
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
    /// Updates an existing question list belonging to the player (instructor).
    /// </summary>
    public void UpdateMyQuestionList(string qlkey, QuestionList ql, SimpleCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_QUESTIONLISTS + "/" + qlkey, ql,
           delegate ()
           {
               successCallback?.Invoke();
           },
           delegate (string failmsg) // failed
            {
                failCallback?.Invoke(failmsg);
            });
    }

    /// <summary>
    /// Creates a new question for the player (instructor) and returns its key in a callback.
    /// </summary>
    public void CreateNewQuestion(Question q, MessageCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_QUESTIONS + "/", q,
           delegate (string key)
           {
               successCallback?.Invoke(key);
           },
           delegate (string failmsg) // failed
            {
                failCallback?.Invoke(failmsg);
            });
    }

    /// <summary>
    /// Updates an existing question belonging to the player (instructor).
    /// </summary>
    public void UpdateMyQuestion(string qkey, Question q, SimpleCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_QUESTIONS + "/" + qkey, q,
            delegate ()
            {
                successCallback?.Invoke();

            },
            delegate (string failmsg) // failed
            {
                failCallback?.Invoke(failmsg);
            });
    }

    /// <summary>
    /// Deletes an existing question belonging to the player (instructor).
    /// </summary>
    public void DeleteMyQuestion(string qkey, SimpleCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_QUESTIONS + "/" + qkey, null,
        delegate ()
        {
            successCallback?.Invoke();
        },
        delegate (string failmsg) // failed
        {
            failCallback?.Invoke(failmsg);
        });
    }

    /// <summary>
    /// Fetches multiple sessions from the database and return the result in the callback.
    /// </summary>
    public void FetchProfiles(MessageCallback successCallback, MessageCallback failCallback, string searchid = null, int max = 100)
    {
        if (searchid == null)
        {
            DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_PROFILES,
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
            DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_PROFILES,
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
    public void FetchChallenges(MessageCallback successCallback, MessageCallback failCallback, int max = 100)
    {

        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_CHALLENGES,
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
}
