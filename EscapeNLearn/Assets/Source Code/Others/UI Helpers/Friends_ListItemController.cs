using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Friends_ListItemController : MonoBehaviour
{

    FriendsUI _ref;
    FriendsListData _profile;
    public Text Name = null;
    public Text Id = null;


    public void Init(FriendsUI friendsUI, FriendsListData friendsListData)

    {
        _ref = friendsUI;
        _profile = friendsListData;
    }

    public void OnClick()
    {
        Profile profile = ProfileMgr.Instance.localProfile;
        Connection sessionID = SessionMgr.Instance.currentConnection;


        if (_ref != null)
            _ref.sendChallenge(sessionID.id_session, sessionID.level_cleared, profile.id_player, _profile.id_player);

    }

}
