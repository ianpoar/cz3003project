using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    public GameObject[] lockedUI;
    public Text Txt_SessionID;
    // Start is called before the first frame update
    private void OnEnable()
    {
        foreach (GameObject obj in lockedUI)
            obj.SetActive(true);

        Connection s = ProfileMgr.Instance.currentConnection;
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
        DatabaseMgr.Instance.DBUpdate(DBQueryConstants.QUERY_CONNECTIONS + "/" + ProfileMgr.Instance.connectionID, null,
            delegate () {
                ProfileMgr.Instance.currentConnection = null;
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

        Connection c = ProfileMgr.Instance.currentConnection;
   
        NotificationMgr.Instance.NotifyLoad("Fetching session reports");
        SessionMgr.Instance.LoadIndividualSessionReports(c,
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
        else TransitMgr.Instance.FadeToScene("Game_" + level);
    }
}
