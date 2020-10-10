public enum NotifyType
{
    Load,
    TransparentLoad,
    Notice,
    RequestTextInput
}

/// <summary>
/// A notification Entity Class.
/// </summary>
public class Notification
{
    public SimpleCallback okCallback { get; private set; }
    public MessageCallback textInputCallback { get; private set; }
    public SimpleCallback cancelCallback { get; private set; }
    public NotifyType type { get; private set; }
    public string text { get; private set; }

    public Notification(NotifyType t, string tex, SimpleCallback ok, SimpleCallback cancel, MessageCallback input)
    {
        type = t;
        text = tex;
        okCallback = ok;
        cancelCallback = cancel;
        textInputCallback = input;
    }
}