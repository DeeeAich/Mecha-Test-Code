using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public struct PersonalData
{
    public string name;
    
    public Color primaryColor;
    public Color secondaryColor;
}

public struct SaveFile
{
    public float level;
}

public class SaveData : MonoBehaviour
{
    public PersonalData currentPersonalData;
    public SaveFile currentSaveFile;
    
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
                print("Removing duplicate save data");
                Destroy(gameObject);
            }
        }

        filePath = Application.persistentDataPath;

        if (File.Exists(filePath + "/PersonalData"))
        {
            LoadPersonalData();
        }
        else
        {
            print("No Personal Data Detected");
        }
    }



    public void LoadPersonalData()
    {
        currentPersonalData = JsonUtility.FromJson<PersonalData>(File.ReadAllText(filePath + "/PersonalData"));
    }

    public void SavePersonalData()
    {
        File.WriteAllText(File.ReadAllText(filePath + "/PersonalData"), JsonUtility.ToJson(currentPersonalData));
    }

    public void LoadSaveFile(int index)
    {
        currentSaveFile = JsonUtility.FromJson<SaveFile>(filePath + "/SaveFile" + index);
    }

    public void SaveSaveFile(int index)
    {
        File.WriteAllText(File.ReadAllText(filePath + "/SaveFile" + index), JsonUtility.ToJson(currentSaveFile));
    }

    public void ComposeSaveFile()
    {
        
    }
}
