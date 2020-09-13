using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

public class MenuScreen : UI
{
    // References
    [SerializeField]
    private GameObject panel_profileinfo;
    [SerializeField]
    private List<GameObject> objects_instructorOnly;
    [SerializeField]
    private GameObject panel_settings;
    [SerializeField]
    private Text txt_info;
    [SerializeField]
    private Button btn_logout;

    protected override void Start()
    {
        base.Start();
        if (DatabaseMgr.Instance.IsLoggedIn && ProfileMgr.Instance.localProfile.accountType == "Instructor") // enable instructor options
        {
            foreach (GameObject obj in objects_instructorOnly)
                obj.SetActive(true);
        }
        AudioMgr.Instance.PlayBGM(AudioConstants.BGM_PERCEPTION);
    }

    public void Btn_ShowProfile(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (show)
        {
            RefreshProfileInfo();
        }

        panel_profileinfo.SetActive(show);
    }

    void RefreshProfileInfo()
    {
        txt_info.text = ""; // reset

        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            txt_info.text +=
                "\nUID: " + DatabaseMgr.Instance.Id +
                "\n Email: " + DatabaseMgr.Instance.Email;
            if (DatabaseMgr.Instance.IsEmailVerified)
                txt_info.text += " (Verified)";

            txt_info.text +=
                "\nLogin Types: ";
            foreach (string str in DatabaseMgr.Instance.LoginTypes)
            {
                txt_info.text += str + " | ";
            }

            Profile profile = ProfileMgr.Instance.localProfile;
            txt_info.text +=
                "\nName: " + profile.name +
                "\nAccount Type: " + profile.accountType +
                "\nAccount EXP: " + profile.accountExp +
                "\nNormal Currency: " + profile.currency_normal +
                "\nPremium Currency: " + profile.currency_premium;

            btn_logout.interactable = true;
        }
        else
        {
            txt_info.text +=
                "You are not logged in.";

            btn_logout.interactable = false;
        }
    }

    public void Btn_ShowSettings(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_settings.SetActive(show);
    }

    public void Btn_Logout()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (DatabaseMgr.Instance.IsLoggedIn)
            DatabaseMgr.Instance.Logout();

        TransitMgr.Instance.Fade(delegate ()
        {
            AudioMgr.Instance.StopBGM();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
            TransitMgr.Instance.Emerge();
        });
    }

    public void Btn_TestAddEXP()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (!DatabaseMgr.Instance.IsLoggedIn)
            return;

        ProfileMgr.Instance.localProfile.accountExp++;
        DatabaseMgr.Instance.SavePlayerProfile();
        RefreshProfileInfo();
    }

    public void Btn_PlayNormal()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        NotificationMgr.Instance.RequestTextInput("Enter Session ID: ",
        delegate (string input)
        {
            // use session ID to do something - to be implemented
        },
        delegate () // cancel
        {
            // do nth
        });
    }
}
