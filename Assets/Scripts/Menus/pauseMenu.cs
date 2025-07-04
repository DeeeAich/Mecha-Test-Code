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
    


    [Tooltip("Set to zero to ignore")]
    [SerializeField] private float inactivityTime;
    public float inactivityTimer = 0;
    private bool useInactivityTimer;
    public bool hasInputsPressed;
    
    
    public bool paused;
    public bool canPause = true;
    public bool canUseDevkit = true;
    
    public float gameSpeed = 1f;
    public int mainMenuScene = 1;
    public int sceneToResetTo = 5;
    
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject mainPauseMenu;
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject devkitCheatsMenu;
    
    [SerializeField] private Button mainPauseMenuInitialButton;
    [SerializeField] private Button inventoryMenuInitialButton;

    [SerializeField] private InventoryManager InventoryManager;
    [SerializeField] private GameObject inventoryPlayerCamera;

    private InputAction pauseAction;
    private InputAction openInventoryAction;
    private InputAction openDevkitCheatsAction;
    private InputAction anyAction;

    public UnityEvent onPause;
    public UnityEvent onUnpause;

    private void Start()
    {
        pauseAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Pause"];
        openInventoryAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Inventory"];
        openDevkitCheatsAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["DevkitCheats"];
        anyAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Any Action"];
        
        pauseAction.performed += onPauseButtonPressed;
        openInventoryAction.performed += OpenInventory;
        openDevkitCheatsAction.performed += OpenDevkitCheats;

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

    public void TogglePause()
    {

        if (canPause)
        {
            if (paused)
            {
                PlayerBody.Instance().PauseSystem(PlayerSystems.AllParts, false);

                onUnpause.Invoke();
                paused = false;
                Time.timeScale = gameSpeed;
                Cursor.visible = false;
            }
            else
            {
                PlayerBody.Instance().PauseSystem(PlayerSystems.AllParts, true);
                
                onPause.Invoke();
                paused = true;
                Time.timeScale = 0;

                if (!PlayerBody.Instance().isGamepad)
                    Cursor.visible = true;
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
            inventoryPlayerCamera.SetActive(true);
            InventoryManager.UpdateInventory();
        }
    }

    public void TriggerBackToMainMenu()
    {
        Time.timeScale = 1;
        
        if (GameGeneralManager.instance != null)
        {
            GameGeneralManager.instance.ChangeScene(mainMenuScene);
        }
        else
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }

    public void Reset()
    {
        Debug.Log("Resetting Game");
        
        pauseAction.performed -= onPauseButtonPressed;
        openInventoryAction.performed -= OpenInventory;
        
        TogglePause();
        sceneToResetTo = SceneManager.GetActiveScene().buildIndex;
        
        if (GameGeneralManager.instance != null)
        {
            GameGeneralManager.instance.ChangeScene(sceneToResetTo);
        }
        else
        {
            SceneManager.LoadScene(sceneToResetTo);
        }
  
    }

    private void OnDestroy()
    {
        pauseAction.performed -= onPauseButtonPressed;
        openInventoryAction.performed -= OpenInventory;
        openDevkitCheatsAction.performed -= OpenDevkitCheats;
    }

    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (paused)
        {
            devkitCheatsMenu.SetActive(false);
            mainPauseMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            inventoryPlayerCamera.SetActive(false);
            
        }
        else
        {
            inventoryPlayerCamera.SetActive(true);
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
            inventoryPlayerCamera.SetActive(false);
        }
        else
        {
            FindObjectOfType<DevKitCheats>().UpdateDropdowns();
            Debug.Log("Opening and Updating devkits");
            devkitCheatsMenu.SetActive(true);
        }
        
        TogglePause();
    }

    public void onPauseButtonPressed(InputAction.CallbackContext context)
    {
        //print("Pause button");

        if (paused)
        {
            devkitCheatsMenu.SetActive(false);
            mainPauseMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            inventoryPlayerCamera.SetActive(false);
            TogglePause();
        }
        else
        {
            bool closedAPickup = false;
            
            foreach (Pickup pickup in FindObjectsOfType<Pickup>(true))
            {
                if (pickup.open)
                {
                    pickup.ClosePickupMenu();
                    closedAPickup = true;
                }
            }

            if (!closedAPickup)
            {
                mainPauseMenu.SetActive(true);
                inventoryPlayerCamera.SetActive(true);
                mainPauseMenuInitialButton.Select();
                TogglePause();
            }
        }
        
   
    }
}