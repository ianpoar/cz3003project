using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionUIItem : MonoBehaviour
{
    public Text txt_name;
    public Text txt_id;

    SessionUI _ref;
    SessionLobbyUI _ref2;
    string _id;

    public void Init(string id, string name, SessionUI sessionUI, SessionLobbyUI sessionLobbyUI)
    {
        _ref = sessionUI;
        _ref2 = sessionLobbyUI;
        _id = id;
        txt_id.text = "ID: " + id;
        txt_name.text = name;
    }

    public void OnClick()
    {
        if (_ref != null)
            _ref.EditSession(_id);
        if (_ref2 != null)
            _ref2.JoinSession(_id);
    }
}
