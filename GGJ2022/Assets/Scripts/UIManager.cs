using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("References - main screens")]
    public GameObject titlePanel;
    public GameObject leaderboardPanel;

    [Header("References - game over panel")]
    public GameObject gameOverPanel;
    public Text gameOverText;
    public string gameOverStr = "GAME OVER";
    public string congratulationsStr = "CONGRATULATIONS";
    public Text gameOverScoreTxt;

    [Header("References - in game")]
    public GameObject startLevelPanel;
    public GameObject topInfoPanel;
    public Text levelTitleText;
    [Space]
    public Text scoreValueText;
    public Text keysValueText;
    public Slider livesValueSlider;
    public Slider dayNightCycleSlider;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateDayNightSliderValue(float remaningTime)
    {
        dayNightCycleSlider.value = Mathf.CeilToInt(remaningTime);
    }

    public void UpdateScoreValueText(int newValue)
    {
        scoreValueText.text = newValue.ToString();
    }
    public void UpdateLivesValueText(int newValue)
    {
        livesValueSlider.value = newValue;
    }
    public void UpdateKeysValueText(int newValue)
    {
        keysValueText.text = newValue.ToString();
    }

    public void HideAllPanels()
    {
        titlePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        startLevelPanel.SetActive(false);
        topInfoPanel.SetActive(false);
    }

    public void ShowTopInfoPanel()
    {
        topInfoPanel.SetActive(true);
    }
    
    public void ShowStartLevelPanel(int levelIndex)
    {
        HideAllPanels();
        startLevelPanel.SetActive(true);
        ShowTopInfoPanel();
        string levelName = "LEVEL " + (levelIndex + 1).ToString();
        levelTitleText.text = levelName;
    }
    public void ShowTitleScreen()
    {
        HideAllPanels();
        titlePanel.SetActive(true);
    }
    public void ShowGameOverScreen(bool finishedGame, int score)
    {
        HideAllPanels();

        if (finishedGame)
        {
            gameOverText.text = congratulationsStr;
        }
        else
        {
            gameOverText.text = gameOverStr;
        }

        gameOverScoreTxt.text = "Score: " + score.ToString();

        //showTopInfoPanel();
        gameOverPanel.SetActive(true);
    }
    public void ShowLeaderboardScreen()
    {
        HideAllPanels();
        leaderboardPanel.SetActive(true);
    }
}
