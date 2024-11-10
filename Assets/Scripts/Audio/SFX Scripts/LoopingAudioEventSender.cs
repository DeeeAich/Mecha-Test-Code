using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class LoopingAudioEventSender : MonoBehaviour
{
    [Header("Sound")]
    public EventReference sound;
    public bool isLooping;


    [Header("If speed changes (optional)")]
    public Animator animatorResponsible;
    public string nameOfSpeedFloat;

    private EventInstance eventInstance;

    public void StartLoopingAudio()
    {
        if (!isLooping)
        {
            eventInstance = RuntimeManager.CreateInstance(sound);
            RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
            eventInstance.start();
            isLooping = true;
            if (animatorResponsible != null)
            {
                eventInstance.setParameterByName("Action Speed", animatorResponsible.GetFloat(nameOfSpeedFloat));
            }
        }
    }
    public void StopLoopingAudio()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
        isLooping = false;
    }

    private void OnDestroy()
    {
        if (isLooping)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }
}
