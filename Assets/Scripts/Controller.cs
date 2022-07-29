using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public static Controller singleton;

    public static int p1Score = 0;
    public static int p2Score = 0;
    public static int p1Lives = 6;
    public static int p2Lives = 6;
    public static int levelNumber = 9;
    public static bool twoPlayers = false;

    public static ScoreStrc[] highScores = new ScoreStrc[5];

    public static bool Frozen = false;
    public static bool stopTimer = false;

    private float levelStartTime;
    private int p1PreLevelScore = 0;
    private int p2PreLevelScore = 0;
    private bool pausedByp1 = false;
    private bool pausedByp2 = false;
    [HideInInspector()]
    public LevelController currentLevel;

    public List<Ball> balls = new List<Ball>();

    [Header("References")]
    public LivesDisplay p1LivesDisplay;
    public LivesDisplay p2LivesDisplay;
    public Image timer;
    public Text player1Score;
    public Text player2Score;
    public Text levelNumberTxt;
    public Text middleText;
    public Canvas pauseCanvas;

    [Header("Prefabs")]
    public List<GameObject> levelPrefabs = new List<GameObject>();

    public GameObject popPre;
    public static GameObject statPopPre;
    public GameObject comboPre;
    public static GameObject statComboPre;

    [Header("Settings")]
    public float hitFreezeTime = 2f;

    public void P1AddScore(int val)
    {
        p1Score += val;
        player1Score.text = p1Score.ToString();
    }

    public void P2AddScore(int val)
    {
        p2Score += val;
        player2Score.text = p2Score.ToString();
    }

    void Awake()
    {
        singleton = this;

        middleText.text = "";

        if (!twoPlayers)
            p2Lives = 0;

        p1LivesDisplay.SetLives(p1Lives);
        p2LivesDisplay.SetLives(p2Lives);

        statPopPre = popPre;
        statComboPre = comboPre;

        pauseCanvas.enabled = false;

        SpawnLevel(levelNumber);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("p1Pause"))
        {
            if (pausedByp1 == true)
            {
                pausedByp1 = false;
                UnFreeze();
                pauseCanvas.enabled = false;
            }
            else if (pausedByp2 == false)
            {
                pausedByp1 = true;
                Freeze();
                Time.timeScale = 0;
                pauseCanvas.enabled = true;
            }
        }

        if (Input.GetButtonDown("p2Pause"))
        {
            if (pausedByp2 == true)
            {
                pausedByp2 = false;
                UnFreeze();
                pauseCanvas.enabled = false;
            }
            else if (pausedByp1 == false)
            {
                pausedByp2 = true;
                Freeze();
                Time.timeScale = 0;
                pauseCanvas.enabled = true;
            }
        }


        if (Frozen || stopTimer)
            return;

        float timeLeft = 1 - ((Time.time - levelStartTime) / currentLevel.levelTime);

        if (timeLeft <= 0)
        {
            LoseLife(true);
            LoseLife(true);
        }
        timer.fillAmount = timeLeft;
    }

    private void SpawnLevel(int number)
    {
        stopTimer = true;

        if (currentLevel != null)
            Destroy(currentLevel.gameObject);

        levelNumberTxt.text = number.ToString();

        number = ((number - 1) % levelPrefabs.Count) + 1;
        number = Mathf.Clamp(number, 1, levelPrefabs.Count);
        currentLevel = Instantiate(levelPrefabs[number - 1]).GetComponent<LevelController>();

        timer.fillAmount = 1;

        balls.Clear();
        balls.AddRange(currentLevel.balls);

        Freeze();

        StartCoroutine(LevelStartCountdown());
    }

    public IEnumerator LevelStartCountdown()
    {
        for (int i = 3; i > 0; --i)
        {
            if (pausedByp1 || pausedByp2)
                yield return new WaitWhile(() => (pausedByp1 || pausedByp2));
            middleText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1);
        }

        if (pausedByp1 || pausedByp2)
            yield return new WaitWhile(() => (pausedByp1 || pausedByp2));

        middleText.text = "GO!";

        levelStartTime = Time.time;
        UnFreeze();
        stopTimer = false;

        yield return new WaitForSecondsRealtime(1);
        middleText.text = "";
    }

    public void BallDestroyed()
    {
        if (balls.Count == 0)
        {
            StartCoroutine(EndLevel());
        }
    }

    public IEnumerator EndLevel()
    {
        Freeze();
        stopTimer = true;
        float remainingTime = currentLevel.levelTime - (Time.time - levelStartTime);
        float timeCounted = 0;

        int p1targetScore = p1Score + (int)(remainingTime * 2);
        int p1startScore = p1Score;

        int p2targetScore = p2Score + (int)(remainingTime * 2);
        int p2startScore = p2Score;

        float timeLeft = 1 - ((Time.time - levelStartTime) / currentLevel.levelTime);

        float lastTimeChecked = Time.time;

        while (timeCounted < remainingTime)
        {
            timeCounted += (Time.time - lastTimeChecked) * 50;
            lastTimeChecked = Time.time;
            if (timeCounted > remainingTime)
                timeCounted = remainingTime;

            timer.fillAmount = ((remainingTime - timeCounted) / currentLevel.levelTime);

            p1Score = p1startScore + (int)((p1targetScore - p1startScore) * (timeCounted / remainingTime));
            player1Score.text = p1Score.ToString();

            if (twoPlayers)
            {
                p2Score = p2startScore + (int)((p1targetScore - p1startScore) * (timeCounted / remainingTime));
                player2Score.text = p2Score.ToString();
            }


            yield return new WaitForEndOfFrame();
        }

        p1PreLevelScore = p1Score;
        p2PreLevelScore = p2Score;
        levelNumber += 1;
        SpawnLevel(levelNumber);
    }

    public void PlayerHit(bool p1)
    {
        StartCoroutine(LoseLife(p1));
    }

    public IEnumerator LoseLife(bool p1)
    {
        Freeze();

        yield return new WaitForSeconds(hitFreezeTime);

        if (p1)
        {
            p1Lives = Mathf.Max(0, p1Lives - 1);
            p1LivesDisplay.SetLives(p1Lives);
        }
        else
        {
            p2Lives = Mathf.Max(0, p2Lives - 1);
            p2LivesDisplay.SetLives(p2Lives);
        }

        if (p1Lives == 0 && (!twoPlayers || p2Lives == 0))
            GameOver();
        else
        {
            p1Score = p1PreLevelScore;
            player1Score.text = p1Score.ToString();
            p2Score = p2PreLevelScore;
            player2Score.text = p2Score.ToString();
            SpawnLevel(levelNumber);
        }
    }

    public void GameOver()
    {
        middleText.text = "Game Over";

        StartCoroutine(BackToMenu());

        if (twoPlayers)
            Save.NewScore("Multiplayer", Mathf.Max(p1Score, p2Score));
        else
            Save.NewScore("SinglePlayer", p1Score);
    }

    public IEnumerator BackToMenu()
    {
        yield return new WaitForSecondsRealtime(3);

        SceneManager.LoadScene("Menu");
    }

    public void PauseResume()
    {
        pausedByp1 = false;
        pausedByp2 = false;
        UnFreeze();
        pauseCanvas.enabled = false;
    }

    public void PauseGoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PauseExit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void Freeze()
    {
        foreach (Ball b in balls)
            b.Freeze();

        Player.player1.Freeze();
        if (twoPlayers)
            Player.player2.Freeze();

        Frozen = true;
    }

    public void UnFreeze()
    {
        Time.timeScale = 1;

        foreach (Ball b in balls)
            b.UnFreeze();

        Player.player1.UnFreeze();
        if (twoPlayers)
            Player.player2.UnFreeze();

        Frozen = false;
    }
}
