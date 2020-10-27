/// <summary>
/// A challenges Entity Class.
/// </summary>
public class Challenges
{
    public int level_cleared;
    public string session_id;
    public string sender_id;

    public string receiver_id;

    public Challenges(string session, int level, string sender, string receiver)
    {
        level_cleared = level;
        session_id = session;
        sender_id = sender;
        receiver_id = receiver;
    }


}