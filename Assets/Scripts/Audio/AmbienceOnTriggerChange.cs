using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceOnTriggerChange : MonoBehaviour
{
    public ambienceTrack ambienceTrackEnter;
    public ambienceTrack ambienceTrackExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.ChangeAmbienceTrack(ambienceTrackEnter);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.ChangeAmbienceTrack(ambienceTrackExit);
        }
    }
}
