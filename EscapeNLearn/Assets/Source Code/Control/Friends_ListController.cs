using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Friends_ListController : MonoBehaviour
{

    public GameObject PanelChild;
    public GameObject ListItem;

    ArrayList friends_list;

    void Start()
    {

        // 1. Get the data to be displayed
        friends_list = new ArrayList() {
            new FriendsListData("Ron"), new FriendsListData("Tom"), new FriendsListData("stacy"), new FriendsListData("mon"), new FriendsListData("Ton"), new FriendsListData("stac"), new FriendsListData("Ron"), new FriendsListData("Tom"), new FriendsListData("stacy"), new FriendsListData("mon"), new FriendsListData("Ton"), new FriendsListData("stac")

        };

        foreach (FriendsListData friend in friends_list)
        {
            GameObject newFriend = Instantiate(ListItem) as GameObject;
            Friends_ListItemController controller = newFriend.GetComponent<Friends_ListItemController>();
            controller.Name.text = friend.Name;
            newFriend.transform.parent = PanelChild.transform;
            newFriend.transform.localScale = Vector3.one;
        }
    }
}


