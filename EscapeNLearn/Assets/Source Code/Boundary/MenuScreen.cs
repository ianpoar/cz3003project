using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.MiniJSON;

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
    public GameObject panel_sessionlobby;
    [SerializeField]
    private GameObject panel_levelselect;
    [SerializeField]
    private Text txt_info;
    [SerializeField]
    private Button btn_logout;
    [SerializeField]
    private Image profilePic;
    [SerializeField]
    private GameObject panel_questionlist;
    [SerializeField]
    private GameObject FBLinkButton;
    [SerializeField]
    private GameObject FBUnLinkButton;

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

        TransitMgr.Instance.FadeToScene("Login");
    }

    // Play Normal button pressed
    public void Btn_PlayNormal(bool playsound)
    {
        if (playsound)
            AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (ProfileMgr.Instance.currentConnection != null)
        {
            panel_levelselect.SetActive(true); // show level select
        }
        else
        {
            panel_sessionlobby.SetActive(true); // show session lobby
        }
    }

    // Play challenge button pressed
    public void Btn_PlayChallenge()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        NotificationMgr.Instance.Notify("This feature has not been implemented yet.");
    }

    public void Btn_QuestionList(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        panel_questionlist.SetActive(!panel_questionlist.activeSelf); // show create question
    }

    public void Btn_FBUnlink()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        NotificationMgr.Instance.NotifyLoad("Unlinking");
        DatabaseMgr.Instance.UnlinkCredentials(LoginTypeConstants.FACEBOOK,
            delegate ()
            {
                ProfileMgr.Instance.localProfile.id_facebook = "Unknown"; // unlinked, reset id_facebook

                DatabaseMgr.Instance.DBLightUpdate(
                DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id,
                nameof(ProfileMgr.Instance.localProfile.id_facebook),
                ProfileMgr.Instance.localProfile.id_facebook,
                delegate () // write success
                {
                    NotificationMgr.Instance.StopLoad();
                    NotificationMgr.Instance.Notify("Unlink successful");
                    // refresh new data locally
                    RefreshProfileInfo();
                },
                delegate (string failmsg) // write failed
                {
                    NotificationMgr.Instance.StopLoad();
                    NotificationMgr.Instance.Notify(failmsg);
                });
            },
            delegate (string failmsg) {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });
    }

    public void Btn_FBLink()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        NotificationMgr.Instance.NotifyLoad("Linking Fb");
        DatabaseMgr.Instance.SNSRequestCredential(
        LoginTypeConstants.FACEBOOK,
        delegate (Firebase.Auth.Credential cred) // success
        {
            DatabaseMgr.Instance.LinkCredentials(
                cred,
                delegate () // successfully linked, update facebook id
                {
                    DatabaseMgr.Instance.DBLightUpdate(
                       DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id,
                       nameof(ProfileMgr.Instance.localProfile.id_facebook),
                       ProfileMgr.Instance.localProfile.id_facebook,
                       delegate () // write success
                        {
                           DatabaseMgr.Instance.Logout();
                           NotificationMgr.Instance.StopLoad();
                           NotificationMgr.Instance.Notify("Link successful. Please relogin.",
                           delegate()
                           {
                               TransitMgr.Instance.FadeToScene("Login");
                           });
                            // refresh new data locally
                            RefreshProfileInfo();
                        },
                       delegate (string failmsg) // write failed
                        {
                           NotificationMgr.Instance.StopLoad();
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
        DatabaseMgr.Instance.DBLightUpdate(DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id, nameof(profile.accountExp), profile.accountExp + 1,
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
        FBUnLinkButton.SetActive(false);
        FBLinkButton.SetActive(false);
        txt_info.text = ""; // reset

        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            FBLinkButton.SetActive(true);

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
                if (str == LoginTypeConstants.FACEBOOK)
                {
                    FBUnLinkButton.SetActive(true);
                    FBLinkButton.SetActive(false);
                }
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
                    {
                        obj.SetActive(true);
                    }
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
