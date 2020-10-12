using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI class encapsulated in MenuScreen in the Unity scene, handles UI functionality of the level select panel.
/// </summary>
public class LevelSelectUI : MonoBehaviour
{
    public GameObject[] lockedUI;
    public Text Txt_SessionID;
    // Start is called before the first frame update
    private void OnEnable()
    {
        foreach (GameObject obj in lockedUI)
            obj.SetActive(true);

        Connection s = SessionMgr.Instance.currentConnection;
        Txt_SessionID.text = s.session_name + "\nOwner: " + s.id_owner + "\nID: " + s.id_session;

        // unlock levels based on progress
        if (s.level_cleared > -1)
            lockedUI[0].SetActive(false);
        if (s.level_cleared > 0)
            lockedUI[1].SetActive(false);
        if (s.level_cleared > 1)
            lockedUI[2].SetActive(false);
    }

    public void Btn_LeaveSession()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        NotificationMgr.Instance.NotifyLoad("Leaving");
        SessionMgr.Instance.LeaveSession(
            delegate () {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify("Left the session.", delegate () { Btn_CloseLevelSelect(false); });
            },
            delegate (string fail)
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(fail);
            });
    }

    public void Btn_ViewCurrentSessionReport()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        NotificationMgr.Instance.NotifyLoad("Fetching Student Report");
        SessionMgr.Instance.LoadIndividualSessionReports(
            delegate ()
            {
                NotificationMgr.Instance.StopLoad();
                TransitMgr.Instance.FadeToScene("Report");
            },
            delegate (string failmsg)
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });
    }

    public void Btn_CloseLevelSelect(bool playsound)
    {
        if (playsound)
            AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        this.gameObject.SetActive(false);
    }

    public void SelectLevel(int level)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        if (level < 1 || level > 3)
        {
            Debug.Log("invalid level");
        }
        else
        {
            NotificationMgr.Instance.NotifyLoad("Loading Game Questions");
            string sessionid = SessionMgr.Instance.currentConnection.id_session;

            // fetch questions
            SessionMgr.Instance.FetchQuestionsForGame(sessionid, level,
            delegate () // success
            {
                NotificationMgr.Instance.StopLoad();
                TransitMgr.Instance.FadeToScene("Game_" + level);
            },
            delegate (string failmsg) // failed
            {
                NotificationMgr.Instance.StopLoad();
                NotificationMgr.Instance.Notify(failmsg);
            });

        }
    }
}
