using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Facebook.MiniJSON;


/// <summary>
/// A UI class encapsulated in MenuScreen in the Unity scene, handles UI functionality of the friends panel.
/// </summary>
public class ChallengesUI : MonoBehaviour
{
    public GameObject PanelChild;
    public GameObject ListItem;
    public InputField input_FriendName;


    ArrayList challenges_list;
    List<GameObject> spawnedObjList = new List<GameObject>();

    /// <summary>
    /// This method executes when the friends panel is displayed.
    /// </summary>
    void OnEnable()
    {
        NotificationMgr.Instance.NotifyLoad("Fetching challenges");
        ProfileMgr.Instance.FetchChallenges(
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
        Profile receiver = ProfileMgr.Instance.localProfile;

        Dictionary<string, object> results = Json.Deserialize(result) as Dictionary<string, object>;


        challenges_list = new ArrayList();
        foreach (KeyValuePair<string, object> pair in results)
        {

            string profiledata = Json.Serialize(pair.Value);

            ChallengeListData profile = JsonUtility.FromJson<ChallengeListData>(profiledata);


            challenges_list.Add(profile);

        }

        foreach (ChallengeListData friend in challenges_list)
        {
            GameObject newFriend = Instantiate(ListItem) as GameObject;
            Challenges_ListItemController controller = newFriend.GetComponent<Challenges_ListItemController>();

            // controller.Init(this, friend);

            if (friend.receiver_id == receiver.id_player)
            {
                controller.Name.text = friend.sender_id;
                newFriend.transform.parent = PanelChild.transform;
                newFriend.transform.localScale = Vector3.one;
                spawnedObjList.Add(newFriend);
            }
        }
    }


}
