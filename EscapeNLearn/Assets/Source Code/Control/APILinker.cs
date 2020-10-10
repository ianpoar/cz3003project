using Firebase.Auth;

/// <summary>
/// API Subsystem Interface, a Control Class that handles different SNS authentication APIs, implemented in a strategy pattern where functionality can be changed at runtime to facilitate different SNS authentication processes.
/// </summary>
public abstract class APILinker
{
    /// <summary>
    /// Executes SNS-dependent Authentication API code.
    /// </summary>
    public abstract void Authenticate(CredentialCallback successCallback, MessageCallback failCallback);
    /// <summary>
    /// Executes SNS-dependent Get Profile Picture API code.
    /// </summary>
    public abstract void GetProfilePic(string id, SpriteCallback successCallback, MessageCallback failCallback);
}
