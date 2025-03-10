using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeCanvas : MonoBehaviour
{
    public static FadeCanvas instance;

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
    }

    public void FadeToBlack()
    {
        GetComponent<Animator>().Play("Fade Canvas Out");

        AudioManager.instance.ChangeMusicTrack(musicTrack.None);
        AudioManager.instance.ChangeAmbienceTrack(ambienceTrack.None);

    }
}
