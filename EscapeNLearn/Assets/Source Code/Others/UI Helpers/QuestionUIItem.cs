using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI helper class that provides functionalities for an question display object.
/// </summary>
public class QuestionUIItem : MonoBehaviour
{
    public Text txt_name;
    public Text txt_correctanswer;

    string name;
    string correctanswer;
    Question q;
    QuestionUI qui;

    public void Init(string correctanswer, string name, Question ql, QuestionUI qui)
    {
        txt_name.text = name;
        this.q = q;
        this.correctanswer = correctanswer;
        txt_correctanswer.text = "Correct option: " + correctanswer;

        this.qui = qui;
    }
    // Onclick event
    public void OnClick()
    {

    }
}
