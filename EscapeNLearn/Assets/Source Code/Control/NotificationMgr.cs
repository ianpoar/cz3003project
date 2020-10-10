using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Notification Subsystem Interface, a Control Class that handles all in-game popup and loading notifications.
/// </summary>
public class NotificationMgr : MonoBehaviour // Singleton class
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
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
    private GameObject obj_default;
    [SerializeField]
    private GameObject obj_transparent;
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

    /// <summary>
    /// Displays an in-game popup notification.
    /// </summary>
    public void Notify(string content = null, SimpleCallback okCallback = null, SimpleCallback cancelCallback = null)
    {
        notificationList.Add(new Notification(NotifyType.Notice, content, okCallback, cancelCallback, null));
        if (!notificationShowing)
            ProcessNextNotification();
    }

    /// <summary>
    /// Displays an in-game popup notification and requests for a text input from the user.
    /// </summary>
    public void RequestTextInput(string content = null, MessageCallback inputTextCallback = null, SimpleCallback cancelCallback = null)
    {
        notificationList.Add(new Notification(NotifyType.RequestTextInput, content, null, cancelCallback, inputTextCallback));
        if (!notificationShowing)
            ProcessNextNotification();
    }

    /// <summary>
    /// Displays an in-game loading notification.
    /// </summary>
    public void NotifyLoad(string content = null, SimpleCallback okCallback = null)
    {
        notificationList.Add(new Notification(NotifyType.Load, content, okCallback, null, null));
        if (!notificationShowing)
            ProcessNextNotification();
    }

    /// <summary>
    /// Block all in-game inputs without displaying any notification.
    /// </summary>
    public void TransparentLoad(SimpleCallback okCallback = null)
    {
        notificationList.Add(new Notification(NotifyType.TransparentLoad, null, okCallback, null, null));
        if (!notificationShowing)
            ProcessNextNotification();
    }

    /// <summary>
    /// OK button handler.
    /// </summary>
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

    /// <summary>
    /// Cancel button handler.
    /// </summary>
    public void Btn_Cancel()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        HideAll();

        currentNotification.cancelCallback?.Invoke();
        ProcessNextNotification();
    }

    /// <summary>
    /// Stops loading notification from being displayed.
    /// </summary>
    public void StopLoad()
    {
        stopLoadIterator++;
    }

    /// <summary>
    /// Loading coroutine.
    /// </summary>
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

    /// <summary>
    /// Transparent load coroutine.
    /// </summary>
    private IEnumerator Sequence_TransparentLoading()
    {
        bool animate = true;

        while (animate)
        {
            if (stopLoadIterator > 0)
            {
               animate = false;
            }

            yield return new WaitForSeconds(0);
        }

        HideAll();
        stopLoadIterator--;
        currentNotification.okCallback?.Invoke();
        ProcessNextNotification();
    }

    /// <summary>
    /// Processes the next notification in the list, if any.
    /// </summary>
    private void ProcessNextNotification()
    {
        if (notificationId < notificationList.Count)
        {
            currentNotification = notificationList[notificationId];

            switch (currentNotification.type)
            {
                case NotifyType.Load:
                    obj_default.SetActive(true);
                    if (currentNotification.text != null)
                    {
                        StartCoroutine(Sequence_Loading(currentNotification.text));
                    }
                    else
                    {
                        StartCoroutine(Sequence_Loading());
                    }
                    break;
                case NotifyType.TransparentLoad:
                    obj_transparent.SetActive(true);
                    StartCoroutine(Sequence_TransparentLoading());
                    break;
                case NotifyType.Notice:
                    obj_default.SetActive(true);
                    txt_content.text = currentNotification.text;
                    okButton.gameObject.SetActive(true);
                    if (currentNotification.cancelCallback != null)
                        cancelButton.gameObject.SetActive(true);
                    break;
                case NotifyType.RequestTextInput:
                    obj_default.SetActive(true);
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

    /// <summary>
    /// Hide all notification displays.
    /// </summary>
    void HideAll()
    {
        if (obj_default.activeSelf)
            obj_default.SetActive(false);
        if (obj_transparent.activeSelf)
            obj_transparent.SetActive(false);
        if (cancelButton.gameObject.activeSelf)
            cancelButton.gameObject.SetActive(false);
        if (okButton.gameObject.activeSelf)
            okButton.gameObject.SetActive(false);
        if (input_text.gameObject.activeSelf)
            input_text.gameObject.SetActive(false);
        notificationObjects.SetActive(false);
    }
}
