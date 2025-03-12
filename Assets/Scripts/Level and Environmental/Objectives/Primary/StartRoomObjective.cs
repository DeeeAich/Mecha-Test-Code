using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoomObjective : Objective
{
    [SerializeField] private float timeToTriggerComplete = 0.25f;
    private void Start()
    {
        room.startRoom();
    }

    private void FixedUpdate()
    {
        if (timeToTriggerComplete > 0)
        {
            timeToTriggerComplete -= Time.fixedDeltaTime;
            if (timeToTriggerComplete <= 0)
            {
                TriggerComplete();
            }
        }
    }
}
