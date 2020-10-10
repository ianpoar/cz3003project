using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;

/// <summary>
/// A concrete class derived from APILinker, executes Facebook API code.
/// </summary>
public class FBAPILinker : APILinker
{
    /// <summary>
    /// Executes Facebook Authentication API code.
    /// </summary>
    public override void Authenticate(CredentialCallback successCallback, MessageCallback failCallback)
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

                    ProfileMgr.Instance.localProfile.id_facebook = AccessToken.CurrentAccessToken.UserId; // set facebook id
                    Credential credential = FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.TokenString);
                    successCallback?.Invoke(credential);
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

    /// <summary>
    /// Executes Facebook Get Profile Pic API code.
    /// </summary>
    public override void GetProfilePic(string id, SpriteCallback successCallback, MessageCallback failCallback)
    {
        ProfileMgr.Instance.StartCoroutine(fetchProfilePic(id, successCallback, failCallback));
    }

    /// <summary>
    /// Coroutine for GetProfilePic.
    /// </summary>
    private IEnumerator fetchProfilePic(string fbid, SpriteCallback successCallback, MessageCallback failCallback = null)
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        Debug.Log("Fetching profile pic of app-scoped fbid: " + fbid);
        using (WWW www = new WWW("http://graph.facebook.com/" + fbid + "/picture?width=400&height=400" ))
        {
            yield return www;
            www.LoadImageIntoTexture(tex);
            successCallback?.Invoke(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f)));
        }
    }
}
