using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoginScreen : Screen
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

    // Start of login screen
    protected override void Start()
    {
        // Run autologin
        StartCoroutine(Autologin());
        AudioMgr.Instance.PlayBGM(AudioConstants.BGM_CLEARDAY);
    }

    // Button to show sign up panel pressed
    public void Btn_ShowSignUp(bool flag)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_signup.SetActive(flag);
        input_signUpEmail.text = input_password1.text = input_password2.text = "";
    }

    // Login button pressed
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

    // Sign up button pressed
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

    // FBLogin button pressed
    public void Btn_FBLogin()
    {
        NotificationMgr.Instance.NotifyLoad("Logging in via Facebook...");
        DatabaseMgr.Instance.SNSLogin(
        LoginTypeConstants.FACEBOOK,
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

    // Transit to menu screen
    private void VerifyAndTransitToMenu(bool checkEmailVerified = true)
    {
        // check for verified email
        if (checkEmailVerified)
        {
            if (!DatabaseMgr.Instance.IsEmailVerified) // if email is not verified, prevent login
            {
                if (DatabaseMgr.Instance.IsLoggedIn)
                    DatabaseMgr.Instance.Logout();

                NotificationMgr.Instance.Notify("Before you can login, you must verify your email first by clicking on the link sent! Remember to check your junk folder.");
                return;
            }
        }

        // Fetch player profile
        NotificationMgr.Instance.NotifyLoad("Fetching profile");
        ProfileMgr.Instance.LoadPlayerProfile(
        delegate () // successfully fetched
        {
            NotificationMgr.Instance.StopLoad();
            // transit to menu screen
            TransitMgr.Instance.FadeToScene(SceneConstants.SCENE_MENU);
        },
        delegate (string failmsg) // failed to fetch an existing profile, need to create one
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
                    ProfileMgr.Instance.SavePlayerProfile(
                    delegate () // success
                    {
                        NotificationMgr.Instance.StopLoad();

                        // transit to menu screen
                        TransitMgr.Instance.FadeToScene(SceneConstants.SCENE_MENU);
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
            delegate () // cancelled - don't create profile, sign out
            {
                if (DatabaseMgr.Instance.IsLoggedIn)
                    DatabaseMgr.Instance.Logout();
            });
        });

    }

    private IEnumerator Autologin()
    {
        // Delay for sdks to initialise
        yield return new WaitForSeconds(0.5f);

        // If user has already logged in before
        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            bool verify = true;

            // Check login types
            foreach (string str in DatabaseMgr.Instance.LoginTypes)
            {
                if (str == LoginTypeConstants.FACEBOOK) // If user was authenticated by Facebook
                {
                    verify = false; // Don't check for a verified email
                    DatabaseMgr.Instance.SNSLogin(LoginTypeConstants.FACEBOOK, null, null, true); // sns login to facebook without actually going through auth
                }
                Debug.Log("Providerid: " + str);
            }

            // Transit to menu
            VerifyAndTransitToMenu(verify);
        }
    }
}
