using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    float timer = 0f;
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
        GameGeneralManager.instance.ChangeScene(1);
    }

    public void LoadSave(int index)
    {
        SaveData.instance.LoadOrCreateFile(index);
    }
}
