using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private InputField input_text = null;

    public void Notify(string content = null, SimpleCallback okCallback = null, SimpleCallback cancelCallback = null)
    {
        notificationList.Add(new Notification(NotifyType.Notice, content, okCallback, cancelCallback, null));
        if (!notificationShowing)
            ProcessNextNotification();
    }

    public void RequestTextInput(string content = null, MessageCallback inputTextCallback = null, SimpleCallback cancelCallback = null)
    {
        notificationList.Add(new Notification(NotifyType.RequestTextInput, content, null, cancelCallback, inputTextCallback));
        if (!notificationShowing)
            ProcessNextNotification();
    }

    public void NotifyLoad(string content = null, SimpleCallback okCallback = null)
    {
        notificationList.Add(new Notification(NotifyType.Load, content, okCallback, null, null));
        if (!notificationShowing)
            ProcessNextNotification();
    }

    public void Btn_OK()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        HideAll();

        if (currentNotification.type == NotifyType.Notice)
            currentNotification.okCallback?.Invoke();
        else
            currentNotification.textInputCallback?.Invoke(input_text.text);

        ProcessNextNotification();
    }

    public void Btn_Cancel()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        HideAll();

        currentNotification.cancelCallback?.Invoke();
        ProcessNextNotification();
    }

    public void StopLoad()
    {
        stopLoadIterator++;
    }

    private IEnumerator Sequence_Loading(string loadingText = "Loading")
    {
        bool animate = true;
        int i = 0;
        string[] texts = new string[4];
        texts[0] = loadingText + ".";
        texts[1] = loadingText + "..";
        texts[2] = loadingText + "...";
        texts[3] = loadingText + "....";
        int stopLoadDelay = 3;

        while (animate)
        {
            txt_content.text = texts[i];
            i++;
            if (i >= texts.Length)
                i = 0;

            if (stopLoadIterator > 0)
            {
                stopLoadDelay--;
                if (stopLoadDelay <= 0)
                {
                    animate = false;
                }
            }

            yield return new WaitForSeconds(textDelay);
        }


        HideAll();
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
                case NotifyType.RequestTextInput:
                    txt_content.text = currentNotification.text;
                    okButton.gameObject.SetActive(true);
                    if (currentNotification.cancelCallback != null)
                        cancelButton.gameObject.SetActive(true);

                    input_text.text = "";
                    input_text.gameObject.SetActive(true);
                    break;
            }
            notificationId++;
            notificationObjects.SetActive(true);
            notificationShowing = true;
        }
        else
        {
            notificationShowing = false;
        }
    }

    void HideAll()
    {
        if (cancelButton.gameObject.activeSelf)
            cancelButton.gameObject.SetActive(false);
        if (okButton.gameObject.activeSelf)
            okButton.gameObject.SetActive(false);
        if (input_text.gameObject.activeSelf)
            input_text.gameObject.SetActive(false);
        notificationObjects.SetActive(false);
    }
}
