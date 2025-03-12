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

    private EventInstance eventInstance;

    public void StartLoopingAudio()
    {
        if (!isLooping)
        {
            eventInstance = RuntimeManager.CreateInstance(sound);
            RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
            eventInstance.start();
            isLooping = true;
        }
    }
    public void StopLoopingAudio()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isLooping = false;
    }

    private void OnDestroy()
    {
        if (isLooping)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
    private void OnDisable()
    {
        if (isLooping)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
