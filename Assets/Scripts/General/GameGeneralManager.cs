using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameGeneralManager : MonoBehaviour
{
    [Header("Game Run Bits")] 
    public bool saveDataOnQuit = false;
    public int currentLevel;
    public float difficulty;
    public int randomSeed;
    public Random seededRandom;

    [Header("Misc")]
    public float gameSpeed = 1;

    public static GameGeneralManager instance;

    private float sceneTransitionTimer = 0;
    private int sceneTransitionTargetScene;
    
    
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

    private void FixedUpdate()
    {
        if (sceneTransitionTimer > 0)
        {
            sceneTransitionTimer -= Time.fixedDeltaTime;

            if (sceneTransitionTimer <= 0)
            {
                SceneManager.LoadScene(sceneTransitionTargetScene);
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
        sceneTransitionTargetScene = targetScene;
        sceneTransitionTimer = 1;

        switch (targetScene)
        {
            case 0:
                if(SaveData.instance != null) SaveData.instance.hasSaveFile = false;
                break;
            
            case 1:
                
                break;
            
            case 2:
                difficulty = 1;
                break;
        }


    }

    public void CreateNewSeed()
    {
        
        randomSeed = (int)(System.DateTime.Now.Ticks);
        seededRandom = new Random(randomSeed);
        
        Debug.Log("Creating new seeded random");
    }
}