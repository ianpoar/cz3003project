using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;

public class FacebookLoginAPI : LoginAPI
{
    public override void Authenticate(SimpleCallback successCallback, MessageCallback failCallback)
    {
        if (!FB.IsInitialized)
        {
            failCallback?.Invoke("The FB sdk is not initialised yet.");
            return;
        }

        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" },
        delegate (ILoginResult result)
        {
            bool error = false;
            string msg = "";
            if (result == null)
            {
                error = true;
                msg = "Null Result";
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Error))
                {
                    error = true;
                    msg = result.Error;
                }
                else if (result.Cancelled)
                {
                    error = true;
                    msg = "Cancelled Response:\n" + result.RawResult;
                }
                else if (!string.IsNullOrEmpty(result.RawResult)) // sucessful login, create credential and pass to DBMgr
                {
                    error = false;
                    msg = AccessToken.CurrentAccessToken.TokenString;
                    Credential credential = FacebookAuthProvider.GetCredential(msg);
                    DatabaseMgr.Instance.SNSAuth(credential, successCallback, failCallback, "Facebook");
                }
                else
                {
                    error = true;
                    msg = "Empty Response\n";
                }
            }

            if (error)
            {
                failCallback?.Invoke(msg);
            }

        });
    }

    public override void GetProfilePic(Texture2DCallback successCallback, MessageCallback failCallback)
    {
        FB.API("/me/picture?redirect=false", HttpMethod.GET, delegate (IGraphResult picResult)
        {
            if (string.IsNullOrEmpty(picResult.Error) && !picResult.Cancelled)
            {
                IDictionary data = picResult.ResultDictionary["data"] as IDictionary;
                string photoURL = data["url"] as string;

                DatabaseMgr.Instance.StartCoroutine(fetchProfilePic(photoURL, successCallback, failCallback));
            }
            else
            {
                failCallback?.Invoke("Error fetching pic.");
            }
        });
    }

    private IEnumerator fetchProfilePic(string url, Texture2DCallback successCallback, MessageCallback failCallback)
    {
        WWW www = new WWW(url);
        yield return www; //wait until it has downloaded
        if (www.error == null)
            successCallback?.Invoke(www.texture); // return Texture2D
        else
            failCallback?.Invoke(www.error);
    }

    public override void Initialise()
    {
        // init facebook sdk
        FB.Init(delegate ()
        {
            Debug.Log("FB.Init Success");
        },
        delegate (bool isGameShown)
        {

        });
    }
}
