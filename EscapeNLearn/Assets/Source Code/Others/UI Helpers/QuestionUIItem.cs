using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionUIItem : MonoBehaviour
{
    public Text txt_name;
    public Text txt_correctanswer;

    string name;
    string correctanswer;
    Question q;
    QuestionUI qui;
    string key;

    public void Init(string key, string correctanswer, string name, Question q, QuestionUI qui)
    {
        this.key = key;
        txt_name.text = name;
        this.q = q;
        this.correctanswer = correctanswer;
        txt_correctanswer.text = "Correct option: " + correctanswer;

        this.qui = qui;
    }
    // Onclick event
    public void OnClick()
    {
        if (qui != null)
        {
            qui.EditQuestion(q, key);
        }
    }
}
