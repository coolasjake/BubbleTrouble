using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Text highScoresTxt;

    void Awake()
    {
        PrintScores();
    }

    private void PrintScores()
    {
        Controller.highScores = Save.GetScores();
        string list = "Scores:";
        for (int i = 0; i < Controller.highScores.Length; ++i)
            list += "\n" + Controller.highScores[i].name + " " + Controller.highScores[i].score;
        highScoresTxt.text = list;
    }

    public void StartSinglePlayer()
    {
        Controller.twoPlayers = false;
        Controller.levelNumber = 1;
        Controller.p1Lives = 6;
        Controller.p1Score = 0;
        Controller.p2Score = 0;
        SceneManager.LoadScene("Game");
    }

    public void StartMultiplayerPlayer()
    {
        Controller.twoPlayers = true;
        Controller.levelNumber = 1;
        Controller.p1Lives = 6;
        Controller.p2Lives = 6;
        Controller.p1Score = 0;
        Controller.p2Score = 0;
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
