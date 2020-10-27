using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Facebook.MiniJSON;


/// <summary>
/// A UI class encapsulated in MenuScreen in the Unity scene, handles UI functionality of the friends panel.
/// </summary>
public class FriendsUI : MonoBehaviour
{
    public GameObject PanelChild;
    public GameObject ListItem;
    public InputField input_FriendName;


    ArrayList friends_list;
    List<GameObject> spawnedObjList = new List<GameObject>();

    /// <summary>
    /// This method executes when the friends panel is displayed.
    /// </summary>
    void OnEnable()
    {
        NotificationMgr.Instance.NotifyLoad("Fetching friends");
        ProfileMgr.Instance.FetchFriends(
         delegate (string result)
         {
             NotificationMgr.Instance.StopLoad();
             SpawnFriendObjects(result);
         },
         delegate (string failmsg)
         {
             NotificationMgr.Instance.StopLoad();
             NotificationMgr.Instance.Notify(failmsg);
         });
    }

    /// <summary>
    /// This method executes when the friends panel is hidden.
    /// </summary>
    private void OnDisable()
    {
        foreach (GameObject obj in spawnedObjList)
        {
            Destroy(obj);
        }

        spawnedObjList.Clear();
    }

    public void Btn_Search()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        Debug.Log(input_FriendName.text);
    }

    void SpawnFriendObjects(string result)
    {

        Dictionary<string, object> results = Json.Deserialize(result) as Dictionary<string, object>;


        friends_list = new ArrayList();
        foreach (KeyValuePair<string, object> pair in results)
        {

            string profiledata = Json.Serialize(pair.Value);

            Profile profile = JsonUtility.FromJson<Profile>(profiledata);


            friends_list.Add(profile);

        }

        foreach (Profile friend in friends_list)
        {
            GameObject newFriend = Instantiate(ListItem) as GameObject;
            Friends_ListItemController controller = newFriend.GetComponent<Friends_ListItemController>();
            controller.Name.text = friend.name;
            newFriend.transform.SetParent(PanelChild.transform);
            newFriend.transform.localScale = Vector3.one;
            controller.Init(this, friend);
            spawnedObjList.Add(newFriend);
        }
    }

    public void sendChallenge(Profile friend)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        Connection con = SessionMgr.Instance.currentConnection;
        Profile p = ProfileMgr.Instance.localProfile;

        if (con == null)
        {
            NotificationMgr.Instance.Notify("You need to join a session first to send a challenge!");
            return;
        }

        if (con.level_cleared <= 0)
        {
            NotificationMgr.Instance.Notify("To send a challenge, you need to clear a level in your current session first.");
            return;
        }

        if (con.level_cleared > 3)
        {
            NotificationMgr.Instance.Notify("Invalid challenge level (" + con.level_cleared + ").");
            return;
        }

        int reward = 10;
        Challenge c = new Challenge();
        c.sender_id = con.id_player;
        c.session_id = con.id_session;
        c.receiver_id = friend.id_player;
        c.level_cleared = con.level_cleared;
        NotificationMgr.Instance.NotifyLoad("Sending");
        createChallenge(c,
        delegate () // success
        {
            NotificationMgr.Instance.StopLoad();
            Profile profile = ProfileMgr.Instance.localProfile;
            DatabaseMgr.Instance.DBLightUpdate(DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id, nameof(profile.currency_normal), profile.currency_normal + reward,
            delegate () // write success
            {
                profile.currency_normal += reward;
            }
            );
            NotificationMgr.Instance.Notify("Challenge sent successfully. Rewarded " + reward + " currency.");
        },
        delegate (string failmsg)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });
    }

    public void createChallenge(Challenge c, SimpleCallback successCallback, MessageCallback failCallback)
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
