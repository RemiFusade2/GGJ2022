using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Space]
    public string scrollingStaticFloatName;
    public string imageDistorsionFloatName;
    public string timeOffsetFloatName;
    public string scrollingStaticRGBVectorName;
    [Space]
    public float timeBonusLifespan;
    [Space]
    public int maxLives = 2;

    [Header("References")]
    public Material screenMaterial;
    public Animator animator;

    [Header("Prefabs")]
    public GameObject scorePrefab;
    [Space]
    public GameObject completionBonusPrefab;
    public GameObject extralifePrefab;

    // Runtime
    [Header("Runtime")]
    public int score;
    private int currentLevelScore;
    public int keys;
    public bool infiniteKeys;
    public int lives;
    public bool gameIsRunning;

    public CYCLETYPE currentCycleType;
    private float cycleCurrentTimer;
    private float currentGameTime;

    private PlayerController myPlayer;


    // Completion Bonus
    private bool currentCompletionBonusWasSpawned;

    // Time Bonus
    private int timeBonusPoints;

    // Keys secret bonus
    public bool keySecretWillUnlock;
    


    private void Awake()
    {
        instance = this;
        gameIsRunning = false;
        glitchEffectIsRunning = false;
        keySecretWillUnlock = true;
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
        StopGlitchEffect();
    }

    private void FixedUpdate()
    {
        if (gameIsRunning)
        {
            cycleCurrentTimer -= Time.fixedDeltaTime;
            currentGameTime += Time.fixedDeltaTime;


            UIManager.instance.UpdateDayNightSliderValue(cycleCurrentTimer);

            if (cycleCurrentTimer <= 0.5f && !glitchEffectIsRunning)
            {
                TriggerGlitchEffect();
            }
            else if (cycleCurrentTimer <= 0)
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
        keySecretWillUnlock = true;
        score = 0;
        currentLevelScore = 0;
        keys = 0;
        lives = maxLives;
        currentGameTime = 0;
        timeBonusPoints = 0;
        currentCompletionBonusWasSpawned = false;
        infiniteKeys = false;
        UIManager.instance.UpdateKeysValueText(keys, infiniteKeys);
        UIManager.instance.UpdateScoreValueText(score);
        UIManager.instance.UpdateLivesValueText(lives);
    }

    public void IncreaseScore(int scoreAdd, Vector3 itemPosition)
    {
        score += scoreAdd;
        currentLevelScore += scoreAdd;
        UIManager.instance.UpdateScoreValueText(score);
        GameObject scoreObj = Instantiate(scorePrefab, itemPosition, scorePrefab.transform.rotation);
        scoreObj.GetComponent<ScoreBehaviour>().Initialize(new Vector2(itemPosition.x, itemPosition.y), scoreAdd);
    }

    public void AddExtraLife()
    {
        if (lives < maxLives)
        {
            lives++;
            UIManager.instance.UpdateLivesValueText(lives);
        }
    }

    public void ShowLevelStartScreen()
    {
        gameIsRunning = false;
        SwitchToDayTime();
        UIManager.instance.UpdateDayNightSliderValue(cycleCurrentTimer);
        UIManager.instance.ShowStartLevelPanel(LevelManager.instance.currentLevelIndex, LevelManager.instance.GameDifficultyLevel);
    }

    public void StartGame()
    {
        UIManager.instance.HideAllPanels();
        UIManager.instance.ShowTopInfoPanel();
        SwitchToDayTime();
        UIManager.instance.UpdateDayNightSliderValue(cycleCurrentTimer);

        gameIsRunning = true;
        currentLevelScore = 0;
        currentGameTime = 0;
        timeBonusPoints = 0;
        currentCompletionBonusWasSpawned = false;
    }

    public void TrySpawnCompletionBonus()
    {
        Invoke("InvokeTrySpawnCompletionBonus", 0.1f);
    }

    private void InvokeTrySpawnCompletionBonus()
    {
        int sumOfAllCollectibleItems = GameObject.FindGameObjectsWithTag("Collectible").Length + GameObject.FindGameObjectsWithTag("Monster").Length;
        
        if (sumOfAllCollectibleItems == 0 && !currentCompletionBonusWasSpawned)
        {
            SpawnCompletionBonus();
            currentCompletionBonusWasSpawned = true;
        }
    }

    private void SpawnCompletionBonus()
    {
        if (LevelManager.instance.coalGo != null)
        {
            Transform parent = LevelManager.instance.currentLevelGameObject.transform;
            LevelManager.instance.RemoveCoal();

            if (lives < maxLives)
            {
                // spawn extra life
                GameObject currentCompletionBonus = Instantiate(extralifePrefab, LevelManager.instance.coalPosition, completionBonusPrefab.transform.rotation, parent);
            }
            else
            {
                // spawn completion bonus (diamond)
                GameObject currentCompletionBonus = Instantiate(completionBonusPrefab, LevelManager.instance.coalPosition, completionBonusPrefab.transform.rotation, parent);
            }
        }
    }

    public void FinishLevel()
    {
        myPlayer.MakePlayerMoveRight();
        gameIsRunning = false;

        // set time bonus
        timeBonusPoints = (600 - Mathf.CeilToInt(currentGameTime * 10));
        timeBonusPoints = (timeBonusPoints <= 0) ? 0 : timeBonusPoints;

        if (!LevelManager.instance.WereAllKeysCollected())
        {
            keySecretWillUnlock = false;
        }

        Invoke("FinishLevelShowLevelComplete", 0.6f);
    }

    private void FinishLevelShowLevelComplete()
    {

        currentLevelScore = 0;
        score += timeBonusPoints;
        UIManager.instance.UpdateScoreValueText(score);

        bool unlockScretInfiniteKeys = LevelManager.instance.IsEndOfWorld1();

        infiniteKeys = infiniteKeys || (unlockScretInfiniteKeys && keySecretWillUnlock);
        keys = 0;
        UIManager.instance.UpdateKeysValueText(keys, infiniteKeys);

        UIManager.instance.ShowLevelCompleteScreen(timeBonusPoints, unlockScretInfiniteKeys && keySecretWillUnlock);

        Invoke("FinishLevelForReal", 2.0f);
    }

    private void FinishLevelForReal()
    {
        currentCompletionBonusWasSpawned = false;
        bool nextLevelLoaded = LevelManager.instance.LoadNextLevel();
        if (nextLevelLoaded)
        {
            AudioManager.instance.PlayFinishLevelSFX();
            MainLogicManager.instance.StartLevelAfterDelay(2.0f);
            //ShowLevelStartScreen();
        }
        else
        {
            LeaderboardManager.instance.UpdateScoreEntriesDisplay(score);
            MainLogicManager.instance.GameOver(true, score);
        }
    }

    public void LoseLife()
    {
        animator.SetBool("IsDead", true);
        myPlayer.StopPlayer();
        gameIsRunning = false;
        Invoke("LoseLifeForReal", 0.8f);
    }

    private void LoseLifeForReal()
    {
        lives--;
        if (lives < 0)
        {
            animator.SetBool("IsDead", false);
            MainLogicManager.instance.GameOver(false, score);
            LeaderboardManager.instance.UpdateScoreEntriesDisplay(score);
        }
        else
        {
            animator.SetBool("IsDead", false);
            score -= currentLevelScore;
            currentLevelScore = 0;
            keys = 0;
            UIManager.instance.UpdateKeysValueText(keys, infiniteKeys);
            UIManager.instance.UpdateScoreValueText(score);
            UIManager.instance.UpdateLivesValueText(lives);
            LevelManager.instance.ReloadLevel();
            MainLogicManager.instance.StartLevelAfterDelay(2.0f);
            //ShowLevelStartScreen();
        }
    }

    public void AddKey()
    {
        keys++;
        UIManager.instance.UpdateKeysValueText(keys, infiniteKeys);
    }
    public bool UseKey()
    {
        bool canUseKey = (keys > 0) || infiniteKeys;
        if (canUseKey)
        {
            keys--;
            UIManager.instance.UpdateKeysValueText(keys, infiniteKeys);
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

    private bool glitchEffectIsRunning;

    public void TriggerGlitchEffect()
    {
        if (!glitchEffectIsRunning)
        {
            glitchEffectIsRunning = true;
            AudioManager.instance.PlayDayNightSwitchSFX();

            screenMaterial.SetFloat(scrollingStaticFloatName, 0.1f);
            screenMaterial.SetFloat(imageDistorsionFloatName, 0.05f);
            screenMaterial.SetFloat(timeOffsetFloatName, Time.time);
            screenMaterial.SetVector(scrollingStaticRGBVectorName, new Vector4(5, 5, 5, 0));

            StartCoroutine(WaitAndStopGlitchEffect(0.9f));
        }
    }

    private void StopGlitchEffect()
    {
        screenMaterial.SetFloat(scrollingStaticFloatName, 0.1f);
        screenMaterial.SetFloat(imageDistorsionFloatName, 0.0f);
        screenMaterial.SetVector(scrollingStaticRGBVectorName, new Vector4(0, 0, 0, 0));
    }

    private IEnumerator WaitAndStopGlitchEffect(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopGlitchEffect();

        glitchEffectIsRunning = false;
    }
}
