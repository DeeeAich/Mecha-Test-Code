using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    public void PauseGame()
    {
        DevKitCheats devkit = FindObjectOfType<DevKitCheats>();
        if (devkit != null && devkit.devkitCheatMenu.activeSelf) // gets rid of devkit menu
        {
            devkit.devkitCheatMenu.SetActive(false);
        }

        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            PlayerBody.PlayBody().StopParts(false, false);
            menu.SetActive(true);
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            PlayerBody.PlayBody().StopParts(true, true);
            menu.SetActive(false);
        }
    }

    public void onPauseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PauseGame();
        }
    }

    public void Reset()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}