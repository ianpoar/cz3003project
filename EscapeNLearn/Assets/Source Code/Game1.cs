using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game1 : GameMgr
{
    public GameObject Player;
    public GameObject room1;
    public GameObject room2;

    private Vector3 savedPlayerpos;

    bool finalQuesList = false;

    public override void StartGame(GameScreen reference)
    {
        GameLevel = 1;
        base.StartGame(reference);
        // Play BGM
        AudioMgr.Instance.PlayBGM(AudioConstants.BGM_CLEARDAY);

        ScreenRef.SetHP(PlayerHP, PlayerMaxHP);
        ScreenRef.SetTips("Speak to the gem to recover HP.");
    }

    protected override void QuestionsComplete()
    {
        base.QuestionsComplete();

        if (finalQuesList)
        {
            GameClear();
        }
        else
        {
            // boost player hp
            PlayerHP += 10;
            if (PlayerHP >= PlayerMaxHP)
                PlayerHP = PlayerMaxHP;
            GameObject obj = Instantiate(spawnTextPrefab, Player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
            Fading3DText text = obj.GetComponent<Fading3DText>();
            text.script.text = "+ 10";
            text.script.color = AddHPColor;
            ScreenRef.SetHP(PlayerHP, PlayerMaxHP);
        }
    }

    private void FinalQuiz()
    {
        Debug.Log("final quiz sequence");
        finalQuesList = true;
        ShowQuestions("Locked Door", 0, 10); // currently only will have 1 qlist
    }

    private void GemQuiz()
    {
        Debug.Log("gem quiz");
        finalQuesList = false;
        ShowQuestions("Gem", 0, 1); // currently only will have 1 qlist
    }

    private void TransitToRoom2()
    {
        Debug.Log("rm 2");
        ScreenRef.PauseInput(true);
        TransitMgr.Instance.Fade(
        delegate ()
        {
            savedPlayerpos = Player.transform.localPosition;
            Player.transform.position = new Vector3(0, 0, 0);
            ScreenRef.SetTips("You are too weak to defeat this monster! Escape before it kills you.");
            room1.SetActive(false);
            room2.SetActive(true);
            TransitMgr.Instance.Emerge(
                delegate
                {
                    ScreenRef.PauseInput(false);
                });
        });

    }

    private void TransitToRoom1()
    {
        Debug.Log("rm 1");
        ScreenRef.PauseInput(true);
        TransitMgr.Instance.Fade(
        delegate ()
        {
            ScreenRef.SetTips("Speak to the gem to recover HP.");
            Player.transform.position = savedPlayerpos + new Vector3(0, -2, 0);
            room1.SetActive(true);
            room2.SetActive(false);
            TransitMgr.Instance.Emerge(
                delegate
                {
                    ScreenRef.PauseInput(false);
                });
        });
    }
}
