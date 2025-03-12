using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureZonesPrimaryObjective : Objective
{
    private float totalProgress;
    
    private CaptureZone[] zones;
    private int zonesCaptured;

    private void Start()
    {
        zones = room.captureZones;
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i].gameObject.SetActive(true);
        }

        foreach (var waveSpawner in FindObjectsOfType<WaveSpawner>())
        {
            waveSpawner.looping = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isComplete)
        {
            zonesCaptured = 0;
            totalProgress = 0;
            
            for (int i = 0; i < zones.Length; i++)
            {
                totalProgress += zones[i].captureProgress / zones.Length;
                if (zones[i].isCaptured) zonesCaptured++;
            }

            progressBar.fillAmount = totalProgress;

            if (zonesCaptured == zones.Length)
            {
                TriggerComplete();
            }
        }
    }
}
