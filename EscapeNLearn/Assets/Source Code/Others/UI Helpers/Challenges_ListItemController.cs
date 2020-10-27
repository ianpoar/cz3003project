using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Challenges_ListItemController : MonoBehaviour
{

    public Text info;
    ChallengesUI _UIRef;
    Challenge _challenge;
    string _id;

    public void Init(Challenge c, string id, ChallengesUI uiref)
    {
        _challenge = c;
        _id = id;
        _UIRef = uiref;
        info.text = "";
        info.text += "Sent by: " + _challenge.sender_id;
        info.text += "\nSession ID:\n" + _challenge.session_id;
        info.text += "\nLevel: " + _challenge.level_cleared;
    }

    public void OnClick()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        Debug.Log("enter challenge");
        _UIRef.EnterChallenge(_challenge, _id);
    }

}
