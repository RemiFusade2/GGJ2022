using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (rewiredPlayer.GetButtonDown(startInputName))
        {
            switch(currentScreen)
            {
                case SCREEN.TITLE:
                    currentScreen = SCREEN.LEVEL;
                    GameManager.instance.ResetGame();
                    LevelManager.instance.LoadFirstLevel();
                    GameManager.instance.ShowLevelStartScreen();
                    break;
                case SCREEN.LEVEL:
                    if (!GameManager.instance.gameIsRunning)
                    {
                        GameManager.instance.StartGame();
                    }
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

    public void DisplayTitleScreen()
    {
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
