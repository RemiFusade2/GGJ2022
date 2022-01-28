using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum CYCLETYPE
{
    DAY,
    NIGHT
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings")]
    public float dayDuration;
    public float nightDuration;


    // Runtime
    private CYCLETYPE currentCycleType;
    private float cycleCurrentTimer;

    public int score { get; private set; }
    public int lives { get; private set; }
    public bool gameStarted { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameStarted = false;

        DayNightCycle[] items = FindObjectsOfType<DayNightCycle>();
        foreach (DayNightCycle item in items)
        {
            item.InitCycle(CYCLETYPE.DAY);
        }
    }

    private void FixedUpdate()
    {
        if (gameStarted)
        {
            cycleCurrentTimer -= Time.fixedDeltaTime;

            UIManager.instance.UpdateDayNightSliderValue(cycleCurrentTimer);

            if (cycleCurrentTimer <= 0)
            {
                switch (currentCycleType)
                {
                    case CYCLETYPE.DAY:
                        SwitchToNightTime();
                        break;
                    case CYCLETYPE.NIGHT:
                        SwitchToDayTime();
                        break;
                }
            }
        }
    }

    public void IncreaseScore(int scoreAdd)
    {
        score += scoreAdd;
        UIManager.instance.UpdateScoreValueText(score);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void FinishLevel()
    {
        StopGame();
    }

    public void LoseLife()
    {
        lives--;
        UIManager.instance.UpdateLivesValueText(lives);

        if (lives < 0)
        {
            GameOver();
        }
        else
        {
            StopGame();
        }
    }

    public void StartGame()
    {
        score = 0;
        lives = 3;

        UIManager.instance.UpdateScoreValueText(score);
        UIManager.instance.UpdateLivesValueText(lives);

        gameStarted = true;

        SwitchToDayTime();

        UIManager.instance.ShowStartGamePanel(false);
    }

    public void StopGame()
    {
        gameStarted = false;
        UIManager.instance.ShowStartGamePanel(true);
    }

    public void GameOver()
    {
        gameStarted = false;
        UIManager.instance.ShowStartGamePanel(true);
    }

    private void SwitchToNightTime()
    {
        currentCycleType = CYCLETYPE.NIGHT;
        cycleCurrentTimer = nightDuration;

        DayNightCycle[] items = FindObjectsOfType<DayNightCycle>();
        foreach (DayNightCycle item in items)
        {
            item.SwitchToNight();
        }
    }

    private void SwitchToDayTime()
    {
        currentCycleType = CYCLETYPE.DAY;
        cycleCurrentTimer = dayDuration;

        DayNightCycle[] items = FindObjectsOfType<DayNightCycle>();
        foreach (DayNightCycle item in items)
        {
            item.SwitchToDay();
        }
    }
}
