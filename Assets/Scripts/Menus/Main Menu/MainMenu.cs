using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{


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
