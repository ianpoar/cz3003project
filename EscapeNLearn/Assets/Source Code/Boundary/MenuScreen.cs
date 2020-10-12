using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Boundary Class for the Menu scene, handles all UI-related events in the scene, encapsulates SessionUI, QuestionUI, LevelSelectUI, SessionLobbyUI, SettingsUI, FriendsUI.
/// </summary>
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
    [SerializeField]
    private GameObject GoogleLinkButton;
    [SerializeField]
    private GameObject GoogleUnLinkButton;

    [SerializeField]
    private GameObject panel_friends;

    /// <summary>
    /// Start of the Menu screen.
    /// </summary>
    protected override void Start()
    {
        // Call parent start
        base.Start();

        // Play BGM
        AudioMgr.Instance.PlayBGM(AudioConstants.BGM_PERCEPTION);
    }

    /// <summary>
    /// An handler that executes actions only after profile data is fetched.
    /// </summary>
    protected override void StartAfterDataFetched()
    {
        CheckLoginDetails();
    }

    /// <summary>
    /// An handler for when the profile button is pressed, enables/disables the profile panel and its ProfileUI class.
    /// </summary>
    public void Btn_ShowProfile(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (show)
        {
            RefreshProfileInfo();
        }

        panel_profileinfo.SetActive(show);
    }

    /// <summary>
    /// An handler for when the settings button is pressed, enables/disables the settings panel and its SettingsUI class.
    /// </summary>
    public void Btn_ShowSettings(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_settings.SetActive(show);
    }

    /// <summary>
    /// An handler for when the view sessions button is pressed by an instructor, enables/disables the sessions panel and its SessionUI class.
    /// </summary>
    public void Btn_ShowSessions(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_sessions.SetActive(show);
    }

    /// <summary>
    /// An handler for when the logout button is pressed, logs the current player out.
    /// </summary>
    public void Btn_Logout()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (DatabaseMgr.Instance.IsLoggedIn)
            DatabaseMgr.Instance.Logout();

        TransitMgr.Instance.FadeToScene("Login");
    }

    /// <summary>
    /// An handler for when the play normal button is pressed, enables the session lobby panel and its SessionLobbyUI class, or the level select panel and its LevelSelectUI class, depending on whether the player has joined a session.
    /// </summary>
    public void Btn_PlayNormal(bool playsound)
    {
        if (playsound)
            AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (SessionMgr.Instance.currentConnection != null)
        {
            panel_levelselect.SetActive(true); // show level select
        }
        else
        {
            panel_sessionlobby.SetActive(true); // show session lobby
        }
    }


    /// <summary>
    /// An handler for when the play challenge button is pressed.
    /// </summary>
    public void Btn_PlayChallenge()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        NotificationMgr.Instance.Notify("This feature has not been implemented yet.");
    }

    /// <summary>
    /// An handler for when the view questions button is pressed by an instructor, enables/disables the questions panel and its QuestionUI class.
    /// </summary>
    public void Btn_QuestionList(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        panel_questionlist.SetActive(!panel_questionlist.activeSelf); // show create question
    }

    /// <summary>
    /// An handler for when the unlink FB button is pressed, unlinks the player's Facebook profile from his game account.
    /// </summary>
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
                nameof(Profile.id_facebook),
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
            delegate (string failmsg)
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });
    }

    /// <summary>
    /// An handler for when the unlink Google button is pressed, unlinks the player's Google profile from his game account.
    /// </summary>
    public void Btn_GoogleUnlink()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        NotificationMgr.Instance.NotifyLoad("Unlinking");
        DatabaseMgr.Instance.UnlinkCredentials(LoginTypeConstants.GOOGLE,
            delegate ()
            {
                ProfileMgr.Instance.localProfile.id_google = "Unknown"; // unlinked, reset id_google

                DatabaseMgr.Instance.DBLightUpdate(
                DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id,
                nameof(Profile.id_google),
                ProfileMgr.Instance.localProfile.id_google,
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
            delegate (string failmsg)
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });
    }

    /// <summary>
    /// An handler for when the link Google button is pressed, links the player's Google profile to his game account.
    /// </summary>
    public void Btn_GoogleLink()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        DatabaseMgr.Instance.SNSRequestCredential(
        LoginTypeConstants.GOOGLE,
        delegate (Firebase.Auth.Credential cred) // success
        {
            NotificationMgr.Instance.NotifyLoad("Linking Google");
            DatabaseMgr.Instance.LinkCredentials(
                cred,
                delegate () // successfully linked, update facebook id
                {
                    DatabaseMgr.Instance.DBLightUpdate(
                       DBQueryConstants.QUERY_PROFILES + "/" + DatabaseMgr.Instance.Id,
                       nameof(Profile.id_google),
                       ProfileMgr.Instance.localProfile.id_google,
                       delegate () // write success
                       {
                           DatabaseMgr.Instance.Logout();
                           NotificationMgr.Instance.StopLoad();
                           NotificationMgr.Instance.Notify("Link successful. Please relogin.",
                           delegate ()
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

    /// <summary>
    /// An handler for when the link Facebook button is pressed, links the player's Facebook profile to his game account.
    /// </summary>
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
                       nameof(Profile.id_facebook),
                       ProfileMgr.Instance.localProfile.id_facebook,
                       delegate () // write success
                        {
                            DatabaseMgr.Instance.Logout();
                            NotificationMgr.Instance.StopLoad();
                            NotificationMgr.Instance.Notify("Link successful. Please relogin.",
                            delegate ()
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

    /// <summary>
    /// A demo method to demonstrate the ability to write data to a document in the database, using account exp as an example.
    /// </summary>
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

    /// <summary>
    /// A method that refreshes the displayed profile information.
    /// </summary>
    private void RefreshProfileInfo()
    {
        FBUnLinkButton.SetActive(false);
        FBLinkButton.SetActive(false);
        GoogleUnLinkButton.SetActive(false);
        GoogleLinkButton.SetActive(false);
        txt_info.text = ""; // reset

        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            FBLinkButton.SetActive(true);
            GoogleLinkButton.SetActive(true);

            txt_info.text +=
                "Firebase UID: " + DatabaseMgr.Instance.Id +
                "\nEmail: " + DatabaseMgr.Instance.Email;
            if (DatabaseMgr.Instance.IsEmailVerified)
                txt_info.text += " (Verified)";

            txt_info.text +=
                "\nLogin Types: ";

            List<string> loginTypes = DatabaseMgr.Instance.LoginTypes;
            int count = loginTypes.Count;

            foreach (string str in loginTypes)
            {
                txt_info.text += str + " | ";
                if (str == LoginTypeConstants.FACEBOOK)
                {
                    FBLinkButton.SetActive(false);
                    if (count > 1) // only allow unlinking if there are other login types
                        FBUnLinkButton.SetActive(true);
                }
                if (str == LoginTypeConstants.GOOGLE)
                {
                    GoogleLinkButton.SetActive(false);
                    if (count > 1) // only allow unlinking if there are other login types
                        GoogleUnLinkButton.SetActive(true);
                }
            }

            Profile profile = ProfileMgr.Instance.localProfile;
            txt_info.text +=
                "\nName: " + profile.name +
                "\nPlayer UID: " + profile.id_player +
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

    /// <summary>
    /// A method that checks the current player's login details and displays the appropriate information and UI objects.
    /// </summary>
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
            List<string> loginTypes = DatabaseMgr.Instance.LoginTypes;
            if (loginTypes.Contains(LoginTypeConstants.FACEBOOK) && !loginTypes.Contains(LoginTypeConstants.GOOGLE)) // if facebook only, no google
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

    /// <summary>
    /// A handler for when the friends button is pressed, enables/disables the friends panel and its FriendsUI class.
    /// </summary>
    public void Btn_ShowFriendsList(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_friends.SetActive(show);
    }
}
