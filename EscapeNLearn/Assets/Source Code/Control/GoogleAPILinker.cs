using Firebase.Auth;

/// <summary>
/// A concrete class derived from APILinker, executes Google API code.
/// </summary>
public class GoogleAPILinker : APILinker
{
    /// <summary>
    /// Executes Google Authentication API code.
    /// </summary>
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

    /// <summary>
    /// Not implemented, as this is not supported.
    /// </summary>
    public override void GetProfilePic(string id, SpriteCallback successCallback, MessageCallback failCallback)
    {
        throw new System.NotImplementedException();
    }
}
