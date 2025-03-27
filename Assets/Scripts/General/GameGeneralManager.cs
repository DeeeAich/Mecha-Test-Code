using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameGeneralManager : MonoBehaviour
{
    [Header("Game Run Bits")] 
    public bool saveDataOnQuit = true;
    public int currentLevel;
    public float difficulty;
    public int randomSeed;
    public Random seededRandom;

    [Header("Misc")]
    public float gameSpeed = 1;
    
    
    public static GameGeneralManager instance;
    
    
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
    }

    private void OnApplicationQuit()
    {
        if (saveDataOnQuit)
        {
            SaveData.instance.SaveCurrentFile();
        }
    }

    public void ChangeScene(int targetScene)
    {
        FadeCanvas.instance.FadeToBlack();

        switch (targetScene)
        {
            case 0:
                SaveData.instance.hasSaveFile = false;
                break;
        }

        SceneManager.LoadScene(targetScene);
    }
}