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
    public int keys;
    public int lives;
    public bool gameIsRunning;

    private CYCLETYPE currentCycleType;
    private float cycleCurrentTimer;

    private PlayerController myPlayer;

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
        myPlayer = GameObject.FindObjectOfType<PlayerController>();
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
        keys = 0;
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
        myPlayer.StopPlayer();
        keys = 0;
        bool nextLevelLoaded = LevelManager.instance.LoadNextLevel();
        if (nextLevelLoaded)
        {
            ShowLevelStartScreen();
        }
    }

    public void LoseLife()
    {
        if (myPlayer == null)
        {
            Debug.LogError("you're kidding me");
        }
        myPlayer.StopPlayer();
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

    public void AddKey()
    {
        keys++;
        UIManager.instance.UpdateKeysValueText(keys);
    }
    public bool UseKey()
    {
        bool canUseKey = (keys > 0);
        if (canUseKey)
        {
            keys--;
            UIManager.instance.UpdateKeysValueText(keys);
        }
        return canUseKey;
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
