using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Friends_ListItemController : MonoBehaviour
{

    FriendsUI _ref;
    Profile _friend;
    public Text Name = null;


    public void Init(FriendsUI friendsUI, Profile friend)

    {
        _ref = friendsUI;
        _friend = friend;
        Name.text = friend.name;
    }

    public void OnClick()
    {

        if (_ref != null)
            _ref.SendChallenge(_friend);

    }

}
