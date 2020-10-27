using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Facebook.MiniJSON;


/// <summary>
/// A UI class encapsulated in MenuScreen in the Unity scene, handles UI functionality of the challenges panel.
/// </summary>
public class ChallengesUI : MonoBehaviour
{
    public GameObject PanelChild;
    public GameObject ListItem;
    public InputField input_FriendName;


    //ArrayList challenges_list;
    Dictionary<string, Challenge> challenges_list = new Dictionary<string, Challenge>();
    List<GameObject> spawnedObjList = new List<GameObject>();

    /// <summary>
    /// This method executes when the challenges panel is displayed.
    /// </summary>
    void OnEnable()
    {
        NotificationMgr.Instance.NotifyLoad("Fetching challenges");
        ProfileMgr.Instance.FetchChallenges(
         delegate (string result)
         {
             NotificationMgr.Instance.StopLoad();
             SpawnChallengeObjects(result);
         },
         delegate (string failmsg)
         {
             NotificationMgr.Instance.StopLoad();
             NotificationMgr.Instance.Notify(failmsg);
         });

    }

    /// <summary>
    /// This method executes when the challenges panel is hidden.
    /// </summary>
    private void OnDisable()
    {
        foreach (GameObject obj in spawnedObjList)
        {
            Destroy(obj);
        }

        spawnedObjList.Clear();
        challenges_list.Clear();
    }

    void SpawnChallengeObjects(string result)
    {
        Profile receiver = ProfileMgr.Instance.localProfile;

        Dictionary<string, object> results = Json.Deserialize(result) as Dictionary<string, object>;

        foreach (KeyValuePair<string, object> pair in results)
        {

            string challengedata = Json.Serialize(pair.Value);
            Challenge challenge = JsonUtility.FromJson<Challenge>(challengedata);
            challenges_list.Add(pair.Key, challenge);
        }

        foreach (KeyValuePair<string, Challenge> challenge in challenges_list)
        {
            if (challenge.Value.receiver_id == receiver.id_player)
            {
                GameObject newChallenge = Instantiate(ListItem) as GameObject;
                Challenges_ListItemController controller = newChallenge.GetComponent<Challenges_ListItemController>();
                controller.Init(challenge.Value, challenge.Key, this);
                newChallenge.transform.SetParent(PanelChild.transform);
                newChallenge.transform.localScale = Vector3.one;
                spawnedObjList.Add(newChallenge);
            }
        }
    }

    public void EnterChallenge(Challenge c, string id)
    {
        int level = c.level_cleared;
        string sessionid = c.session_id;
    
        if (level < 1 || level > 3)
        {
            Debug.Log("invalid level");
        }
        else
        {
            NotificationMgr.Instance.NotifyLoad("Loading Game Questions");
            SessionMgr.Instance.currChallenge = c;
            SessionMgr.Instance.challengeID = id;

            // fetch questions
            SessionMgr.Instance.FetchQuestionsForGame(sessionid, level,
            delegate () // success
            {
                NotificationMgr.Instance.StopLoad();
                TransitMgr.Instance.FadeToScene("Game_" + level);
            },
            delegate (string failmsg) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });

        }
    }

}
