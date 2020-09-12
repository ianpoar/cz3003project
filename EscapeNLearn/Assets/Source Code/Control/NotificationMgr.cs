using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NotifyType
{
    Load,
    Notice
}

public class Notification
{
    public SimpleCallback okCallback { get; private set; }
    public SimpleCallback cancelCallback { get; private set; }
    public NotifyType type { get; private set; }
    public string text { get; private set; }

    public Notification(NotifyType t, string tex, SimpleCallback ok, SimpleCallback cancel)
    {
        type = t;
        text = tex;
        okCallback = ok;
        cancelCallback = cancel;
    }
}

public class NotificationMgr : MonoBehaviour // Singleton class
{
    // Singleton implementation
    public static NotificationMgr Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Private variables
    private List<Notification> notificationList = new List<Notification>();
    int stopLoadIterator = 0;
    private bool notificationShowing = false;
    private int notificationId = 0;
    private Notification currentNotification = null;
    private float textDelay = 0.1f;

    // Inspector references
    [SerializeField]
    private Text txt_content = null;
    [SerializeField]
    private GameObject notificationObjects = null;
    [SerializeField]
    private Button okButton = null;
    [SerializeField]
    private Button cancelButton = null;

    public void Notify(string content = null, SimpleCallback okCallback = null, SimpleCallback cancelCallback = null)
    {
        notificationList.Add(new Notification(NotifyType.Notice, content, okCallback, cancelCallback));
        if (!notificationShowing)
            ProcessNextNotification();
    }

    public void NotifyLoad(string content = null, SimpleCallback okCallback = null)
    {
        notificationList.Add(new Notification(NotifyType.Load, content, okCallback, null));
        if (!notificationShowing)
            ProcessNextNotification();
    }

    public void Btn_OK()
    {
        if (cancelButton.gameObject.activeSelf)
            cancelButton.gameObject.SetActive(false);
        if (okButton.gameObject.activeSelf)
            okButton.gameObject.SetActive(false);
        notificationObjects.SetActive(false);

        notificationShowing = false;

        currentNotification.okCallback?.Invoke();

        ProcessNextNotification();
    }

    public void Btn_Cancel()
    {
        if (cancelButton.gameObject.activeSelf)
            cancelButton.gameObject.SetActive(false);
        if (okButton.gameObject.activeSelf)
            okButton.gameObject.SetActive(false);
        notificationObjects.SetActive(false);

        notificationShowing = false;

        currentNotification.cancelCallback?.Invoke();

        ProcessNextNotification();
    }

    public void StopLoad()
    {
        stopLoadIterator++;
    }

    private IEnumerator Sequence_Loading(string loadingText = "Loading")
    {
        notificationShowing = true;
        int i = 0;
        string[] texts = new string[4];
        texts[0] = loadingText + ".";
        texts[1] = loadingText + "..";
        texts[2] = loadingText + "...";
        texts[3] = loadingText + "....";
        float stopLoadDelay = 0.5f;

        while (notificationShowing)
        {
            txt_content.text = texts[i];
            i++;
            if (i >= texts.Length)
                i = 0;

            if (stopLoadIterator != 0)
            {
                stopLoadDelay -= textDelay * 500 * Time.deltaTime;
                if (stopLoadDelay <= 0)
                {
                    notificationShowing = false;
                }
            }

            yield return new WaitForSeconds(textDelay);
        }

        if (cancelButton.gameObject.activeSelf)
            cancelButton.gameObject.SetActive(false);
        if (okButton.gameObject.activeSelf)
            okButton.gameObject.SetActive(false);
        notificationObjects.SetActive(false);
        notificationShowing = false;
        stopLoadIterator--;
        currentNotification.okCallback?.Invoke();
        ProcessNextNotification();
    }

    private void ProcessNextNotification()
    {
        if (notificationId < notificationList.Count)
        {
            currentNotification = notificationList[notificationId];

            switch (currentNotification.type)
            {
                case NotifyType.Load:
                    if (currentNotification.text != null)
                    {
                        StartCoroutine(Sequence_Loading(currentNotification.text));
                    }
                    else
                    {
                        StartCoroutine(Sequence_Loading());
                    }
                    break;
                case NotifyType.Notice:
                    txt_content.text = currentNotification.text;
                        okButton.gameObject.SetActive(true);
                    if (currentNotification.cancelCallback != null)
                        cancelButton.gameObject.SetActive(true);
                    break;
            }
            notificationId++;
            notificationObjects.SetActive(true);
            notificationShowing = true;
        }
    }

    
}
