using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class OverseerFightPrimaryObjective : Objective
{
    public UnityEvent Phase2Start;
    public UnityEvent<bool> Phase2End;
    
    [FormerlySerializedAs("bossWaveSpawner")] [SerializeField] private WaveSpawner bossWaveSpawnerInScene;
    [SerializeField] private GameObject phase2WaveSpawnerPrefab;

    [SerializeField] private float phaseTransitionTime = 10;
    private float phaseTransitionTimer;

    private float postPhaseTransitionWaveSpawnerStartTimer;

    private Health bossHealth;
    private bool hasRegisteredEvents = false;

    private void StartPhaseTransition()
    {
        PlayerBody.Instance().StopParts(false,false);
        
        for (int i = 0; i < room.waveSpawners.Length; i++)
        {
            if (room.waveSpawners[i] != bossWaveSpawnerInScene)
            {
                room.waveSpawners[i].StopSpawning();
            }
        }
        
        Phase2Start.Invoke();
    }

    private void StopPhaseTransition()
    {
        PlayerBody.Instance().StopParts(true, true);

        room.waveSpawners = new WaveSpawner[]
            {bossWaveSpawnerInScene, Instantiate(phase2WaveSpawnerPrefab, room.transform).GetComponent<WaveSpawner>()};
        
        Phase2End.Invoke(true);
    }

    private void FixedUpdate()
    {
        if (bossHealth == null)
        {
            if (!bossWaveSpawnerInScene.isComplete && bossWaveSpawnerInScene.spawnedEnemies.Count > 0)
            {
                if (bossWaveSpawnerInScene.spawnedEnemies[0].TryGetComponent(out bossHealth))
                {
                    bossHealth = FindObjectOfType<OverseerBT>().GetComponent<Health>();
                    bossHealth.GetComponent<OverseerBT>().onPhaseTransition.AddListener(StartPhaseTransition);
                }
            }
        }
        else
        {
            progressBar.fillAmount = bossHealth.health / bossHealth.maxHealth;
            
            if (bossHealth != null && bossHealth.health <= 0)
            {
                TriggerComplete();
            }
        }
        
        if (phaseTransitionTimer > 0)
        {
            phaseTransitionTimer -= Time.fixedDeltaTime;

            if (phaseTransitionTimer <= 0)
            {
                StopPhaseTransition();
                postPhaseTransitionWaveSpawnerStartTimer = 1;
            }
        }

        if (postPhaseTransitionWaveSpawnerStartTimer > 0)
        {
            postPhaseTransitionWaveSpawnerStartTimer -= Time.fixedDeltaTime;

            if (postPhaseTransitionWaveSpawnerStartTimer <= 0)
            {
                room.waveSpawners[1].StartSpawning();
            }
        }
    }
}
