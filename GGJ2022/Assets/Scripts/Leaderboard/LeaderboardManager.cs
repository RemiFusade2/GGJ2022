using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class ScoreEntryData
{
    public string name;
    public int score;
}

[System.Serializable]
public class ScoreEntriesData
{
    public List<ScoreEntryData> data;
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;

    private ScoreEntriesData allScoreEntriesData;

    public bool activeScoreEntered;

    public string saveFileName;
    public const string playerPrefsKey = "Mole_Leaderboard_Data";

    private ScoreEntryData currentScoreEntry;

    public void SaveScoreEntries()
    {
        try
        {
            string json = JsonUtility.ToJson(allScoreEntriesData);
            PlayerPrefs.SetString(playerPrefsKey, json);

            /*
            string destination = Application.persistentDataPath + "/" + saveFileName;
            FileStream file;

            if (File.Exists(destination)) file = File.OpenWrite(destination);
            else file = File.Create(destination);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, allScoreEntriesData);

            file.Close();*/
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Can't save file. Exception: " + ex.Message);
        }
    }

    public void LoadScoreEntries()
    {
        //string destination = Application.persistentDataPath + "/" + saveFileName;
        //FileStream file;


        try
        {
            string json = PlayerPrefs.GetString(playerPrefsKey);
            if (string.IsNullOrEmpty(json))
            {
                // load default scores
                CreateDefaultEntriesData();
                SaveScoreEntries();
                return;
            }
            else
            {
                ScoreEntriesData loadedData = JsonUtility.FromJson<ScoreEntriesData>(json);
                allScoreEntriesData = loadedData;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Can't load file. Exception: " + ex.Message);
        }

        /*
        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogWarning("File not found");
            CreateDefaultEntriesData();
            SaveScoreEntries();
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        ScoreEntriesData data = (ScoreEntriesData)bf.Deserialize(file);
        file.Close();*/
    }

    public void CreateDefaultEntriesData()
    {
        ScoreEntriesData defaultEntries = new ScoreEntriesData();
        defaultEntries.data = new List<ScoreEntryData>();
        defaultEntries.data.Add(new ScoreEntryData() { name = "REM", score = 10000 });
        defaultEntries.data.Add(new ScoreEntryData() { name = "JEJ", score = 18000 });
        defaultEntries.data.Add(new ScoreEntryData() { name = "AND", score = 15000 });

        allScoreEntriesData = defaultEntries;
    }

    public void UpdateScoreEntriesDisplay(int currentScore)
    {
        LoadScoreEntries();

        List<ScoreEntryData> ordererScoreEntries = allScoreEntriesData.data.OrderByDescending(x => x.score).ToList();

        activeScoreEntered = true;

        int currentRank = 1;
        foreach (ScoreEntryData scoreEntry in ordererScoreEntries)
        {
            if (currentScore <= scoreEntry.score)
            {
                currentRank++;
            }
            else
            {
                if (currentRank <= 7)
                {
                    currentScoreEntry = new ScoreEntryData() { name = "...", score = currentScore };
                    allScoreEntriesData.data.Add(currentScoreEntry);
                    ordererScoreEntries.Insert(currentRank - 1, currentScoreEntry);
                    activeScoreEntered = false;
                }
                break;
            }
        }

        if (activeScoreEntered && ordererScoreEntries.Count < 7 && currentScore > 0)
        {
            currentScoreEntry = new ScoreEntryData() { name = "...", score = currentScore };
            allScoreEntriesData.data.Add(currentScoreEntry);
            ordererScoreEntries.Add(currentScoreEntry);
            activeScoreEntered = false;
        }

        UIManager.instance.DisplayScoreEntries(ordererScoreEntries, currentRank);
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void EnterScore(string _name, int _score)
    {
        activeScoreEntered = true;

        currentScoreEntry.name = _name;
        currentScoreEntry.score = _score;

        Debug.Log("Enter Score : " + currentScoreEntry.name + " : " + currentScoreEntry.score);

        SaveScoreEntries();
    }
}
