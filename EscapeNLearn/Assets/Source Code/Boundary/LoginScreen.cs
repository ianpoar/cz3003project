using System.Collections;
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
        // no base.start() - don't refresh profile into here as there's autologin
        AudioMgr.Instance.PlayBGM(AudioConstants.BGM_CLEARDAY);
    }

    protected override void StartAfterDataFetched()
    {
        // Run autologin
        Autologin();
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

        // call api login and get credential
        DatabaseMgr.Instance.SNSRequestCredential(
        LoginTypeConstants.FACEBOOK,
        delegate (Firebase.Auth.Credential cred) // success
        {
            // with credential, login via firebase db
            DatabaseMgr.Instance.SNSLoginWithCredential(cred,
            delegate () // successful login to db
            {
                NotificationMgr.Instance.StopLoad();
                VerifyAndTransitToMenu(false); // don't verify email
            },
            delegate (string failmsg) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });

        },
        delegate (string failmsg) // failed
        {
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg);
        });
    }


    // GoogleLogin button pressed
    public void Btn_GoogleLogin()
    {
        // call api login and get credential
        DatabaseMgr.Instance.SNSRequestCredential(
        LoginTypeConstants.GOOGLE,
        delegate (Firebase.Auth.Credential cred) // success
        {
            // with credential, login via firebase db
            DatabaseMgr.Instance.SNSLoginWithCredential(cred,
            delegate () // successful login to db
            {
                NotificationMgr.Instance.StopLoad();
                VerifyAndTransitToMenu(false); // don't verify email
            },
            delegate (string failmsg) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });

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
            TransitMgr.Instance.FadeToScene("Menu");
        },
        delegate (string failmsg) // failed to fetch an existing profile, need to create one
        {
            PlayerPrefs.SetInt(PPConstants.BUGFIX_StoppedAppInProfileCreation, 1);
            NotificationMgr.Instance.StopLoad();
            NotificationMgr.Instance.Notify(failmsg + "\n\nCreate new profile?",
            delegate () // ok pressed
            {
                CreateProfileSequence();
            },
            delegate () // cancelled - don't create profile, sign out
            {
                PlayerPrefs.SetInt(PPConstants.BUGFIX_StoppedAppInProfileCreation, 0);
                if (DatabaseMgr.Instance.IsLoggedIn)
                    DatabaseMgr.Instance.Logout();
            });
        });

    }

    private bool IsAlphaNum(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        for (int i = 0; i < str.Length; i++)
        {
            if (!(char.IsLetter(str[i])) && (!(char.IsNumber(str[i]))))
                return false;
        }

        return true;
    }

    private void CreateProfileSequence()
    {
        NotificationMgr.Instance.RequestTextInput("Please enter a unique ID (min. 3, max 10 characters). Allowed characters are: [a-zA-Z0-9]",
                delegate (string input) // ok pressed
                {
                    // check that characters are valid

                    if (input.Length > 10 || input.Length < 3) // length more than 10 or less than 3
                    {
                        NotificationMgr.Instance.Notify("Please ensure that your ID is between 3 - 10 characters. Your ID \'" + input + "\' is not allowed", // notify
                        delegate () // on ok pressed
                        {
                            CreateProfileSequence(); // restart the profile creation process
                        }, null);
                    }
                    else if (!IsAlphaNum(input)) // input is not alpha numberic
                    {
                        NotificationMgr.Instance.Notify("Allowed characters are: [a-zA-Z0-9]. Your ID \'" + input + "\' is not allowed", // notify
                        delegate () // on ok pressed
                        {
                            CreateProfileSequence(); // restart the profile creation process
                        }, null);
                    }
                    else // no problems on input, now check for existing user id
                    {
                        NotificationMgr.Instance.NotifyLoad("Performing existing ID check");
                        // to be done
                        DatabaseMgr.Instance.DBFetchMulti(DBQueryConstants.QUERY_PROFILES, "id_account", input, 1,
                        delegate (string result) // exists
                        {
                            NotificationMgr.Instance.StopLoad();
                            NotificationMgr.Instance.Notify("ID \'" + input + "\' already exists.", // notify
                                delegate () // on ok pressed
                                {
                                    CreateProfileSequence(); // restart the profile creation process
                                }, null);
                        },
                        delegate (string result) // does not exist
                        {
                            NotificationMgr.Instance.StopLoad();
                            NotificationMgr.Instance.RequestTextInput("Please enter your name/nickname.",

                                delegate (string nameInput) // name entered
                                {
                                    // create profile
                                    ProfileMgr.Instance.localProfile.id_account = input; // set account id
                                    ProfileMgr.Instance.localProfile.name = nameInput; // set name

                                    NotificationMgr.Instance.NotifyLoad("Creating profile");
                                    ProfileMgr.Instance.SavePlayerProfile(
                                    delegate () // success
                                    {
                                        PlayerPrefs.SetInt(PPConstants.BUGFIX_StoppedAppInProfileCreation, 0);
                                        NotificationMgr.Instance.StopLoad();

                                        // transit to menu screen
                                        TransitMgr.Instance.FadeToScene("Menu");
                                    },
                                    delegate (string failmsg2) // failed to create profile
                                    {
                                        NotificationMgr.Instance.Notify(failmsg2);
                                    });
                                },
                                delegate () // cancelled
                                {
                                    CreateProfileSequence(); // restart the profile creation process
                                });
                        });
                    }
                },
                delegate () // cancelled - don't create profile, sign out
                {
                    PlayerPrefs.SetInt(PPConstants.BUGFIX_StoppedAppInProfileCreation, 0);
                    if (DatabaseMgr.Instance.IsLoggedIn)
                        DatabaseMgr.Instance.Logout();
                });
    }

    private void Autologin()
    {
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
                    DatabaseMgr.Instance.SNSRequestCredential(LoginTypeConstants.FACEBOOK, null, null, true); // sns login to facebook without actually going through auth
                }
                Debug.Log("Providerid: " + str);
            }

            // Transit to menu
            VerifyAndTransitToMenu(verify);
        }
    }
}
