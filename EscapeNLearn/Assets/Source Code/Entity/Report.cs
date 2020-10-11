using System;
using System.Collections.Generic;

/// <summary>
/// A report Entity Class.
/// </summary>
public class Report
{
    public int time_elapsed = 0;
    public int score = 0;
    public string id_session = "";
    public string id_player = "";
    public int game_level = 0;
    public int answer_count = 0;
    public AnswerList answers = new AnswerList();
}
