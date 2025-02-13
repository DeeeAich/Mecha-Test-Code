using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UIElements;


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
    public EventReference Level2BaseMusic;
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
        currentMusic = RuntimeManager.CreateInstance(Level2BaseMusic);
        currentMusic.start();
        ChangeMusicState(musicState.idle);

    }

    public void PlayOneShotSFX(EventReference sound, Vector3 worldPos, string parameterName = "none", float parameterValue = 0)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);

        var instance = RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPos));
        if (parameterName != "none")
        {
            instance.setParameterByName(parameterName, parameterValue);
        }
        instance.start();
        instance.release();

    }

    public void ChangeMusicState(musicState newState)
    {
        switch (newState)
        {
            case musicState.idle:
                Debug.Log("idle");
                RuntimeManager.StudioSystem.setParameterByName("inCombat", 0);
                
                break;
            case musicState.combat:
                Debug.Log("combat");
                RuntimeManager.StudioSystem.setParameterByName("inCombat", 1);
                break;
        }
    }
}
