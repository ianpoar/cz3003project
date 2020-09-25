using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : Screen
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
    [SerializeField]
    private Image profilePic;

    // Start of menu screen
    protected override void Start()
    {
        // Call parent start
        base.Start();

        // Play BGM
        AudioMgr.Instance.PlayBGM(AudioConstants.BGM_PERCEPTION);
    }

    protected override void StartAfterDataFetched()
    {
        CheckLoginDetails();
    }

    // Button to show profile pressed
    public void Btn_ShowProfile(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (show)
        {
            RefreshProfileInfo();
        }

        panel_profileinfo.SetActive(show);
    }

    // Button to show settings pressed
    public void Btn_ShowSettings(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_settings.SetActive(show);
    }

    // Logout button at settings panel pressed
    public void Btn_Logout()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (DatabaseMgr.Instance.IsLoggedIn)
            DatabaseMgr.Instance.Logout();

        TransitMgr.Instance.FadeToScene(SceneConstants.SCENE_LOGIN);
    }

    // Play Normal button pressed, to be implemented
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

    // Example for Aru
    public void Btn_FBLink()
    {
        NotificationMgr.Instance.NotifyLoad("Linking Fb");
        DatabaseMgr.Instance.SNSLogin(
        LoginTypeConstants.FACEBOOK,
        delegate () // success
        {
            // link here
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });
    }

    // Placeholder function that increments a value in the player's profile data and saves it to firebase db
    // Can be used as a reference for modifying user data and saving it to db
    public void Btn_TestAddEXP()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (!DatabaseMgr.Instance.IsLoggedIn)
            return;

        ProfileMgr.Instance.localProfile.accountExp++;
        ProfileMgr.Instance.SavePlayerProfile();
        RefreshProfileInfo();
    }

    // Placeholder function that displays some debug text in settings panel
    // Can be used as a reference for accessing user data such as login type etc.
    private void RefreshProfileInfo()
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

    // Checking login details and displaying appropriate UI
    private void CheckLoginDetails()
    {
        // If user is logged in
        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            // if user is an instructor
            if (ProfileMgr.Instance.localProfile.accountType == "Instructor")
            {
                // show instructor UI
                foreach (GameObject obj in objects_instructorOnly)
                    obj.SetActive(true);
            }

            // if user has facebook linked
            if (DatabaseMgr.Instance.LoginTypes.Contains(LoginTypeConstants.FACEBOOK))
            {
                // fetch profile pic
                DatabaseMgr.Instance.FetchProfilePic(ProfileMgr.Instance.localProfile.facebookid,
                    delegate (Sprite sprite)
                    {
                        profilePic.sprite = sprite;
                    },
                    delegate (string failmsg)
                    {
                        NotificationMgr.Instance.Notify(failmsg);
                    });
            }
        }

    }
}
