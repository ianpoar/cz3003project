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
    void Start()
    {
        StartCoroutine(Autologin());
    }

    private IEnumerator Autologin()
    {
        yield return new WaitForSeconds(0.5f);
        // Check if user is already logged in
        if (DatabaseMgr.Instance.IsSignedIn)
        {
            VerifyAndTransitToMenu();
        }
    }

    public void Btn_ShowSignUp(bool flag)
    {
        panel_signup.SetActive(flag);
        input_signUpEmail.text = input_password1.text = input_password2.text = "";
    }

    public void Btn_Login()
    {
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

    private void VerifyAndTransitToMenu()
    {
        if (DatabaseMgr.Instance.IsEmailVerified) // check for verified email
        {
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
                    delegate() // ok pressed
                    {
                        // create profile
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
                    delegate() // cancel pressed
                    {
                        // nothing
                    });
                });
         }
        else
        {
            DatabaseMgr.Instance.SignOut();
            NotificationMgr.Instance.Notify("Before you can login, you must verify your email first by clicking on the link sent! Remember to check your junk folder.");
        }
       
    }
}
