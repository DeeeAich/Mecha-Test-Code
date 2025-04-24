using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeCanvas : MonoBehaviour
{
    public static FadeCanvas instance;
    public List<LoadGridToggle> loadGridAnims;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }


        for (var i = 0; i < loadGridAnims.Count; i++)
        {
            loadGridAnims[i].exit = false;
        }
    }

    public void FadeToBlack()
    {

        for (var i = 0; i < loadGridAnims.Count; i++)
        {
            loadGridAnims[i].exit = true;
        }


        GetComponent<Animator>().Play("Fade Canvas Out");

        AudioManager.instance.ChangeMusicTrack(musicTrack.None);
        AudioManager.instance.ChangeAmbienceTrack(ambienceTrack.None);

    }
}
