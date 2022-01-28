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
                    StartCoroutine(DestroyItselfAfterDelay(0.1f));
                    StartCoroutine(SpawnPrefabfAfterDelay(dualPrefab, 0.08f));
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
                    StartCoroutine(DestroyItselfAfterDelay(0.1f));
                    StartCoroutine(SpawnPrefabfAfterDelay(dualPrefab, 0.08f));
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
    private IEnumerator SpawnPrefabfAfterDelay(GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject spawnedObject = Instantiate(prefab, this.transform.position, prefab.transform.rotation, this.transform.parent);
        spawnedObject.GetComponent<DayNightCycle>().currentCycle = currentCycle;
        spawnedObject.GetComponent<DayNightCycle>().facingDirection = facingDirection;
    }
}
