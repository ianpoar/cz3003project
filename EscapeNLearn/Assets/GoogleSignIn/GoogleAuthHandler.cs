using System;
using Proyecto26;
using UnityEngine;

public class GoogleAuthHandler
{
    private const string ClientId = "900118418800-cfcpn1bebpr8ju8ojfjkstc5svahr822.apps.googleusercontent.com"; //TODO: Change [CLIENT_ID] to your CLIENT_ID
    private const string ClientSecret = "WqrYpcCenrlXuB7vkcrKdCP0"; //TODO: Change [CLIENT_SECRET] to your CLIENT_SECRET

    /// <summary>
    /// Opens a webpage that prompts the user to sign in and copy the auth code
    /// </summary>
    public static void GetAuthCode()
    {
        Application.OpenURL($"https://accounts.google.com/o/oauth2/v2/auth?client_id={ClientId}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&response_type=code&scope=email");
    }

    /// <summary>
    /// Exchanges the Auth Code with the user's Id Token
    /// </summary>
    /// <param name="code"> Auth Code </param>
    /// <param name="callback"> What to do after this is successfully executed </param>
    public static void ExchangeAuthCodeWithIdToken(string code, Action<CombinedGoogleResponse> callback)
    {
        RestClient.Post($"https://oauth2.googleapis.com/token?code={code}&client_id={ClientId}&client_secret={ClientSecret}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&grant_type=authorization_code", null).Then(
            response => {
                //Debug.Log(response.Text);
                var data = StringSerializationAPI.Deserialize(typeof(GoogleIdTokenResponse), response.Text) as GoogleIdTokenResponse;

                RestClient.Post($"https://oauth2.googleapis.com/tokeninfo?id_token=" + data.id_token, null).Then(
                        response2 => {
                            //Debug.Log(response2.Text);
                            var data2 = StringSerializationAPI.Deserialize(typeof(GoogleUniqueIdResponse), response2.Text) as GoogleUniqueIdResponse;
                            CombinedGoogleResponse final = new CombinedGoogleResponse();
                            final.id_token = data.id_token;
                            final.uniqueid = data2.sub;
                            final.access_token = data.access_token;
                            callback(final);
                        }).Catch(Debug.Log);
            }).Catch(Debug.Log);
    }
}
