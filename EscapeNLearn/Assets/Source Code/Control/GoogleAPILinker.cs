using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class GoogleAPILinker : APILinker
{
    public override void Authenticate(CredentialCallback successCallback, MessageCallback failCallback)
    {
        NotificationMgr.Instance.RequestTextInput("Paste your Google code here:",
           delegate (string text)
           {
               GoogleAuthHandler.ExchangeAuthCodeWithIdToken(text, idToken =>
               {
                   //GoogleAuthProvider.GetCredential(idToken, )
                   Debug.Log(idToken);
                   failCallback?.Invoke("tbc");
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
