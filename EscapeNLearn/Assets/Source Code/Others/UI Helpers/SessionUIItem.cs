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
    Session _session;
    string _id;

    public void Init(string id, Session session, SessionUI sessionUI, SessionLobbyUI sessionLobbyUI)
    {
        _ref = sessionUI;
        _ref2 = sessionLobbyUI;
        _id = id;
        _session = session;
        txt_id.text = "ID: " + id;
        txt_name.text = session.session_name;
    }

    public void OnClick()
    {
        if (_ref != null)
            _ref.ViewSessionReport(_id);
        if (_ref2 != null)
            _ref2.JoinSession(_id, _session.id_owner, _session.session_name);
    }
}
