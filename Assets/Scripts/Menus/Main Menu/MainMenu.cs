using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    float timer = 0f;
    [SerializeField] private int sceneForTutorial = 2;
    [SerializeField] private int sceneForSkipTutorial = 5;
    private bool skipTutorial;
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > 5f)
        {
            UnityEngine.UI.Button selected = GetComponentInChildren<UnityEngine.UI.Button>();
            selected.Select();
            timer = 0f;
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
}
