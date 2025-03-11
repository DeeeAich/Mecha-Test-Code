using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullPrimaryObjective : Objective
{
    [SerializeField] private float timeToTriggerComplete = 1;

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
