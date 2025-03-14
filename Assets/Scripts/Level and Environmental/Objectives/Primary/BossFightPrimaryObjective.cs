using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightPrimaryObjective : Objective
{
    [SerializeField] private Health bossHealth;
    
    
    private void Update()
    {
        progressBar.fillAmount = bossHealth.health / bossHealth.maxHealth;
    }

    private void FixedUpdate()
    {
        if (bossHealth == null || bossHealth.health <= 0)
        {
            TriggerComplete();
        }
    }
}
