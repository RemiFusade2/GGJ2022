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

    [Header("References")]
    public Material screenMaterial;

    [Header("Prefabs")]
    public GameObject score50Prefab;
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

    private PlayerController myPlayer;

    private GameObject currentTimeBonus;



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
        UIManager.instance.UpdateScoreValueText(score);
        UIManager.instance.UpdateLivesValueText(lives);
    }

    public void IncreaseScore(int scoreAdd, Vector3 itemPosition)
    {
        score += scoreAdd;
        currentLevelScore += scoreAdd;
        UIManager.instance.UpdateScoreValueText(score);
        GameObject scoreObj = Instantiate(score50Prefab, itemPosition, score50Prefab.transform.rotation);
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
        SpawnTimeBonus();

        UIManager.instance.HideAllPanels();
        UIManager.instance.ShowTopInfoPanel();
        SwitchToDayTime();
        UIManager.instance.UpdateDayNightSliderValue(cycleCurrentTimer);

        gameIsRunning = true;
        currentLevelScore = 0;
    }

    private void SpawnTimeBonus()
    {
        Vector3 exitPosition = LevelManager.instance.GetExitPosition();

        Transform parent = LevelManager.instance.currentLevelGameObject.transform;
        currentTimeBonus = Instantiate(timeBonusPrefab, exitPosition - 0.3f * Vector3.right, timeBonusPrefab.transform.rotation, parent);
        currentTimeBonus.GetComponent<DayNightCycle>().currentCycle = CYCLETYPE.DAY;
        currentTimeBonus.GetComponent<LifetimeBehaviour>().SetLifespan(timeBonusLifespan);
    }

    public void FinishLevel()
    {
        myPlayer.StopPlayer();
        keys = 0;
        currentLevelScore = 0;
        UIManager.instance.UpdateKeysValueText(keys);
        bool nextLevelLoaded = LevelManager.instance.LoadNextLevel();
        if (nextLevelLoaded)
        {
            ShowLevelStartScreen();
        }
        else
        {
            MainLogicManager.instance.GameOver(true, score);
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
        if (lives < 0)
        {
            gameIsRunning = false;
            MainLogicManager.instance.GameOver(false, score);
        }
        else
        {
            score -= currentLevelScore;
            currentLevelScore = 0;
            UIManager.instance.UpdateScoreValueText(score);
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

    private bool glitchEffectIsRunning;

    public void TriggerGlitchEffect()
    {
        if (!glitchEffectIsRunning)
        {
            glitchEffectIsRunning = true;

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
