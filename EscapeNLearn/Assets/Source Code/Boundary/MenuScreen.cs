using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Boundary Class for the Menu scene, handles all UI-related events in the scene, encapsulates SessionUI, QuestionUI, LevelSelectUI, SessionLobbyUI, SettingsUI, FriendsUI.
/// </summary>
public class MenuScreen : Screen
{
    // References
    [SerializeField]
    private GameObject panel_profileinfo;
    [SerializeField]
    private List<GameObject> objects_instructorOnly;
    [SerializeField]
    private GameObject panel_settings;
    [SerializeField]
    private GameObject panel_sessions;
    [SerializeField]
    public GameObject panel_sessionlobby;
    [SerializeField]
    private GameObject panel_levelselect;
    [SerializeField]
    private Image profilePic;
    [SerializeField]
    private GameObject panel_questionlist;
    [SerializeField]
    private GameObject panel_friends;

    [SerializeField]
    private GameObject panel_challenges;

    /// <summary>
    /// Start of the Menu screen.
    /// </summary>
    protected override void Start()
    {
        // Call parent start
        base.Start();

        // Play BGM
        AudioMgr.Instance.PlayBGM(AudioConstants.BGM_PERCEPTION);
    }

    /// <summary>
    /// An handler that executes actions only after profile data is fetched.
    /// </summary>
    protected override void StartAfterDataFetched()
    {
        CheckLoginDetails();
    }

    /// <summary>
    /// An handler for when the profile button is pressed, enables/disables the profile panel and its ProfileUI class.
    /// </summary>
    public void Btn_ShowProfile(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_profileinfo.SetActive(show);
    }

    /// <summary>
    /// An handler for when the settings button is pressed, enables/disables the settings panel and its SettingsUI class.
    /// </summary>
    public void Btn_ShowSettings(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_settings.SetActive(show);
    }

    /// <summary>
    /// An handler for when the view sessions button is pressed by an instructor, enables/disables the sessions panel and its SessionUI class.
    /// </summary>
    public void Btn_ShowSessions(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_sessions.SetActive(show);
    }

    /// <summary>
    /// An handler for when the play normal button is pressed, enables the session lobby panel and its SessionLobbyUI class, or the level select panel and its LevelSelectUI class, depending on whether the player has joined a session.
    /// </summary>
    public void Btn_PlayNormal(bool playsound)
    {
        if (playsound)
            AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (SessionMgr.Instance.currentConnection != null)
        {
            panel_levelselect.SetActive(true); // show level select
        }
        else
        {
            panel_sessionlobby.SetActive(true); // show session lobby
        }
    }

    /// <summary>
    /// An handler for when the play challenge button is pressed.
    /// </summary>
    public void Btn_PlayChallenge()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        NotificationMgr.Instance.Notify("This feature has not been implemented yet.");
    }

    /// <summary>
    /// An handler for when the view questions button is pressed by an instructor, enables/disables the questions panel and its QuestionUI class.
    /// </summary>
    public void Btn_QuestionList(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        panel_questionlist.SetActive(!panel_questionlist.activeSelf); // show create question
    }

    /// <summary>
    /// A method that checks the current player's login details and displays the appropriate information and UI objects.
    /// </summary>
    private void CheckLoginDetails()
    {
        // If user is logged in
        if (DatabaseMgr.Instance.IsLoggedIn)
        {
            // if user is an instructor
            if (ProfileMgr.Instance.localProfile.accountType == "Instructor")
            {
                // show instructor UI
                foreach (GameObject obj in objects_instructorOnly)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }
            }

            // if user has facebook linked
            List<string> loginTypes = DatabaseMgr.Instance.LoginTypes;
            if (loginTypes.Contains(LoginTypeConstants.FACEBOOK) && !loginTypes.Contains(LoginTypeConstants.GOOGLE)) // if facebook only, no google
            {
                // fetch profile pic
                DatabaseMgr.Instance.FetchProfilePic(ProfileMgr.Instance.localProfile.id_facebook,
                    delegate (Sprite sprite)
                    {
                        profilePic.sprite = sprite;
                    },
                    null);
            }
        }

    }

    /// <summary>
    /// A handler for when the friends button is pressed, enables/disables the friends panel and its FriendsUI class.
    /// </summary>
    public void Btn_ShowFriendsList(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_friends.SetActive(show);
    }

    /// <summary>
    /// A handler for when challenge mode button is pressed, enables/disables the challenges panel.
    /// </summary>
    public void Btn_ShowChallenges(bool show)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        panel_challenges.SetActive(show);
    }
}
