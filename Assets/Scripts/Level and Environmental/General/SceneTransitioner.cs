using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    [SerializeField] private bool setTargetSceneToNextInIndex = true;
    [SerializeField] public int targetScene;
    [SerializeField] private bool isCutscene = false;

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
            if (!isCutscene)
            {
                PlayerBody.Instance().PauseSystem(PlayerSystems.AllParts, false);
                if (GameGeneralManager.instance != null)
                {
                    GameGeneralManager.instance.ChangeScene(targetScene);
                }
                else
                {
                    SceneManager.LoadScene(targetScene);
                }
            }
            else
            {
                SceneManager.LoadScene(targetScene);
            }
            
            transitioning = true;
        }
    }
}
