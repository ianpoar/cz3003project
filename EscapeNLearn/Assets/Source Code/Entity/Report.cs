using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[Serializable]
public class Answer
{
    public string question;
    public string answer1;
    public string answer2;
    public string answer3;
    public string answer4;
    public int correctanswer;
    public int answer;
}

[Serializable]
public class AnswerList
{
    public List<Answer> list = new List<Answer>();
}

public class Report
{
    public int time_elapsed = 0;
    public int score = 0;
    public string id_session = "";
    public string id_account = "";
    public int game_level = 0;
    public int answer_count = 0;
    public AnswerList answers = new AnswerList();
}
