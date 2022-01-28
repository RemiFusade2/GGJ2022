using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Reference")]
    public GameObject startPanel;

    // Runtime
    private CYCLETYPE currentCycleType;
    private float cycleCurrentTimer;

    public bool gameStarted { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameStarted = false;
        SwitchToDayTime();
    }

    private void FixedUpdate()
    {
        if (gameStarted)
        {
            cycleCurrentTimer -= Time.fixedDeltaTime;

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

    public void StartGame()
    {
        gameStarted = true;
        startPanel.SetActive(false);
    }

    public void StopGame()
    {
        gameStarted = false;
        startPanel.SetActive(true);
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
