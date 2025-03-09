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
    
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject mainPauseMenu;
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject devkitCheatsMenu;

    [SerializeField] private InventoryManager InventoryManager;

    private InputAction pauseAction;
    private InputAction openInventoryAction;
    private InputAction openDevkitCheatsAction;

    public UnityEvent onPause;
    public UnityEvent onUnpause;

    private void Start()
    {
        devkitCheatsMenu = FindObjectOfType<DevKitCheats>(true).gameObject;
        
        pauseAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Pause"];
        openInventoryAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Inventory"];
        
        pauseAction.performed += onPauseButtonPressed;
        openInventoryAction.performed += OpenInventory;
    }

    public void TogglePause()
    {
        if (canPause)
        {
            if (paused)
            {
                PlayerBody.Instance().StopParts(true, true);

                onUnpause.Invoke();
                paused = false;
                Time.timeScale = gameSpeed;
            }
            else
            {
                PlayerBody.Instance().StopParts(false, false);
                
                onPause.Invoke();
                paused = true;
                Time.timeScale = 0;
            }
        }
        else
        {
            Debug.LogWarning("Cant TogglePause Right Now");
        }
    }

    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (paused)
        {
            devkitCheatsMenu.SetActive(false);
            mainPauseMenu.SetActive(false);
            inventoryMenu.SetActive(false);
        }
        else
        {
            inventoryMenu.SetActive(true);
            InventoryManager.UpdateInventory();
        }

        TogglePause();
    }

    public void OpenDevkitCheats(InputAction.CallbackContext context)
    {
        if (paused)
        {
            devkitCheatsMenu.SetActive(false);
            mainPauseMenu.SetActive(false);
            inventoryMenu.SetActive(false);
        }
        else
        {
            devkitCheatsMenu.SetActive(true);
        }
        
        TogglePause();
    }

    public void onPauseButtonPressed(InputAction.CallbackContext context)
    {
        print("Pause button");

        if (paused)
        {
            devkitCheatsMenu.SetActive(false);
            mainPauseMenu.SetActive(false);
            inventoryMenu.SetActive(false);
        }
        else
        {
            mainPauseMenu.SetActive(true);
        }
        
        TogglePause();
    }

    public void Reset()
    {
        Debug.Log("Resetting Game");
        
        pauseAction.performed -= onPauseButtonPressed;
        openInventoryAction.performed -= OpenInventory;
        
        TogglePause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        pauseAction.performed -= onPauseButtonPressed;
        openInventoryAction.performed -= OpenInventory;
    }
}