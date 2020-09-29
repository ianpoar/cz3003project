using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMgr : MonoBehaviour
{
    // reference
    public Color AddHPColor;
    public Color ReduceHpColor;
    public GameObject spawnTextPrefab;

    // protected, accessed by child classes
    protected int score = 0;
    protected int PlayerMaxHP = 100;
    protected int PlayerHP = 100;
    protected GameScreen ScreenRef;

    // private
    private int secondsElapsed = 0;
    private bool IsPaused = false;
    private List<Question> QuestionList = new List<Question>();
    private List<int> ShownQuestions = new List<int>();
    private int currQuesIterator = 0;
    string currNPCName;
    private int currNoOfQues = 0;

    public virtual void StartGame(GameScreen reference)
    {
        ScreenRef = reference;

        Question q1 = new Question();
        q1.question = "1 + 2 = ?";
        q1.answer1 = "1";
        q1.answer2 = "2";
        q1.answer3 = "3";
        q1.answer4 = "4";
        q1.correctanswer = 3;

        Question q2 = new Question();
        q2.question = "2 + 2 = ?";
        q2.answer1 = "1";
        q2.answer2 = "2";
        q2.answer3 = "3";
        q2.answer4 = "4";
        q2.correctanswer = 4;

        Question q3 = new Question();
        q3.question = "20 + 2 = ?";
        q3.answer1 = "1";
        q3.answer2 = "22";
        q3.answer3 = "3";
        q3.answer4 = "4";
        q3.correctanswer = 2;

        Question q4 = new Question();
        q4.question = "22 - 11 = ?";
        q4.answer1 = "11";
        q4.answer2 = "2";
        q4.answer3 = "3";
        q4.answer4 = "4";
        q4.correctanswer = 1;

        QuestionList.Add(q1);
        QuestionList.Add(q2);
        QuestionList.Add(q3);
        QuestionList.Add(q4);

        StartCoroutine(Timer());

    }
    public void PauseGame(bool flag)
    {
        IsPaused = flag;
    }

    protected void ShowQuestions(string npcname, int listnumber, int numberOfQuestions)
    {
        // randomly sort list
        for (int i = 0; i < QuestionList.Count; i++)
        {
            Question temp = QuestionList[i];
            int randomIndex = Random.Range(i, QuestionList.Count);
            QuestionList[i] = QuestionList[randomIndex];
            QuestionList[randomIndex] = temp;
        }

        currQuesIterator = 0;
        currNPCName = npcname;
        currNoOfQues = numberOfQuestions;

        if (QuestionList == null)
        {
            Debug.Log("null list");
            return;
        }

        Question ques = QuestionList[currQuesIterator];
        if (ques != null)
        {
            ScreenRef.ShowNPCQuestion(currNPCName, ques);
        }
        else
        {
            Debug.Log("null ques");
        }
    }

    protected virtual void QuestionsComplete()
    {
        ScreenRef.Btn_CancelQuestion(false);
    }

    public void AnswerQuestion(int choice)
    {
        Question ques = QuestionList[currQuesIterator];

        if (choice == ques.correctanswer)
        {
            currQuesIterator++;
            if (currQuesIterator < currNoOfQues && currQuesIterator < QuestionList.Count)
            {
                ques = QuestionList[currQuesIterator];
                if (ques != null)
                {
                    ScreenRef.ShowNPCQuestion(currNPCName, ques);
                }
                else
                {
                    // Ended
                    QuestionsComplete();
                }
            }
            else
            {
                // Ended
                QuestionsComplete();
            }
        }
        else
        {
            ScreenRef.ShowWrongAnswer();
        }
    }

    private IEnumerator Timer()
    {
        bool time = true;
        while (time)
        {
            yield return new WaitForSeconds(1);
            secondsElapsed += 1;
            ScreenRef.SetTimer(secondsElapsed);
        }
    }

    protected void GameEnd()
    {
        PauseGame(true);
        StopAllCoroutines();
        ScreenRef.ShowResultsScreen();
    }
}
