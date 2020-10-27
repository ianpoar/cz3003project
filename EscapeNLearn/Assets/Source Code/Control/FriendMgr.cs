using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Friends Subsystem Interface, a Control Class that handles all friends related processes.
/// </summary>
public class FriendMgr : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static FriendMgr Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    /// <summary>
    /// Fetches friends from the database and return the result in the callback.
    /// </summary>
    public void FetchFriends(MessageCallback successCallback, MessageCallback failCallback, string searchid = null, int max = 100)
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


    /// <summary>
    /// Fetches all challenges from the database and returns the result in the callback.
    /// </summary>
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

    /// <summary>
    /// Creates a new challenge between the current player and another.
    /// </summary>
    public void CreateChallenge(Challenge c, SimpleCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.DBPush(DBQueryConstants.QUERY_CHALLENGES + "/", c,
        delegate (string key)
        {
            successCallback?.Invoke();
        },
        delegate (string failmsg) // failed
        {
            failCallback?.Invoke(failmsg);
        });
    }
}
