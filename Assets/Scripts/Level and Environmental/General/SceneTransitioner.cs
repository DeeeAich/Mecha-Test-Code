using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public Scene nextScene;
    
    [SerializeField] private float sceneTransitionTime;
    [SerializeField] private float sceneTransitionTimer;

    public void StartSceneTransition()
    {
        PlayerBody.PlayBody().StopParts(true, true);
        sceneTransitionTimer = sceneTransitionTime;
    }

    private void FixedUpdate()
    {
        if (sceneTransitionTimer > 0)
        {
            sceneTransitionTimer -= Time.fixedDeltaTime;
            if (sceneTransitionTimer <= 0)
            {
                SceneManager.LoadScene(nextScene.name);
            }
        }
    }
}
