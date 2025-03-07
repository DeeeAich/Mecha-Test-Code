using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    public bool paused;
    public bool canPause = true;
    
    public float gameSpeed = 1f;
    
    [SerializeField] private GameObject[] objectsToActivateWhenPaused;

    private InputAction pauseAction;

    public UnityEvent onPause;
    public UnityEvent onUnpause;

    private void Start()
    {
        pauseAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Pause"];
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

            if (!paused)
            {
                PlayerBody.Instance().StopParts(false, false);

                for (int i = 0; i < objectsToActivateWhenPaused.Length; i++)
                {
                    objectsToActivateWhenPaused[i].SetActive(true);
                }
                
                onPause.Invoke();
                paused = true;
                Time.timeScale = 0;
            }
            else if (paused)
            {
                PlayerBody.Instance().StopParts(true, true);
      
                for (int i = 0; i < objectsToActivateWhenPaused.Length; i++)
                {
                    objectsToActivateWhenPaused[i].SetActive(false);
                }
                
                onUnpause.Invoke();
                paused = false;
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