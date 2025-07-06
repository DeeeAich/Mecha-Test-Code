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
    [Header("EDITOR TOOLS")] 
    [SerializeField] private bool EditorClearSaveFiles;
    [SerializeField] private bool EditorClearPersonalData;

    [Header("Internal")]
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

    private void Update()
    {
        if (EditorClearSaveFiles)
        {
            EditorClearSaveFiles = false;
        }

        if (EditorClearPersonalData)
        {
            EditorClearPersonalData = false;
        }
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
            Debug.Log("Loading Save File " + index);
            currentSaveFile = JsonUtility.FromJson<SaveFile>(filePath + "/SaveFile" + index);
            hasSaveFile = true;
        }
        else
        {
            Debug.LogError("Failed To Load Save File " + index + ", No File Found");
        }
    }

    private void SaveFile(int index)
    {
        if (File.Exists(filePath + "/SaveFile" + index))
        {
            File.WriteAllText(File.ReadAllText(filePath + "/SaveFile" + index), JsonUtility.ToJson(currentSaveFile));
        }

    }
    
    public void CreateFile(int index)
    {
        Debug.Log("Creating Save File " + index);
        
        currentSaveFileIndex = index;
        
        currentSaveFile = new SaveFile();
        currentSaveFile.randomSeed = (int)(System.DateTime.Now.Ticks);
        currentSaveFile.level = 0;
        currentSaveFile.difficulty = 0;
        
        SaveFile(index);
    }

    public void LoadOrCreateFile(int index)
    {
        if (File.Exists(filePath + "/SaveFile" + index))
        {
            LoadFile(index);
        }
        else
        {
            CreateFile(index);
        }
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
        currentSaveFile.randomSeed = GameGeneralManager.instance.randomSeed;
        currentSaveFile.level = GameGeneralManager.instance.currentLevel;
        currentSaveFile.difficulty = GameGeneralManager.instance.difficulty;
    }


}
