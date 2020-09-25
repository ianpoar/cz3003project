using Firebase.Auth;

public abstract class APILinker // Strategy class: abstract functionality can be changed at runtime
{
    public abstract void Authenticate(SimpleCallback successCallback, MessageCallback failCallback);
    public abstract void GetProfilePic(string id, SpriteCallback successCallback, MessageCallback failCallback);

    // Non - override methods that retain the same functionality for all 
    protected void LoginToDB(Credential credential, SimpleCallback successCallback, MessageCallback failCallback)
    {
        DatabaseMgr.Instance.ProcessSNSLogin(credential, successCallback, failCallback);
    }
}
