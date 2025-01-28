using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public enum musicState
{
    idle,
    combat
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music")]
    public musicState musicState;
    public EventReference combatMusic;
    public EventReference nonCombatMusic;
    public EventInstance currentMusic;

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
       RuntimeManager.PlayOneShot(ambience);
        ChangeMusicState(musicState.idle);

    }

    public void PlayOneShotSFX(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void ChangeMusicState(musicState newState)
    {
        switch (newState)
        {
            case musicState.idle:
                Debug.Log("idle");
                currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                currentMusic = RuntimeManager.CreateInstance(nonCombatMusic);
                currentMusic.start();
                break;
            case musicState.combat:
                Debug.Log("combat");
                currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                currentMusic = RuntimeManager.CreateInstance(combatMusic);
                currentMusic.start();
                break;
        }
    }
}
