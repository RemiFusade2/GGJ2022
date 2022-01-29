using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateManager : MonoBehaviour
{
    public static InstantiateManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnObjectAfterDelay(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, float delay, CYCLETYPE cycle, DIRECTION facingDirection)
    {
        StartCoroutine(SpawnPrefabfAfterDelay(prefab, position, rotation, parent, delay, cycle, facingDirection));
    }

    private IEnumerator SpawnPrefabfAfterDelay(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, float delay, CYCLETYPE cycle, DIRECTION facingDirection)
    {
        yield return new WaitForSeconds(delay);

        if (parent != null)
        {
            GameObject spawnedObject = Instantiate(prefab, position, rotation, parent);
            spawnedObject.GetComponent<DayNightCycle>().currentCycle = cycle;
            spawnedObject.GetComponent<DayNightCycle>().SetFacingDirection(facingDirection);
        }
    }
}
