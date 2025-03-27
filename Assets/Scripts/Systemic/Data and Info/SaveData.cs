using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

[Serializable]
public struct PersonalData
{
    public string name;
    
    public Color primaryColor;
    public Color secondaryColor;
}

[Serializable]
public struct SaveFile
{
    public int level;
    public float difficulty;
    public int randomSeed;
}

public class SaveData : MonoBehaviour
{
    public bool hasSaveFile;
    public PersonalData currentPersonalData;
    public SaveFile currentSaveFile;
    public int currentSaveFileIndex;
    
    public static SaveData instance;

    [SerializeField] private string filePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        filePath = Application.persistentDataPath;
        
        LoadPersonalData();
    }

    public void LoadPersonalData()
    {
        if (File.Exists(filePath + "/PersonalData"))
        {
            currentPersonalData = JsonUtility.FromJson<PersonalData>(File.ReadAllText(filePath + "/PersonalData"));
        }
        else
        {
            print("No Personal Data Detected");
        }
    }

    public void SavePersonalData()
    {
        File.WriteAllText(File.ReadAllText(filePath + "/PersonalData"), JsonUtility.ToJson(currentPersonalData));
    }

    public void LoadFile(int index)
    {
        if (File.Exists(filePath + "/SaveFile" + index))
        {
            currentSaveFile = JsonUtility.FromJson<SaveFile>(filePath + "/SaveFile" + index);
            hasSaveFile = true;
        }
    }

    private void SaveFile(int index)
    {
        File.WriteAllText(File.ReadAllText(filePath + "/SaveFile" + index), JsonUtility.ToJson(currentSaveFile));
    }
    
    public void CreateFile(int index)
    {
        currentSaveFileIndex = index;
        
        currentSaveFile = new SaveFile();
        currentSaveFile.randomSeed = (int)(System.DateTime.Now.Ticks);
        currentSaveFile.level = 0;
        currentSaveFile.difficulty = 0;
    }

    public void SaveCurrentFile()
    {
        SaveFile(currentSaveFileIndex);
    }

    public void ComposeAndSaveCurrentFile()
    {
        ComposeFile();
        SaveCurrentFile();
    }

    public void ComposeFile() // this fills out a save struct, to then be written to JSON by the SaveFile()
    {
        currentSaveFile.randomSeed = GameGeneralManager.instance.seededRandom.Next();
        currentSaveFile.level = GameGeneralManager.instance.currentLevel;
        currentSaveFile.difficulty = GameGeneralManager.instance.difficulty;
    }


}
