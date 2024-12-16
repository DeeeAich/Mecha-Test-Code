using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public enum MusicState
{
    idle,
    combat,
    muffled,
    stopImmediately,
    fadeOut
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music")]
    public MusicState musicState;
    public EventReference music;

    [Header("Ambience")]
    public EventReference ambience;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
       RuntimeManager.PlayOneShot(music);
       RuntimeManager.PlayOneShot(ambience);

    }

    public void PlayOneShotSFX(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void ChangeMusicState(MusicState newState)
    {
        
    }
}
