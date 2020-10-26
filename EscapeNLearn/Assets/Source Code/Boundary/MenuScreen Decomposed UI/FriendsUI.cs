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
        NotificationMgr.Instance.NotifyLoad("Fetching profiles");
        ProfileMgr.Instance.FetchProfiles(
         delegate (string result)
         {
             NotificationMgr.Instance.StopLoad();
             SpawnProfileObjects(result);
         },
         delegate (string failmsg)
         {
             NotificationMgr.Instance.StopLoad();
             NotificationMgr.Instance.Notify(failmsg);
         });
        // 1. Get the data to be displayed

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


        // ClearSessionObjects();
        // if (input_FriendName.text != "")
        // {
        //     GenerateSearchSessionObjects(input_FriendName.text);
        // }
        // else
        // {
        //     GenerateAllSessionObjects();
        // }
    }

    void SpawnProfileObjects(string result)
    {

        Dictionary<string, object> results = Json.Deserialize(result) as Dictionary<string, object>;


        friends_list = new ArrayList();
        foreach (KeyValuePair<string, object> pair in results)
        {

            string profiledata = Json.Serialize(pair.Value);

            FriendsListData profile = JsonUtility.FromJson<FriendsListData>(profiledata);


            friends_list.Add(profile);

        }

        foreach (FriendsListData friend in friends_list)
        {
            GameObject newFriend = Instantiate(ListItem) as GameObject;
            Friends_ListItemController controller = newFriend.GetComponent<Friends_ListItemController>();
            controller.Name.text = friend.name;
            newFriend.transform.parent = PanelChild.transform;
            newFriend.transform.localScale = Vector3.one;
            controller.Init(this, friend);
            spawnedObjList.Add(newFriend);
        }
    }

    public void sendChallenge(string session, int level, string senderID, string receiverID)
    {
        Challenges c = new Challenges(session, level, senderID, receiverID);
        createChallenge(c,
        delegate () // success
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify("Challenge sent successfully.");
        },
        delegate (string failmsg)
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });


    }

    public void createChallenge(Challenges c, SimpleCallback successCallback, MessageCallback failCallback)
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
