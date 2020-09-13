using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoginScreen : UI
{
    [SerializeField]
    private GameObject panel_signup;
    [SerializeField]
    private InputField input_signUpEmail;
    [SerializeField]
    private InputField input_password1;
    [SerializeField]
    private InputField input_password2;
    [SerializeField]
    private InputField input_loginEmail;
    [SerializeField]
    private InputField input_loginPassword;

    // Start is called before the first frame update
    protected override void Start()
    {
        StartCoroutine(Autologin());
    }

    private IEnumerator Autologin()
    {
        yield return new WaitForSeconds(0.5f);
        // Check if user is already logged in
        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            bool verify = true;
            foreach (string str in DatabaseMgr.Instance.LoginTypes)
            {
                if (str != "password")
                    verify = false;

                Debug.Log("Providerid: " + str);
            }

            VerifyAndTransitToMenu(verify);
        }
    }

    public void Btn_ShowSignUp(bool flag)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_signup.SetActive(flag);
        input_signUpEmail.text = input_password1.text = input_password2.text = "";
    }

    public void Btn_Login()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (string.IsNullOrEmpty(input_loginEmail.text)) // empty email
        {
            NotificationMgr.Instance.Notify("Email cannot be empty.");
            return;
        }

        if (string.IsNullOrEmpty(input_loginPassword.text)) // empty password
        {
            NotificationMgr.Instance.Notify("Password cannot be empty.");
            return;
        }

        NotificationMgr.Instance.NotifyLoad("Logging in");
        DatabaseMgr.Instance.EmailLogin(input_loginEmail.text, input_loginPassword.text,
        delegate () // passed
        {
            NotificationMgr.Instance.StopLoad();
            VerifyAndTransitToMenu();
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.StopLoad();
            input_loginPassword.text = "";
            NotificationMgr.Instance.Notify(failmsg);
        });
    }

    public void Btn_SignUp()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (string.IsNullOrEmpty(input_signUpEmail.text)) // empty email
        {
            NotificationMgr.Instance.Notify("Email cannot be empty.");
            return;
        }

        if (string.IsNullOrEmpty(input_password1.text)) // empty password
        {
            NotificationMgr.Instance.Notify("Password cannot be empty.");
            return;
        }
        if (input_password1.text != input_password2.text)
        {
            NotificationMgr.Instance.Notify("Passwords do not match");
            return;
        }

        // register
        NotificationMgr.Instance.NotifyLoad("Creating account");
        DatabaseMgr.Instance.EmailRegister(input_signUpEmail.text, input_password2.text,
        delegate () // passed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify("Success! An email with a verification link has been sent to you; please click on it to complete the registration process. Remember to check your junk folder.", delegate ()
            {
                input_signUpEmail.text = input_password1.text = input_password2.text = "";
                panel_signup.SetActive(false);
            });
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.StopLoad();
            input_signUpEmail.text = input_password1.text = input_password2.text = "";
            NotificationMgr.Instance.Notify(failmsg);
        });
    }

    public void Btn_FBLogin()
    {
        NotificationMgr.Instance.NotifyLoad("Logging in via Facebook...");
        DatabaseMgr.Instance.FacebookLogin(
        delegate () // success
        {
            NotificationMgr.Instance.StopLoad();
            VerifyAndTransitToMenu(false); // don't verify email
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });
    }

    private void VerifyAndTransitToMenu(bool checkEmailVerified = true)
    {
        if (checkEmailVerified)
        {
            if (!DatabaseMgr.Instance.IsEmailVerified) // check for verified email
            {
                if (DatabaseMgr.Instance.IsLoggedIn)
                    DatabaseMgr.Instance.Logout();

                NotificationMgr.Instance.Notify("Before you can login, you must verify your email first by clicking on the link sent! Remember to check your junk folder.");
                return;
            }
        }

        NotificationMgr.Instance.NotifyLoad("Fetching profile");
        DatabaseMgr.Instance.LoadPlayerProfile(
        delegate () // success
        {
            NotificationMgr.Instance.StopLoad();
            // transit to menu screen
            TransitMgr.Instance.Fade(delegate ()
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
                TransitMgr.Instance.Emerge();
            });
        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg + "\n\nCreate new profile?",
            delegate () // ok pressed
            {
                NotificationMgr.Instance.RequestTextInput("Enter a name/nickname.",
                delegate (string input) // ok pressed
                {
                    // create profile
                    ProfileMgr.Instance.localProfile.name = input; // set name

                    NotificationMgr.Instance.NotifyLoad("Creating profile");
                    DatabaseMgr.Instance.SavePlayerProfile(
                    delegate () // success
                    {

                        NotificationMgr.Instance.StopLoad();

                        // transit to menu screen
                        TransitMgr.Instance.Fade(delegate ()
                        {
                            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
                            TransitMgr.Instance.Emerge();
                        });
                    },
                    delegate (string failmsg2) // failed to create profile
                    {
                        NotificationMgr.Instance.Notify(failmsg2);
                    });
                },
                delegate () // cancelled - don't create profile, sign out
                {
                    if (DatabaseMgr.Instance.IsLoggedIn)
                        DatabaseMgr.Instance.Logout();
                });
            },
            delegate () // cancel pressed - don't create profile, sign out
            {
                if (DatabaseMgr.Instance.IsLoggedIn)
                    DatabaseMgr.Instance.Logout();
            });
        });

    }
}
