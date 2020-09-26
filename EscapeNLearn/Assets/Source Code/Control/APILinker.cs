using Firebase.Auth;

public abstract class APILinker // Strategy class: abstract functionality can be changed at runtime
{
    public abstract void Authenticate(CredentialCallback successCallback, MessageCallback failCallback);
    public abstract void GetProfilePic(string id, SpriteCallback successCallback, MessageCallback failCallback);
}
