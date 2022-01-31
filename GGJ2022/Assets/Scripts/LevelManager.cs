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
        GameObject coal = null;
        foreach (Transform levelChild in currentLevelGameObject.transform)
        {
            if (levelChild.CompareTag("Coal"))
            {
                coal = levelChild.gameObject;
                break;
            }
            foreach (Transform levelChildChild in levelChild)
            {
                if (levelChildChild.CompareTag("Coal"))
                {
                    coal = levelChildChild.gameObject;
                    break;
                }
            }
        }

        if (coal != null)
        {
            Destroy(coal);
        }
    }

    public bool GetCoalPosition(out Vector3 coalPosition)
    {
        // Find Coal
        bool isCoal = false;
        coalPosition = Vector2.zero;
        foreach (Transform levelChild in currentLevelGameObject.transform)
        {
            if (levelChild.CompareTag("Coal"))
            {
                coalPosition = levelChild.position;
                isCoal = true;
                break;
            }
            foreach (Transform levelChildChild in levelChild)
            {
                if (levelChildChild.CompareTag("Coal"))
                {
                    coalPosition = levelChildChild.position;
                    isCoal = true;
                    break;
                }
            }
        }
        return isCoal;
    }
}
