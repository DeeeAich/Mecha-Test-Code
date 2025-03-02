using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private bool EditorNextStage;
    
    [SerializeField] private int tutorialStage = 0;
    public UnityEvent[] onStageStarts;
    public UnityEvent[] onStageCompletes;

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

    public void StartTutorialStage(int stage)
    {
        tutorialStage = stage;

        switch (tutorialStage)
        {
            case 0:
                // player left shoot
                break;
            
            case 2:
                // player right shoot
                
                break;
            
        }
        
        onStageStarts[stage].Invoke();
    }
}
