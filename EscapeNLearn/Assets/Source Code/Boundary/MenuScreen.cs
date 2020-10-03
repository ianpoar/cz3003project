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
    private GameObject panel_sessions;
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
        Debug.Log("Database Instance" + DatabaseMgr.Instance.Id);
        Debug.Log("ProfileMgr Instance" + ProfileMgr.Instance.localProfile.id_account);
        Debug.Log("Firebase Instance" + Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId);
    }

    // Button to show sessions pressed
    public void Btn_ShowSessions(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_sessions.SetActive(show);
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
        TransitMgr.Instance.FadeToScene("Game_1");

        /*
        NotificationMgr.Instance.RequestTextInput("Enter Session ID: ",
        delegate (string input)
        {
            // use session ID to do something - to be implemented
            TransitMgr.Instance.FadeToScene("Game_Escape");
        },
        delegate () // cancel
        {
            // do nth
        }); */
    }

    public void Btn_FBUnlink()
    {
        Debug.Log(Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser);
        NotificationMgr.Instance.NotifyLoad("Linking Fb");
        DatabaseMgr.Instance.SNSRequestCredential(
        LoginTypeConstants.FACEBOOK,
        delegate (Firebase.Auth.Credential cred) // success
        {
            Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UnlinkAsync("facebook.com").ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UnlinkAsync was canceled.");
                    NotificationMgr.Instance.StopLoad();
                    NotificationMgr.Instance.Notify("UnlinkAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UnlinkAsync encountered an error: " + task.Exception);
                    NotificationMgr.Instance.StopLoad();
                    NotificationMgr.Instance.Notify("UnlinkAsync encountered an error: " + task.Exception.ToString());
                    return;
                }

                // The user has been unlinked from the provider.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Credentials successfully unlinked from user: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify("Credentials successfully unlinked from user");
            });
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });

    }

    // Example for Aru
    public void Btn_FBLink()
    {
        Debug.Log(Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser);
        NotificationMgr.Instance.NotifyLoad("Linking Fb");
        DatabaseMgr.Instance.SNSRequestCredential(
        LoginTypeConstants.FACEBOOK,
        delegate (Firebase.Auth.Credential cred) // success
        {
            DatabaseMgr.Instance.LinkCredentials(
                cred,
                delegate ()
                {
                    DatabaseMgr.Instance.DBLightUpdate(
                        DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id,
                        nameof(ProfileMgr.Instance.localProfile.id_facebook),
                        ProfileMgr.Instance.localProfile.id_facebook,
                        delegate () // write success
                        {
                            // refresh new data locally
                            Profile profile = ProfileMgr.Instance.localProfile;
                            profile.id_facebook = ProfileMgr.Instance.localProfile.id_facebook;
                            RefreshProfileInfo();
                            NotificationMgr.Instance.StopLoad();
                            NotificationMgr.Instance.Notify("Link succsessful");
                        },
                        delegate (string failmsg) // write failed
                        {
                            NotificationMgr.Instance.StopLoad(); // allow input
                            NotificationMgr.Instance.Notify(failmsg);
                        }
                    );
                },
                delegate (string failmsg)
                {
                    NotificationMgr.Instance.StopLoad();
                    NotificationMgr.Instance.Notify(failmsg);
                }
            );
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });

    }

    // Use this as a reference for modifying user data and saving it to db
    public void Btn_TestAddEXP()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (!DatabaseMgr.Instance.IsLoggedIn)
            return;

        // Method to update data to db here!

        // This is required to block input - only allow input after the update is done
        NotificationMgr.Instance.TransparentLoad();

        Profile profile = ProfileMgr.Instance.localProfile;
        DatabaseMgr.Instance.DBLightUpdate(DBQueryConstants.QUERY_PROFILES + DatabaseMgr.Instance.Id, nameof(profile.accountExp), profile.accountExp + 1,
        delegate () // write success
        {
            NotificationMgr.Instance.StopLoad(); // allow input

            // refresh new data locally
            profile.accountExp++;
            RefreshProfileInfo();
        },
        delegate (string failmsg) // write failed
        {
            NotificationMgr.Instance.StopLoad(); // allow input
        });
    }

    // Use this as a reference for accessing user data such as login type etc.
    private void RefreshProfileInfo()
    {
        txt_info.text = ""; // reset

        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            txt_info.text +=
                "Firebase UID: " + DatabaseMgr.Instance.Id +
                "\nEmail: " + DatabaseMgr.Instance.Email;
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
                "\nAccount ID: " + profile.id_account +
                "\nAccount Type: " + profile.accountType +
                "\nAccount EXP: " + profile.accountExp +
                "\nNormal Currency: " + profile.currency_normal +
                "\nPremium Currency: " + profile.currency_premium +
                "\nFacebook ID: " + profile.id_facebook +
                "\nGoogle ID: " + profile.id_google;

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
                {
                    if (obj != null)
                        obj.SetActive(true);
                }
            }

            // if user has facebook linked
            if (DatabaseMgr.Instance.LoginTypes.Contains(LoginTypeConstants.FACEBOOK))
            {
                // fetch profile pic
                DatabaseMgr.Instance.FetchProfilePic(ProfileMgr.Instance.localProfile.id_facebook,
                    delegate (Sprite sprite)
                    {
                        profilePic.sprite = sprite;
                    },
                    null);
            }
        }

    }
}
