using System;

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