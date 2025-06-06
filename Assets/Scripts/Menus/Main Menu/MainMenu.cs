using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private int sceneForTutorial = 2;
    [SerializeField] private int sceneForSkipTutorial = 5;
    private bool skipTutorial;

    private bool timerRunningToSelectFirstButton = true;
    [SerializeField] private Button firstButton;
    float timer = 2f;

    private void Start()
    {
        Cursor.visible = true;
    }

    private void FixedUpdate()
    {
        if (timerRunningToSelectFirstButton)
        {
            timer -= Time.fixedDeltaTime;

            if (timer <= 0)
            {
                if (firstButton.gameObject.activeInHierarchy)
                {
                    firstButton.Select();
                }
   
                timerRunningToSelectFirstButton = false;
            }
        }

    }

    public void StartGame()
    {
        if (skipTutorial)
        {
            GameGeneralManager.instance.ChangeScene(sceneForSkipTutorial);
        }
        else
        {
            GameGeneralManager.instance.ChangeScene(sceneForTutorial);
        }

    }

    public void LoadSave(int index)
    {
        SaveData.instance.LoadOrCreateFile(index);
    }

    public void SetSkipTutorial(Toggle toggle)
    {
        skipTutorial = toggle.isOn;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
