using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI helper class that provides functionalities for an question list display object.
/// </summary>
public class QuestionListUIItem : MonoBehaviour
{
    public Text txt_name;
    public Text txt_size;
    string name;
    string size;
    QuestionList ql;
    QuestionUI qui;

    public void Init(string size, string name, QuestionList ql, QuestionUI qui)
    {
        txt_name.text = name;
        this.ql = ql;
        this.size = size;
        txt_size.text = "Number of questions: " + size;
        
        this.qui = qui;
    }
    // Onclick event
    public void OnClick()
    {

    }
}
