using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pauseMenu : MonoBehaviour
{
    [SerializeField] private bool usesDevkitButtons = true;
    
    [Tooltip("Set to zero to ignore")]
    [SerializeField] private float inactivityTime;
    public float inactivityTimer = 0;
    private bool useInactivityTimer;
    public bool hasInputsPressed;
    
    
    public bool paused;
    public bool canPause = true;
    public bool canUseDevkit = true;
    
    public float gameSpeed = 1f;
    
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject mainPauseMenu;
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject devkitCheatsMenu;
    
    [SerializeField] private Button mainPauseMenuInitialButton;
    [SerializeField] private Button inventoryMenuInitialButton;

    [SerializeField] private InventoryManager InventoryManager;

    private InputAction pauseAction;
    private InputAction openInventoryAction;
    private InputAction openDevkitCheatsAction;
    private InputAction anyAction;

    public UnityEvent onPause;
    public UnityEvent onUnpause;

    private void Start()
    {
        devkitCheatsMenu = FindObjectOfType<DevKitCheats>(true).gameObject;
        
        pauseAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Pause"];
        openInventoryAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Inventory"];
        openDevkitCheatsAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["DevkitCheats"];
        anyAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Any Action"];
        
        pauseAction.performed += onPauseButtonPressed;
        openInventoryAction.performed += OpenInventory;
        openDevkitCheatsAction.performed += OpenDevkitCheats;
        
        anyAction.performed += anyActionPressed;
        anyAction.canceled += anyActionReleased;
        
        if (inactivityTime != 0)
        {
            useInactivityTimer = true;
            inactivityTimer = inactivityTime;
        }
    }

    private void FixedUpdate()
    {
        if (useInactivityTimer && !paused && !hasInputsPressed)
        {
            if (inactivityTimer > 0)
            {
                inactivityTimer -= Time.fixedDeltaTime;
                if (inactivityTimer <= 0)
                {
                    SceneManager.LoadScene(0);
                }
            }
        }
    }
    
    private void Update()
    {
        if (usesDevkitButtons && Input.GetKey(KeyCode.RightBracket))
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                FindObjectOfType<DevKitCheats>(true).ToggleEnemyTime();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                SceneManager.LoadScene(0);
            }
        }
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
    
    public void TriggerPauseFromMenu()
    {
        if(paused) onPauseButtonPressed(new InputAction.CallbackContext());
    }

    public void TriggerOpenInventoryFromMenu()
    {
        if (paused)
        {
            devkitCheatsMenu.SetActive(false);
            mainPauseMenu.SetActive(false);
            inventoryMenu.SetActive(true);
            InventoryManager.UpdateInventory();
        }
    }

    public void TriggerBackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Reset()
    {
        Debug.Log("Resetting Game");
        
        pauseAction.performed -= onPauseButtonPressed;
        openInventoryAction.performed -= OpenInventory;
        
        TogglePause();
        SceneManager.LoadScene(1);
    }

    private void OnDestroy()
    {
        pauseAction.performed -= onPauseButtonPressed;
        openInventoryAction.performed -= OpenInventory;
        openDevkitCheatsAction.performed -= OpenDevkitCheats;
        anyAction.performed -= anyActionPressed;
        anyAction.started -= anyActionPressed;
        anyAction.canceled -= anyActionPressed;
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
            inventoryMenuInitialButton.Select();
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
            mainPauseMenuInitialButton.Select();
        }
        
        TogglePause();
    }

    public void anyActionPressed(InputAction.CallbackContext context)
    {
        if (useInactivityTimer)
        {
            inactivityTimer = inactivityTime;
            hasInputsPressed = true;
        }
    }

    public void anyActionReleased(InputAction.CallbackContext context)
    {
        if (useInactivityTimer)
        {
            hasInputsPressed = false;
        }
        
    }



}