using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Levels")]
    public List<GameObject> allLevelPrefabs;

    [Header("Reference to Player")]
    public Transform player;

    [Header("Runtime")]
    public GameObject currentLevelGameObject;
    public int currentLevelIndex;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //LoadFirstLevel();
    }

    public bool IsLastLevel()
    {
        return (currentLevelIndex >= allLevelPrefabs.Count);
    }

    public void UnloadLevel()
    {
        if (currentLevelGameObject != null)
        {
            Destroy(currentLevelGameObject);
        }
    }

    public void ReloadLevel()
    {
        UnloadLevel();
        currentLevelGameObject = Instantiate(allLevelPrefabs[currentLevelIndex]);
        
        // Position player at spawn point
        foreach (Transform levelChild in currentLevelGameObject.transform)
        {
            if (levelChild.CompareTag("Spawn Position") && player != null)
            {
                player.GetComponent<PlayerController>().Teleport(new Vector2(levelChild.position.x, levelChild.position.y));
                break;
            }
        }
    }

    public bool LoadNextLevel()
    {
        currentLevelIndex++;
        bool nextLevelLoaded = false;
        if (IsLastLevel())
        {
            //MainLogicManager.instance.GameOver();
        }
        else 
        {
            ReloadLevel();
            nextLevelLoaded = true;
        }
        return nextLevelLoaded;
    }

    public void LoadFirstLevel()
    {
        currentLevelIndex = 0;
        ReloadLevel();
    }
}
