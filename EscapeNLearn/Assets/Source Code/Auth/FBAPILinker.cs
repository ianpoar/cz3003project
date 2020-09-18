using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;

public class FBAPILinker : APILinker
{
    public override void Authenticate(SimpleCallback successCallback, MessageCallback failCallback)
    {
        if (!FB.IsInitialized)
        {
            failCallback?.Invoke("The FB sdk is not initialised yet.");
            // init facebook sdk
            FB.Init(delegate ()
            {
                Debug.Log("FB.Init Success");
            });
            return;
        }

        FB.LogInWithReadPermissions(new List<string>() { "email" },
        delegate (ILoginResult result)
        {
            bool error = false;
            string msg = "";

            if (result == null) // null result error
            {
                error = true;
                msg = "Null Result";
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Error)) // error exists
                {
                    error = true;
                    msg = result.Error;
                }
                else if (result.Cancelled) // login cancelled
                {
                    error = true;
                    msg = "Cancelled Response:\n" + result.RawResult;
                }
                else if (!string.IsNullOrEmpty(result.RawResult)) // sucessful login, connect to firebase
                {
                    error = false;

                    Credential credential = FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.TokenString);
                    LoginToDB(credential, successCallback, failCallback);
                }
                else // rawresult empty
                {
                    error = true;
                    msg = "Empty RawResult\n";
                }
            }

            if (error) // if error encountered
            {
                failCallback?.Invoke(msg); // invoke failure method
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

                ProfileMgr.Instance.StartCoroutine(fetchProfilePic(photoURL, successCallback, failCallback));
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
}
