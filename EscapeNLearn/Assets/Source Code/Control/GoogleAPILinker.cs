using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Proyecto26;

public class GoogleAPILinker : APILinker
{

    public override void Authenticate(CredentialCallback successCallback, MessageCallback failCallback)
    {
        NotificationMgr.Instance.RequestTextInput("Paste your Google code here:",
           delegate (string text)
           {
               NotificationMgr.Instance.NotifyLoad("Authenticating");
               GoogleAuthHandler.ExchangeAuthCodeWithIdToken(text, final =>
               {
                   NotificationMgr.Instance.StopLoad();
                   ProfileMgr.Instance.localProfile.id_google = final.uniqueid; // set google id
                   Credential cred = GoogleAuthProvider.GetCredential(final.id_token, final.access_token);
                   successCallback?.Invoke(cred);
               });
           },
           delegate ()
           {
               failCallback?.Invoke("Cancelled");
           });

        GoogleAuthHandler.GetAuthCode();
    }

    public override void GetProfilePic(string id, SpriteCallback successCallback, MessageCallback failCallback)
    {
        throw new System.NotImplementedException();
    }
}
