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
    GAME_OVER,
    CREDITS
}

public class MainLogicManager : MonoBehaviour
{
    public static MainLogicManager instance;

    [Header("Input settings")]
    public int playerID;
    public string horizontalInputName;
    public string verticalInputName;
    public string startInputName;
    public string exitInputName;
    public string insertCoinInputName;
    public string nextLevelInputName;
    private Player rewiredPlayer;

    [Header("Runtime")]
    public SCREEN currentScreen;

    // Konami code (hardcoded)
    public bool konami1Up;
    public bool konami2Up;
    public bool konami3Down;
    public bool konami4Down;
    public bool konami5Left;
    public bool konami6Right;
    public bool konami7Left;
    public bool konami8Right;
    public bool konami9Coin;
    public bool konami10Start;
    [Space]
    public bool upRegistered;
    public bool downRegistered;
    public bool leftRegistered;
    public bool rightRegistered;
    
    private void DisplayCredits()
    {
        UIManager.instance.ShowCreditsPanel();
        Invoke("PlayGlitchAndDisplayTitleScreen", 6.0f);
    }

    private void PlayGlitchAndDisplayTitleScreen()
    {
        GameManager.instance.TriggerGlitchEffect();
        Invoke("HideCreditsAndShowTitle", 0.5f);
    }

    private void HideCreditsAndShowTitle()
    {
        DisplayTitleScreen();
    }

    private void RegisterInputForKonamiCode(DIRECTION dir, bool coin, bool start)
    {
        if (!konami10Start && konami9Coin && !coin && start)
        {
            // KONAMI CODE IS COMPLETE !
            // Display Credits
            konami1Up = false;
            konami2Up = false;
            konami3Down = false;
            konami4Down = false;
            konami5Left = false;
            konami6Right = false;
            konami7Left = false;
            konami8Right = false;
            konami9Coin = false;
            konami10Start = false;
            Invoke("DisplayCredits", 0.5f);
            GameManager.instance.TriggerGlitchEffect();
            currentScreen = SCREEN.CREDITS;
        }
        else if (!konami9Coin && konami8Right && !start && coin)
        {
            konami9Coin = true;
        }
        else if (!konami8Right && konami7Left && !coin && !start && dir == DIRECTION.RIGHT)
        {
            konami8Right = true;
        }
        else if (!konami7Left && konami6Right && !coin && !start && dir == DIRECTION.LEFT)
        {
            konami7Left = true;
        }
        else if (!konami6Right && konami5Left && !coin && !start && dir == DIRECTION.RIGHT)
        {
            konami6Right = true;
        }
        else if (!konami5Left && konami4Down && !coin && !start && dir == DIRECTION.LEFT)
        {
            konami5Left = true;
        }
        else if (!konami4Down && konami3Down && !coin && !start && dir == DIRECTION.DOWN)
        {
            konami4Down = true;
        }
        else if (!konami3Down && konami2Up && !coin && !start && dir == DIRECTION.DOWN)
        {
            konami3Down = true;
        }
        else if (!konami2Up && konami1Up && !coin && !start && dir == DIRECTION.UP)
        {
            konami2Up = true;
        }
        else if (!konami1Up && !coin && !start && dir == DIRECTION.UP)
        {
            konami1Up = true;
        }
        else
        {
            konami1Up = false;
            konami2Up = false;
            konami3Down = false;
            konami4Down = false;
            konami5Left = false;
            konami6Right = false;
            konami7Left = false;
            konami8Right = false;
            konami9Coin = false;
            konami10Start = false;
        }
    }



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
        bool ignoreNextStartInput = false;

        float verticalInput = rewiredPlayer.GetAxis(verticalInputName);
        float horizontalInput = rewiredPlayer.GetAxis(horizontalInputName);

        if (currentScreen == SCREEN.TITLE)
        {
            if (verticalInput > 0.8f && !upRegistered)
            {
                upRegistered = true;
                downRegistered = false;
                RegisterInputForKonamiCode(DIRECTION.UP, false, false);
            }
            if (verticalInput < -0.8f && !downRegistered)
            {
                downRegistered = true;
                upRegistered = false;
                RegisterInputForKonamiCode(DIRECTION.DOWN, false, false);
            }
            else if (verticalInput >= -0.5f && verticalInput <= 0.5f)
            {
                upRegistered = false;
                downRegistered = false;
            }

            if (horizontalInput > 0.8f && !rightRegistered)
            {
                rightRegistered = true;
                leftRegistered = false;
                RegisterInputForKonamiCode(DIRECTION.RIGHT, false, false);
            }
            if (horizontalInput < -0.8f && !leftRegistered)
            {
                leftRegistered = true;
                rightRegistered = false;
                RegisterInputForKonamiCode(DIRECTION.LEFT, false, false);
            }
            else if (horizontalInput >= -0.5f && horizontalInput <= 0.5f)
            {
                leftRegistered = false;
                rightRegistered = false;
            }
        }

               
        if (rewiredPlayer.GetButtonDown(nextLevelInputName))
        {
            currentScreen = SCREEN.LEVEL;
            LevelManager.instance.LoadNextLevel();
            StartLevelAfterDelay(2.0f);
        }

        if (rewiredPlayer.GetButtonDown(exitInputName))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (rewiredPlayer.GetButtonDown(insertCoinInputName))
        {
            if (currentScreen == SCREEN.TITLE)
            {
                RegisterInputForKonamiCode(DIRECTION.LEFT, true, false);
            }
            AudioManager.instance.PlayInsertCoinSFX();
            UIManager.instance.InsertOneCoin();
        }

        if (currentScreen == SCREEN.LEADERBOARD)
        {
            // leaderboard controls
            if (verticalInput > 0.1f && !upRegistered)
            {
                upRegistered = true;
                downRegistered = false;
                UIManager.instance.ActiveScoreEntrySwitchLetter(-1);
            }
            if (verticalInput < -0.1f && !downRegistered)
            {
                downRegistered = true;
                upRegistered = false;
                UIManager.instance.ActiveScoreEntrySwitchLetter(1);
            }
            else if (verticalInput >= -0.08f && verticalInput <= 0.08f)
            {
                upRegistered = false;
                downRegistered = false;
            }

            if (rewiredPlayer.GetButtonDown(startInputName))
            {
                if (LeaderboardManager.instance.activeScoreEntered)
                {
                    DisplayTitleScreen();
                    ignoreNextStartInput = true;
                }
                else
                {
                    UIManager.instance.ActiveScoreEntryConfirmLetter();
                }
            }
        }

        if (rewiredPlayer.GetButtonDown(startInputName) && !ignoreNextStartInput)
        {
            if (currentScreen == SCREEN.TITLE)
            {
                RegisterInputForKonamiCode(DIRECTION.LEFT, false, true);
            }
            switch (currentScreen)
            {
                case SCREEN.CREDITS:
                    // do nothing
                    break;
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
                    //DisplayTitleScreen();
                    break;
                default:
                    break;
            }
        }
    }

    public void StartLevelAfterDelay(float delay)
    {
        AudioManager.instance.PlayTitleMusic();
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
        AudioManager.instance.PlayTitleMusic();
        currentScreen = SCREEN.GAME_OVER;
        UIManager.instance.ShowGameOverScreen(successfullyFinishedTheGame, score);
    }
}
