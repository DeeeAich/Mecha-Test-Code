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
        StartCoroutine(StartGameFade());    
    }

    IEnumerator StartGameFade()
    {
        FadeCanvas.instance.FadeToBlack();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }
}
