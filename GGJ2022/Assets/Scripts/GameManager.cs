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
    [Header("Runtime")]
    public int score;
    public int lives;
    public bool gameIsRunning;

    public CYCLETYPE currentCycleType;
    private float cycleCurrentTimer;


    private void Awake()
    {
        instance = this;
        gameIsRunning = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        DayNightCycle[] items = FindObjectsOfType<DayNightCycle>();
        foreach (DayNightCycle item in items)
        {
            item.InitCycle(CYCLETYPE.DAY);
        }
    }

    private void FixedUpdate()
    {
        if (gameIsRunning)
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

    public void ResetGame()
    {
        score = 0;
        lives = 3;
        UIManager.instance.UpdateScoreValueText(score);
        UIManager.instance.UpdateLivesValueText(lives);
    }

    public void IncreaseScore(int scoreAdd)
    {
        score += scoreAdd;
        UIManager.instance.UpdateScoreValueText(score);
    }

    public void ShowLevelStartScreen()
    {
        gameIsRunning = false;
        SwitchToDayTime();
        UIManager.instance.ShowStartLevelPanel(LevelManager.instance.currentLevelIndex);
        UIManager.instance.UpdateDayNightSliderValue(cycleCurrentTimer);
    }

    public void StartGame()
    {
        UIManager.instance.HideAllPanels();
        UIManager.instance.ShowTopInfoPanel();
        SwitchToDayTime();
        UIManager.instance.UpdateDayNightSliderValue(cycleCurrentTimer);
        gameIsRunning = true;
    }

    public void FinishLevel()
    {
        LevelManager.instance.LoadNextLevel();
        UIManager.instance.ShowStartLevelPanel(LevelManager.instance.currentLevelIndex);
    }

    public void LoseLife()
    {
        Debug.Log("lose life. Lives remaining: " + lives);
        lives--;
        if (lives <= 0)
        {
            gameIsRunning = false;
            MainLogicManager.instance.GameOver();
        }
        else
        {
            UIManager.instance.UpdateLivesValueText(lives);
            LevelManager.instance.ReloadLevel();
            ShowLevelStartScreen();
        }
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
