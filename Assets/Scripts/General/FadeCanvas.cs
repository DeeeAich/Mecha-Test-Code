using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeCanvas : MonoBehaviour
{
    public static FadeCanvas instance;
    public List<Animator> loadGridAnims;

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


        foreach (var anim in loadGridAnims)
        {
            anim.SetBool("exit", false);
        }
    }

    public void FadeToBlack()
    {
        foreach (var anim in loadGridAnims)
        {
            anim.SetBool("exit", true);
        }


        GetComponent<Animator>().Play("Fade Canvas Out");

        AudioManager.instance.ChangeMusicTrack(musicTrack.None);
        AudioManager.instance.ChangeAmbienceTrack(ambienceTrack.None);

    }
}
