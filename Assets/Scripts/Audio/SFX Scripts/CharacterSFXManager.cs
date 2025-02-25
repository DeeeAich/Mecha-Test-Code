using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class CharacterSFXManager : MonoBehaviour
{


    public EventReference enableSound;
    public EventReference disableSound;

    public EventReference idleSound;
    public bool hasIdleSound = false;
    EventInstance idleSoundInstance;


    private void Start()
    {
        idleSoundInstance = RuntimeManager.CreateInstance(idleSound);
        RuntimeManager.AttachInstanceToGameObject(idleSoundInstance, gameObject);
    }
    public void SwitchIdleSFX(int state)
    {
        idleSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        idleSoundInstance.setParameterByName("IdleState", state);
        idleSoundInstance.start();
    }

    public void StopIdleSound()
    {
        idleSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    private void OnEnable()
    {
        SwitchIdleSFX(0);
        AudioManager.instance.PlayOneShotSFX(enableSound, transform.position);
    }
    private void OnDisable()
    {
        StopIdleSound();
        AudioManager.instance.PlayOneShotSFX(disableSound, transform.position);
    }    
}
