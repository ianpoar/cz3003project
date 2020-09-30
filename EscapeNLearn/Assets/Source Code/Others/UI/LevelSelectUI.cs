using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    public Text Txt_SessionID;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Txt_SessionID.text = ProfileMgr.Instance.currentConnection.id_session;
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
