using UnityEngine;

public abstract class LoginAPI // Stategy class
{
    public abstract void Initialise();
    public abstract void Authenticate(SimpleCallback successCallback, MessageCallback failCallback);
    public abstract void GetProfilePic(Texture2DCallback successCallback, MessageCallback failCallback);
}
