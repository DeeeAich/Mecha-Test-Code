using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CaptureZone : MonoBehaviour
{
    public UnityEvent onCapture;
    public bool isCaptured;
    public float captureProgress;
    public float captureProgressPerSecond = 0.01f;

    public bool playerPresent;
    public int enemiesPresent;

    [SerializeField] private LayerMask characterLayers;
    

    private void FixedUpdate()
    {
        if (playerPresent && enemiesPresent == 0)
        {
            captureProgress += captureProgressPerSecond * Time.fixedDeltaTime;
        }

        if (captureProgress >= 1)
        {
            isCaptured = true;
            onCapture.Invoke();
        }
        
        enemiesPresent = 0;
        playerPresent = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerBody>()) playerPresent = true;

        if (other.gameObject.layer == 9) enemiesPresent++;
    }
}
