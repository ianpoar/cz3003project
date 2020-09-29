using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionUIItem : MonoBehaviour
{
    public Text txt_name;
    public Text txt_id;

    SessionUI _ref;
    string _id;

    public void Init(SessionUI refer, string id, string name)
    {
        _ref = refer;
        _id = id;
        txt_id.text = "ID: " + id;
        txt_name.text = name;
    }

    public void OnClick()
    {
        _ref.EditSession(_id);
    }
}
