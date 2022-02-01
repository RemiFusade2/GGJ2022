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

    public int GameDifficultyLevel;

    public int allKeysInCurrentLevel;
    public int allCollectiblesInCurrentLevel;

    public int allKeysCollected;
    public int allCollectiblesCollected;

    public Vector2 coalPosition;
    public GameObject coalGo;

    public bool WereAllKeysCollected()
    {
        return (allKeysCollected == allKeysInCurrentLevel);
    }

    public bool WereAllCollectiblesCollected()
    {
        return (allCollectiblesCollected == allCollectiblesInCurrentLevel);
    }

    public void CollectKey()
    {
        allKeysCollected++;
    }

    public void CollectCollectible()
    {
        allCollectiblesCollected++;
    }

    private void Awake()
    {
        instance = this;
        GameDifficultyLevel = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        //LoadFirstLevel();
    }

    public bool IsEndOfWorld1()
    {
        return (currentLevelIndex == allLevelPrefabs.Count - 1 && GameDifficultyLevel == 1);
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

        allKeysInCurrentLevel = 0;
        allCollectiblesInCurrentLevel = 0;
        allKeysCollected = 0;
        allCollectiblesCollected = 0;
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

        // Find Coal, count keys and count collectibles
        coalPosition = Vector2.zero;
        coalGo = null;
        allKeysInCurrentLevel = 0;
        allCollectiblesInCurrentLevel = 0;
        foreach (Transform levelChild in currentLevelGameObject.transform)
        {
            if (levelChild.CompareTag("Coal"))
            {
                coalPosition = levelChild.position;
                coalGo = levelChild.gameObject;
            }
            else if (levelChild.CompareTag("Key") || levelChild.CompareTag("Door"))
            {
                allKeysInCurrentLevel++;
            }
            else if (levelChild.CompareTag("Collectible") || levelChild.CompareTag("Monster"))
            {
                allCollectiblesInCurrentLevel++;
            }
            foreach (Transform levelChildChild in levelChild)
            {
                if (levelChildChild.CompareTag("Coal"))
                {
                    coalPosition = levelChildChild.position;
                    coalGo = levelChildChild.gameObject;
                }
                else if (levelChildChild.CompareTag("Key") || levelChildChild.CompareTag("Door"))
                {
                    allKeysInCurrentLevel++;
                }
                else if (levelChildChild.CompareTag("Collectible") || levelChildChild.CompareTag("Monster"))
                {
                    allCollectiblesInCurrentLevel++;
                }
            }
        }
    }

    public bool LoadNextLevel()
    {
        currentLevelIndex++;
        bool nextLevelLoaded = false;
        if (IsLastLevel())
        {
            // Either we increase difficulty and play all levels again, or we don't load a level and the game end
            if (GameDifficultyLevel == 1)
            {
                GameDifficultyLevel = 2;
                currentLevelIndex = 0;
                ReloadLevel();
                nextLevelLoaded = true;
            }
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

    public Vector3 GetExitPosition()
    {
        // Find Exit
        Vector3 exitPosition = Vector2.zero;
        foreach (Transform levelChild in currentLevelGameObject.transform)
        {
            if (levelChild.CompareTag("Exit"))
            {
                exitPosition = levelChild.transform.position;
                break;
            }
        }
        return exitPosition;
    }

    public void RemoveCoal()
    {
        if (coalGo != null)
        {
            Destroy(coalGo);
        }
    }
}
