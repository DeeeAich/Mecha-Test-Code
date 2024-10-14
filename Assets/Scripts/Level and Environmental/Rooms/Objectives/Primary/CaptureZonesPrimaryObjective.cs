using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureZonesPrimaryObjective : Objective
{
    private CaptureZone[] zones;
    private int zonesCaptured;

    private void Start()
    {
        zones = room.captureZones;
    }

    private void FixedUpdate()
    {
        if (!isComplete)
        {
            zonesCaptured = 0;
            for (int i = 0; i < zones.Length; i++)
            {
                if (zones[i].isCaptured) zonesCaptured++;
            }

            if (zonesCaptured == zones.Length)
            {
                isComplete = true;
                onComplete.Invoke();
            }
        }
    }
}
