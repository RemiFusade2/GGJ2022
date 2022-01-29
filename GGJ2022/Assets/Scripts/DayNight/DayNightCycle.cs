using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CYCLEMODE
{
    NOTHING_HAPPEN,
    INSTANTIATE_PREFAB,
    SWITCH_GAMEOBJECT
}

public class DayNightCycle : MonoBehaviour
{
    [Header("Settings - mode")]
    public CYCLEMODE cycleMode;

    [Header("Settings - starting state")]
    public CYCLETYPE currentCycle;
    public DIRECTION facingDirection;

    [Header("Settings - INSTANTIATE_PREFAB mode")]
    public GameObject dualPrefab;

    [Header("Settings - SWITCH_GAMEOBJECT mode")]
    public GameObject dayGameObject;
    public GameObject nightGameObject;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitCycle(CYCLETYPE startCycleType)
    {
        currentCycle = startCycleType;
    }

    private void DestroyItselfAndSpawnNextPrefabAfterDelay(float delay)
    {
        StartCoroutine(DestroyItselfAfterDelay(delay));
        InstantiateManager.instance.SpawnObjectAfterDelay(dualPrefab, this.transform.position, dualPrefab.transform.rotation, this.transform.parent, delay + 0.01f, currentCycle, facingDirection);
    }

    public void SwitchToDay()
    {
        bool toggle = (currentCycle != CYCLETYPE.DAY);

        currentCycle = CYCLETYPE.DAY;

        switch (cycleMode)
        {
            case CYCLEMODE.NOTHING_HAPPEN:
                break;
            case CYCLEMODE.INSTANTIATE_PREFAB:

                if (toggle)
                {
                    DestroyItselfAndSpawnNextPrefabAfterDelay(0.01f);
                }

                break;
            case CYCLEMODE.SWITCH_GAMEOBJECT:
                dayGameObject.SetActive(true);
                nightGameObject.SetActive(false);
                break;
        }
    }

    public void SwitchToNight()
    {
        bool toggle = (currentCycle != CYCLETYPE.NIGHT);

        currentCycle = CYCLETYPE.NIGHT;

        switch (cycleMode)
        {
            case CYCLEMODE.NOTHING_HAPPEN:
                break;
            case CYCLEMODE.INSTANTIATE_PREFAB:

                if (toggle)
                {
                    DestroyItselfAndSpawnNextPrefabAfterDelay(0.01f);
                }

                break;
            case CYCLEMODE.SWITCH_GAMEOBJECT:
                dayGameObject.SetActive(false);
                nightGameObject.SetActive(true);
                break;
        }
    }

    private IEnumerator DestroyItselfAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }

    public void SetFacingDirection(DIRECTION facingDir)
    {
        facingDirection = facingDir;
        if (this.GetComponent<MonsterBehaviour>() != null)
        {
            this.GetComponent<MonsterBehaviour>().SetCurrentDirection(facingDir);
        }
    }
}
