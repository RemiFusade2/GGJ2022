using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("References - leaderboard")]
    public GameObject leaderboardPanel;
    public List<ScoreEntry> allScoreEntriesDisplay;
    private ScoreEntry activeScoreEntry;

    [Header("References - title screen")]
    public GameObject titlePanel;
    public Text titleText;
    public Text insertCoinText;

    [Header("References - game over panel")]
    public GameObject gameOverPanel;
    public Text gameOverText;
    public string gameOverStr = "GAME OVER";
    public string congratulationsStr = "CONGRATULATIONS";
    public Text gameOverScoreTxt;

    [Header("References - credits panel")]
    public GameObject creditsPanel;
    public Text thankYouText;

    [Header("References - level complete")]
    public GameObject levelCompletePanel;
    public Text levelCompleteTimeBonusText;
    public string timeBonusPrefix = "TIME BONUS: ";

    [Header("References - in game")]
    public GameObject startLevelPanel;
    public GameObject topInfoPanel;
    public Text levelTitleText;
    [Space]
    public Text scoreValueText;
    public Text keysValueText;
    public GameObject keysValueInfinitePanel;
    public GameObject infiniteKeysUnlockedPanel;
    public Slider livesValueSlider;
    public Slider dayNightCycleSlider;

    [Header("Settings")]
    public List<string> titlePool;

    private bool preventInsertCoinBlink;

    private int coins;

    private void Awake()
    {
        instance = this;
        coins = 0;
        preventInsertCoinBlink = false;
    }

    private void Start()
    {
        ShowInsertCoinText();
        ShowThankYouText();
    }

    #region Leaderboard

    public void DisplayScoreEntries(List<ScoreEntryData> scoreEntriesData, int activeRank)
    {
        activeScoreEntry = null;
        for (int rank = 1; rank <= scoreEntriesData.Count; rank++)
        {
            if (rank-1 < allScoreEntriesDisplay.Count)
            {
                ScoreEntryData data = scoreEntriesData[rank - 1];
                allScoreEntriesDisplay[rank - 1].Initialize(rank, data.score, data.name, (rank == activeRank));

                if (rank == activeRank)
                {
                    activeScoreEntry = allScoreEntriesDisplay[rank - 1];
                }
            }
        }
    }

    public void ActiveScoreEntrySwitchLetter(int delta)
    {
        if (activeScoreEntry != null)
        {
            activeScoreEntry.SwitchLetter(delta);
        }
    }
    public void ActiveScoreEntryConfirmLetter()
    {
        if (activeScoreEntry != null)
        {
            activeScoreEntry.ConfirmLetter();
        }
    }

    #endregion
    
    #region Insert Coin

    private void ShowInsertCoinText()
    {
        insertCoinText.enabled = true;
        Invoke("HideInsertCoinText", 1.0f);
    }
    private void HideInsertCoinText()
    {
        if (!preventInsertCoinBlink)
        {
            insertCoinText.enabled = false;
        }
        Invoke("ShowInsertCoinText", 0.5f);
    }

    private void StopPreventInsertCoinBlink()
    {
        preventInsertCoinBlink = false;
    }

    public bool IsThereEnoughCoinToPlayTheGame()
    {
        return coins > 0;
    }

    private void UpdateInsertCoinText()
    {
        if (coins == 0)
        {
            insertCoinText.text = "PLEASE INSERT COIN";
        }
        else if (coins == 1)
        {
            insertCoinText.text = "1 COIN";
        }
        else
        {
            insertCoinText.text = coins.ToString() + " COINS";
        }
    }

    public void UseOneCoin()
    {
        coins--;
        if (coins < 0)
            coins = 0;
        UpdateInsertCoinText();
    }

    public void InsertOneCoin()
    {
        coins++;
        UpdateInsertCoinText();

        preventInsertCoinBlink = true;
        insertCoinText.enabled = true;
        Invoke("StopPreventInsertCoinBlink", 5f);
    }

    #endregion

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
    public void UpdateKeysValueText(int newValue, bool infiniteKeys)
    {
        if (infiniteKeys)
        {
            keysValueInfinitePanel.SetActive(true);
            keysValueText.gameObject.SetActive(false);
        }
        else
        {
            keysValueInfinitePanel.SetActive(false);
            keysValueText.gameObject.SetActive(true);
            keysValueText.text = newValue.ToString();
        }

    }

    public void HideAllPanels()
    {
        titlePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        startLevelPanel.SetActive(false);
        topInfoPanel.SetActive(false);
        creditsPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
    }

    private void ShowThankYouText()
    {
        thankYouText.enabled = true;
        Invoke("HideThankYouText", 1.0f);
    }
    private void HideThankYouText()
    {
        thankYouText.enabled = false;
        Invoke("ShowThankYouText", 0.5f);
    }

    public void ShowCreditsPanel()
    {
        HideAllPanels();
        creditsPanel.SetActive(true);
    }

    public void ShowTopInfoPanel()
    {
        topInfoPanel.SetActive(true);
    }
    
    public void ShowStartLevelPanel(int levelIndex, int difficultyLevel)
    {
        HideAllPanels();
        startLevelPanel.SetActive(true);
        ShowTopInfoPanel();
        string levelName = "LEVEL " + (levelIndex + 1).ToString();
        levelName += (difficultyLevel >= 2) ? "+" : "";
        levelName += (difficultyLevel >= 3) ? "+" : "";
        levelTitleText.text = levelName;
    }
    public void ShowTitleScreen()
    {
        HideAllPanels();
        int randomIndex = Random.Range(0, titlePool.Count);
        titleText.text = titlePool[randomIndex];
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

    public void ShowLevelCompleteScreen(int timeBonus, bool infiniteKeysUnlocked)
    {
        HideAllPanels();
        ShowTopInfoPanel();
        levelCompleteTimeBonusText.text = timeBonusPrefix + timeBonus.ToString();
        infiniteKeysUnlockedPanel.SetActive(infiniteKeysUnlocked);
        levelCompletePanel.SetActive(true);
    }
}
