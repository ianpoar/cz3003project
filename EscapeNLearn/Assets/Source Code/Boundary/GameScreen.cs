using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : Screen
{
    public GameMgr GameManager;
    public JoystickInput JSInput;
    public Image HpFill;
    public Text txt_HP;
    public GameObject pauseButton;
    public GameObject mask_npcquestions;
    public GameObject[] answerButtons;
    public Text txt_header;
    public Text txt_question;
    public Text txt_ans1;
    public Text txt_ans2;
    public Text txt_ans3;
    public Text txt_ans4;
    public GameObject mask_pause;
    public Text txt_timer;
    public Text txt_tips;

    protected override void Start()
    {
        base.Start();
        GameManager.StartGame(this);
    }

    // used by gamemgr
    public void PauseInput(bool flag)
    {
        JSInput.PauseInput(flag);
    }

    // used by gamemgr
    public void ShowNPCQuestion(string npcname, Question ques)
    {
        PauseInput(true);

        txt_header.text = npcname;
        txt_question.text = ques.question;
        txt_ans1.text = ques.answer1;
        txt_ans2.text = ques.answer2;
        txt_ans3.text = ques.answer3;
        txt_ans4.text = ques.answer4;

        mask_npcquestions.SetActive(true);
        foreach (GameObject obj in answerButtons)
        {
            obj.SetActive(true);
        }
    }

    // used by gamemgr
    public void ShowWrongAnswer()
    {
        txt_question.text = "Wrong answer! Please try again.";
        foreach (GameObject obj in answerButtons)
        {
            obj.SetActive(false);
        }
    }

    public void SetTips(string tips)
    {
        txt_tips.text = tips;
    }

    // used by gamemgr
    public void ShowResultsScreen(Report report)
    {
        pauseButton.SetActive(false);
        JSInput.PauseInput(true);

        NotificationMgr.Instance.Notify("You have cleared the level!",
         delegate ()
         {
             SessionMgr.Instance.LoadLevelReport(report);
             TransitMgr.Instance.FadeToScene("Report");
         });
    }

    // used by gamemgr
    public void ShowGameFailed(Report report)
    {
        pauseButton.SetActive(false);
        JSInput.PauseInput(true);
        NotificationMgr.Instance.Notify("You have failed the level!",
        delegate ()
        {
            SessionMgr.Instance.LoadLevelReport(report);
            TransitMgr.Instance.FadeToScene("Report");
        });

    }

    // used by gamemgr
    public void SetHP(int value, int maxhp)
    {
        float scale = (float)value / (float)maxhp;
        HpFill.rectTransform.localScale = new Vector3(scale, HpFill.rectTransform.localScale.y, HpFill.rectTransform.localScale.z);
        txt_HP.text = "" + value + " / " + maxhp;
    }

    // called by gamemgr
    public void SetTimer(int value)
    {
        txt_timer.text = "" + value;
    }

    // called by UI
    public void Btn_Pause(bool flag)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);

        if (flag)
            mask_pause.SetActive(true);
        else
            mask_pause.SetActive(false);

        JSInput.PauseInput(flag);
        GameManager.PauseGame(flag);
    }

    public void Btn_LeaveGame()
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        GameManager.SendReportToDB(); // even when leave game, send report to db
        TransitMgr.Instance.FadeToScene("Menu");
    }

    // called by UI
    public void AnswerQuestion(int choice)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        GameManager.AnswerQuestion(choice);
    }

    // called by UI
    public void Btn_CancelQuestion(bool playsound)
    {
        AudioMgr.Instance.PlaySFX(AudioConstants.SFX_CLICK);
        mask_npcquestions.SetActive(false);
        PauseInput(false);
    }

}
