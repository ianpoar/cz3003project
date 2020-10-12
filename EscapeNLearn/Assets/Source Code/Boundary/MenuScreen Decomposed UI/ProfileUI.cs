using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI class encapsulated in MenuScreen in the Unity scene, handles UI functionality of the profile panel.
/// </summary>
public class ProfileUI : MonoBehaviour
{
    [SerializeField]
    private Text txt_profileinfo;
    [SerializeField]
    private GameObject FBLinkButton;
    [SerializeField]
    private GameObject FBUnLinkButton;
    [SerializeField]
    private GameObject GoogleLinkButton;
    [SerializeField]
    private GameObject GoogleUnLinkButton;
    [SerializeField]
    private Button btn_logout;

    /// <summary>
    /// This method executes when the profile panel is displayed.
    /// </summary>
    private void OnEnable()
    {
        RefreshProfileInfo();
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
        txt_profileinfo.text = ""; // reset

        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            FBLinkButton.SetActive(true);
            GoogleLinkButton.SetActive(true);

            txt_profileinfo.text +=
                "Firebase UID: " + DatabaseMgr.Instance.Id +
                "\nEmail: " + DatabaseMgr.Instance.Email;
            if (DatabaseMgr.Instance.IsEmailVerified)
                txt_profileinfo.text += " (Verified)";

            txt_profileinfo.text +=
                "\nLogin Types: ";

            List<string> loginTypes = DatabaseMgr.Instance.LoginTypes;
            int count = loginTypes.Count;

            foreach (string str in loginTypes)
            {
                txt_profileinfo.text += str + " | ";
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
            txt_profileinfo.text +=
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
            txt_profileinfo.text +=
                "You are not logged in.";

            btn_logout.interactable = false;
        }
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
}
