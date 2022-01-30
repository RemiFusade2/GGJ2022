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
    public float timeBonusRequiredTime;

    [Header("References")]
    public Material screenMaterial;
    public Animator animator;

    [Header("Prefabs")]
    public GameObject scorePrefab;
    [Space]
    public GameObject timeBonusPrefab;

    // Runtime
    [Header("Runtime")]
    public int score;
    private int currentLevelScore;
    public int keys;
    public int lives;
    public bool gameIsRunning;

    public CYCLETYPE currentCycleType;
    private float cycleCurrentTimer;
    private float currentGameTime;

    private PlayerController myPlayer;

    private bool currentTimeBonusWasSpawned;



    private void Awake()
    {
        instance = this;
        gameIsRunning = false;
        glitchEffectIsRunning = false;
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
        score = 0;
        currentLevelScore = 0;
        keys = 0;
        lives = 2;
        currentGameTime = 0;
        currentTimeBonusWasSpawned = false;
        UIManager.instance.UpdateKeysValueText(keys);
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

    public void ShowLevelStartScreen()
    {
        gameIsRunning = false;
        SwitchToDayTime();
        UIManager.instance.UpdateDayNightSliderValue(cycleCurrentTimer);
        UIManager.instance.ShowStartLevelPanel(LevelManager.instance.currentLevelIndex);
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
        currentTimeBonusWasSpawned = false;
    }

    public void TrySpawnTimeBonus()
    {
        Invoke("InvokeTrySpawnTimeBonus", 0.1f);
    }

    private void InvokeTrySpawnTimeBonus()
    {
        int sumOfAllCollectibleItems = GameObject.FindGameObjectsWithTag("Collectible").Length + GameObject.FindGameObjectsWithTag("Monster").Length;

        Debug.Log("TrySpawnTimeBonus. " + sumOfAllCollectibleItems + " ; " + currentGameTime);
        if (sumOfAllCollectibleItems == 0 && !currentTimeBonusWasSpawned && currentGameTime <= timeBonusRequiredTime)
        {
            SpawnTimeBonus();
            currentTimeBonusWasSpawned = true;
        }
    }

    private void SpawnTimeBonus()
    {
        Vector3 exitPosition = LevelManager.instance.GetExitPosition();

        Transform parent = LevelManager.instance.currentLevelGameObject.transform;
        GameObject currentTimeBonus = Instantiate(timeBonusPrefab, exitPosition, timeBonusPrefab.transform.rotation, parent);
        currentTimeBonus.GetComponent<DayNightCycle>().currentCycle = CYCLETYPE.DAY;
        currentTimeBonus.GetComponent<LifetimeBehaviour>().SetLifespan(timeBonusLifespan);
    }

    public void FinishLevel()
    {
        myPlayer.StopPlayer();
        gameIsRunning = false;

        Invoke("FinishLevelForReal", 0.6f);
    }

    private void FinishLevelForReal()
    {
        keys = 0;
        currentLevelScore = 0;
        currentTimeBonusWasSpawned = false;
        UIManager.instance.UpdateKeysValueText(keys);
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
        Invoke("LoseLifeForReal", 0.6f);
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
            UIManager.instance.UpdateKeysValueText(keys);
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
