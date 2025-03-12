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
    public bool isPlayingAuido;

    private EventInstance eventInstance;

    public void LoopingAudio(int trueFalse)
    {
        if (!isPlayingAuido)
            return;

        if (trueFalse == 0)
        {
            if (!isLooping)
            {
                eventInstance = RuntimeManager.CreateInstance(sound);
                RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
                eventInstance.start();
                isLooping = true;
                Debug.Log(gameObject.name + " started looping audio");
            }
        }
        else
        {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isLooping = false;
        Debug.Log(gameObject.name + " stopped looping audio");
        }
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
