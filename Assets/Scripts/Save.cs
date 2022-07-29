using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Save
{
    public static ScoreStrc[] GetScores()
    {
        string data = PlayerPrefs.GetString("highscores");
        string[] items = data.Split(',');

        ScoreStrc[] scores = new ScoreStrc[Mathf.Max(items.Length / 2, 5)];
        for (int i = 0; i < scores.Length; ++i)
            scores[i] = new ScoreStrc { name = "Barry B.", score = i * 100 };

        for (int i = 0; i < items.Length; i += 2)
        {
            if (i + 1 < items.Length)
                scores[4 - (i / 2)] = new ScoreStrc { name = items[i], score = int.Parse(items[i + 1]) };
        }

        return scores;
    }

    public static void SetScores(ScoreStrc[] value)
    {
        string data = string.Empty;
        foreach (ScoreStrc s in value)
        {
            data = s.score + "," + data;
            data = s.name + "," + data;
        }

        PlayerPrefs.SetString("highscores", data);
    }

    public static void NewScore(string Name, int value)
    {
        ScoreStrc[] scores = GetScores();
        List<ScoreStrc> scoresList = new List<ScoreStrc>(scores);
        scoresList.Add(new ScoreStrc { name = Name, score = value });

        ScoreStrc temp;

        for (int i = 0; i < scoresList.Count; ++i)
        {
            for (int j = 0; j < scoresList.Count - i - 1; ++j)
            {
                if (scoresList[j].score > scoresList[j + 1].score)
                {
                    temp = scoresList[j];
                    scoresList[j] = scoresList[j + 1];
                    scoresList[j + 1] = temp;
                }
            }
        }

        scoresList.RemoveAt(0);

        Save.SetScores(scoresList.ToArray());
    }
}
public struct ScoreStrc
{
    public string name;
    public int score;
}
