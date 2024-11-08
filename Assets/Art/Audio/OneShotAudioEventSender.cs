using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class OneShotAudioEventSender : MonoBehaviour
{
    public List<EventReference> sound;

    public void PlayOneShot(int soundNumber)
    {
        AudioManager.instance.PlayOneShotSFX(sound[soundNumber], this.transform.position);
    }
}
