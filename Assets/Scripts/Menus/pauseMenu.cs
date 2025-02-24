using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    public bool canPause = true;
    public float gameSpeed = 1f;
    
    [SerializeField] private GameObject menu;

    private InputAction pauseAction;

    public UnityEvent onPause;
    public UnityEvent onUnpause;

    private void Start()
    {
        pauseAction = PlayerBody.PlayBody().GetComponent<PlayerInput>().actions["Pause"];
        pauseAction.performed += onPauseButtonPressed;
    }

    public void PauseGame()
    {
        if (canPause)
        {
            DevKitCheats devkit = FindObjectOfType<DevKitCheats>();
            if (devkit != null && devkit.devkitCheatMenu.activeSelf) // gets rid of devkit menu
            {
                devkit.devkitCheatMenu.SetActive(false);
            }

            if (Time.timeScale == 1 || Time.timeScale == gameSpeed)
            {
                PlayerBody.PlayBody().StopParts(false, false);
                menu.SetActive(true);
                onPause.Invoke();
                Time.timeScale = 0;
            }
            else if (Time.timeScale == 0)
            {
                PlayerBody.PlayBody().StopParts(true, true);
                menu.SetActive(false);
                onUnpause.Invoke();
                Time.timeScale = gameSpeed;
            }
        }
    }

    public void onPauseButtonPressed(InputAction.CallbackContext context)
    {
        print("Pause button");
        PauseGame();
    }

    public void Reset()
    {
        Debug.Log("Resetting Game");
        pauseAction.performed -= onPauseButtonPressed;
        PauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}