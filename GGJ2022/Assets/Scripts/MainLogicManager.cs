using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SCREEN
{
    TITLE,
    LEADERBOARD,
    LEVEL,
    GAME_OVER
}

public class MainLogicManager : MonoBehaviour
{
    public static MainLogicManager instance;

    [Header("Input settings")]
    public int playerID;
    public string startInputName;
    public string exitInputName;
    public string insertCoinInputName;
    private Player rewiredPlayer;
    
    [Header("Runtime")]
    public SCREEN currentScreen;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DisplayTitleScreen();
        rewiredPlayer = ReInput.players.GetPlayer(playerID);
    }

    // Update is called once per frame
    void Update()
    {
        if (rewiredPlayer.GetButtonDown(exitInputName))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (rewiredPlayer.GetButtonDown(insertCoinInputName))
        {
            UIManager.instance.InsertOneCoin();
        }

        if (rewiredPlayer.GetButtonDown(startInputName))
        {
            switch(currentScreen)
            {
                case SCREEN.TITLE:
                    if (UIManager.instance.IsThereEnoughCoinToPlayTheGame())
                    {
                        UIManager.instance.UseOneCoin();
                        currentScreen = SCREEN.LEVEL;
                        GameManager.instance.ResetGame();
                        LevelManager.instance.LoadFirstLevel();
                        StartLevelAfterDelay(2.0f);
                    }
                    break;
                case SCREEN.LEVEL:
                    break;
                case SCREEN.GAME_OVER:
                    DisplayLeaderboard();
                    break;
                case SCREEN.LEADERBOARD:
                    DisplayTitleScreen();
                    break;
                default:
                    break;
            }
        }
    }

    public void StartLevelAfterDelay(float delay)
    {
        GameManager.instance.ShowLevelStartScreen();
        AudioManager.instance.PlayStartLevelSFX();
        StartCoroutine(WaitAndStartLevel(delay));
    }

    private IEnumerator WaitAndStartLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.PlayInGameMusic();
        GameManager.instance.StartGame();
    }

    public void DisplayTitleScreen()
    {
        AudioManager.instance.PlayTitleMusic();
        currentScreen = SCREEN.TITLE;
        UIManager.instance.ShowTitleScreen();
    }

    public void DisplayLeaderboard()
    {
        currentScreen = SCREEN.LEADERBOARD;
        UIManager.instance.ShowLeaderboardScreen();
    }

    public void GameOver(bool successfullyFinishedTheGame, int score)
    {
        currentScreen = SCREEN.GAME_OVER;
        UIManager.instance.ShowGameOverScreen(successfullyFinishedTheGame, score);
    }
}
