using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    [SerializeField] private bool setTargetSceneToNextInIndex = true;
    [SerializeField] public int targetScene;
    
    [SerializeField] private float sceneTransitionTime;
    [SerializeField] private float sceneTransitionTimer;

    private bool transitioning;

    private void Awake()
    {
        if (setTargetSceneToNextInIndex)
        {
            targetScene = SceneManager.GetActiveScene().buildIndex + 1;
        }
    }

    public void StartSceneTransition()
    {
        if (!transitioning)
        {
            PlayerBody.PlayBody().StopParts(true, true);
            sceneTransitionTimer = sceneTransitionTime;
            transitioning = true;
        }
    }

    private void FixedUpdate()
    {
        if (sceneTransitionTimer > 0)
        {
            sceneTransitionTimer -= Time.fixedDeltaTime;
            if (sceneTransitionTimer <= 0)
            {
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}
