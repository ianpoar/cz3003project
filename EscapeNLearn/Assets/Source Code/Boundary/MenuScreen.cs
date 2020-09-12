using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

public class MenuScreen : UI
{
    // References
    [SerializeField]
    private Text txt_placeholder;

    // Start is called before the first frame update
    void Start()
    {
        RefreshPlaceholderText();
    }

    void RefreshPlaceholderText()
    {
        txt_placeholder.text = "Email: " + DatabaseMgr.Instance.Email + "\nUID: " + DatabaseMgr.Instance.Id + "\nNumber: " + ProfileMgr.Instance.localProfile.number;
    }

    public void Btn_Logout()
    {
        if (DatabaseMgr.Instance.IsSignedIn)
            DatabaseMgr.Instance.SignOut();

        TransitMgr.Instance.Fade(delegate ()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
            TransitMgr.Instance.Emerge();
        });
    }

    public void Btn_Add()
    {
        ProfileMgr.Instance.localProfile.number++;
        DatabaseMgr.Instance.SavePlayerProfile();
        RefreshPlaceholderText();
    }
}
