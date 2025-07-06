using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    
    private PlayerInput playerInput;
    private bool usingGamepad;
    private void Start()
    {
        Cursor.visible = true;
        playerInput = FindObjectOfType<PlayerInput>();
        playerInput.onControlsChanged += OnControlsChanged;
        playerInput.actions["Any Action"].performed += TriggerControlsCheck;
        
        usingGamepad = playerInput.currentControlScheme.Equals("Controller");
        Cursor.visible = !usingGamepad;
    }

    private void FixedUpdate()
    {
        if (timerRunningToSelectFirstButton)
        {
            timer -= Time.fixedDeltaTime;

            if (timer <= 0)
            {
                if (firstButton.gameObject.activeInHierarchy && usingGamepad)
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

    private void TriggerControlsCheck(InputAction.CallbackContext context)
    {
        if(!usingGamepad) OnControlsChanged(playerInput);
    }

    private void OnControlsChanged(PlayerInput input)
    {
        usingGamepad = input.currentControlScheme.Equals("Controller");
        Cursor.visible = !usingGamepad;

        if (usingGamepad)
        {
            foreach (Button button in FindObjectsOfType<Button>())
            {
                if (button.gameObject.activeInHierarchy)
                {
                    button.Select();
                    break;
                }
            }
        }
    }

    private void OnDisable()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
        playerInput.actions["Any Action"].performed -= TriggerControlsCheck;
    }
}
