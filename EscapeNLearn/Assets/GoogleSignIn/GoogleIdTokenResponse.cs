using System;

/// <summary>
/// Response object to exchanging the Google Auth Code with the Id Token
/// </summary>

[Serializable]
public class GoogleIdTokenResponse
{
    public string id_token;
    public string access_token;
}

[Serializable]
public class GoogleUniqueIdResponse
{
    public string sub;
}

[Serializable]
public class CombinedGoogleResponse
{
    public string id_token;
    public string uniqueid;
    public string access_token;
}