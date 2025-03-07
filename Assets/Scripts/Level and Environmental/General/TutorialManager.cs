using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private bool EditorNextStage;
    
    [SerializeField] private int tutorialStage = 0;
    public UnityEvent[] onStageStarts;
    public UnityEvent[] onStageCompletes;

    private InputAction dash;

    private void FixedUpdate()
    {
        if (EditorNextStage)
        {
            CompleteTutorialStage();
            StartTutorialStage(tutorialStage +1 );
            EditorNextStage = false;
        }

    }

    public void CompleteTutorialStage()
    {
        onStageCompletes[tutorialStage].Invoke();
    }

    private void CompleteDashChallenge(InputAction.CallbackContext context)
    {
        CompleteTutorialStage();
        dash.performed -= CompleteDashChallenge;
    }

    public void StartTutorialStage(int stage)
    {
        tutorialStage = stage;

        switch (tutorialStage)
        {
            case 0:
                PlayerBody.Instance().weaponHolder.rightFire.AddListener(delegate
                {
                    CompleteTutorialStage();
                    PlayerBody.Instance().weaponHolder.rightFire.RemoveAllListeners();
                });
                break;
            
            case 1:
                dash = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Dash"];
                dash.performed += CompleteDashChallenge;
                break;
            
            case 2:
                
                PlayerBody.Instance().weaponHolder.leftFire.AddListener(delegate
                {
                    CompleteTutorialStage();
                    PlayerBody.Instance().weaponHolder.leftFire.RemoveAllListeners();
                });
                
                break;
            
        }
        
        onStageStarts[stage].Invoke();
    }
}
