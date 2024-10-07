using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CaptureZone : MonoBehaviour
{
    public UnityEvent onCapture;
    public float captureProgress;

    public bool playerPresent;
    public int enemiesPresent;
    
    [SerializeField]

    private void FixedUpdate()
    {
        
    }
}
