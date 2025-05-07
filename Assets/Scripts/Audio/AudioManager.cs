using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Slider = UnityEngine.UI.Slider;


public enum musicState
{
    idle,
    combat
}

public enum musicTrack
{
    None,
    Level2Combat,
    Tutorial
}

public enum ambienceTrack
{
    None,
    Level2Ambience,
    OverseerAmbience
}

[Serializable]
public struct SceneStartingAudio
{
    public string Scene;
    public musicState stateToPlay;
    public musicTrack trackToPlay;
    public ambienceTrack ambienceTrackToPlay;
}

public class AudioManager : MonoBehaviour
{
    public UnityEngine.UI.Slider MasterVolumeSlider;
    
    public static AudioManager instance;

    private FMOD.Studio.VCA MusicVca;
    private FMOD.Studio.VCA SfxVca;
    private FMOD.Studio.VCA MasterVca;

    public float MusicVolume = 1;
    public float SFXVolume = 1;
    public float MasterVolume = 1f;

    [Header("Music")]
    public musicTrack currentMusicTrack;
    public musicState currentMusicState;
    private EventInstance currentMusic;
    
    public EventReference Level2BaseMusic;
    public EventReference tutorialMusic;
    
    
    public SceneStartingAudio[] startingAudios;


    [Header("Ambience")]
    public ambienceTrack currentAmbienceTrack;
    public EventInstance currentAmbience;
    public EventReference Level2BaseAmbience;
    public EventReference overseerAmbience;


    [Header("Dialogue")]
    public DialogueOneShot dialogueOneLiner;

    /*
    [Header("Scenes")]
    public string level2SceneName;
    public string tutorialSceneName;
    public string mainMenuSceneName;
    */
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

        MusicVca = FMODUnity.RuntimeManager.GetVCA("vca:/MUSIC VCA");
        SfxVca = FMODUnity.RuntimeManager.GetVCA("vca:/SFX VCA");
        MasterVca = FMODUnity.RuntimeManager.GetVCA("vca:/MASTER VCA");

        MusicVca.getVolume(out MusicVolume);
        MasterVca.getVolume(out MasterVolume);
        SfxVca.getVolume(out SFXVolume);

        if(MasterVolumeSlider != null) MasterVolumeSlider.value = MasterVolume;
    }



    private void OnEnable()
    {
        bool hasSpecifiedStartingAudio = false;
        for (int i = 0; i < startingAudios.Length; i++)
        {
            if (startingAudios[i].Scene == SceneManager.GetActiveScene().name)
            {
                PlayStartingAudio(startingAudios[i]);
                hasSpecifiedStartingAudio = true;
                break;
            }
        }

        if (!hasSpecifiedStartingAudio)
        {
            PlayStartingAudio(startingAudios[0]);
        }
        
        /*
        if (SceneManager.GetActiveScene().name.ToString() == level2SceneName)
        {
            ChangeMusicTrack(musicTrack.Level2Combat);
            ChangeMusicState(musicState.idle);
            ChangeAmbienceTrack(ambienceTrack.Level2Ambience);
        }
        if (SceneManager.GetActiveScene().name.ToString() == tutorialSceneName)
        {
            ChangeMusicTrack(musicTrack.Tutorial);
            ChangeAmbienceTrack(ambienceTrack.None);
        }
        if (SceneManager.GetActiveScene().name.ToString() == mainMenuSceneName)
        {
            ChangeMusicTrack(musicTrack.Level2Combat);
            ChangeMusicState(musicState.combat);
            ChangeAmbienceTrack(ambienceTrack.None);
        }
        */
    }

    private void PlayStartingAudio(SceneStartingAudio audio)
    {
        ChangeMusicState(audio.stateToPlay);
        ChangeMusicTrack(audio.trackToPlay);
        ChangeAmbienceTrack(audio.ambienceTrackToPlay);
    }

    public void PlayOneShotSFX(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void ChangeMusicTrack(musicTrack newTrack)
    {

        currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Debug.Log("started " + currentMusicTrack);


        switch (newTrack)
        {
            case musicTrack.Level2Combat:

                currentMusic = RuntimeManager.CreateInstance(Level2BaseMusic);
                currentMusic.start();
                break;

            case musicTrack.Tutorial:
                currentMusic = RuntimeManager.CreateInstance(tutorialMusic);
                currentMusic.start();
                break;
        }

        currentMusicTrack = newTrack;
        Debug.Log("started " + newTrack.ToString());
    }

    public void ChangeMusicState(musicState newState)
    {
        switch (newState)
        {
            case musicState.idle:               
                RuntimeManager.StudioSystem.setParameterByName("inCombat", 0);
                
                break;
            case musicState.combat:
                RuntimeManager.StudioSystem.setParameterByName("inCombat", 1);
                break;
        }
        currentMusicState = newState;
    }
    
    public void ChangeAmbienceTrack(ambienceTrack newTrack)
    {
        currentAmbience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        switch (newTrack)
        {
            case ambienceTrack.Level2Ambience:
                currentAmbience = RuntimeManager.CreateInstance(Level2BaseAmbience);
                currentAmbience.start();
                break;
                
            case ambienceTrack.OverseerAmbience:
                currentAmbience = RuntimeManager.CreateInstance(overseerAmbience);
                currentAmbience.start();
                break;

        }
        currentAmbienceTrack = newTrack;
        Debug.Log(newTrack.ToString());
    }

    public void ChangeGameMasterVoluem(Slider volumeSlider)
    {
        MasterVolume = volumeSlider.value;
        MasterVca.setVolume(MasterVolume);
    }
}
