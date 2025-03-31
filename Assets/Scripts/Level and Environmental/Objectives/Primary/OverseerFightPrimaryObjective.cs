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
        ToggleGameFrozenForPhaseTransition(true);
        
        phaseTransitionTimer = phaseTransitionTime;
        
        Phase2Start.Invoke();
    }

    private void StopPhaseTransition()
    {
        ToggleGameFrozenForPhaseTransition(false);
        
        room.waveSpawners = new WaveSpawner[] {bossWaveSpawnerInScene, Instantiate(phase2WaveSpawnerPrefab, room.transform).GetComponent<WaveSpawner>()};

        Phase2End.Invoke(true);
    }

    private void ToggleGameFrozenForPhaseTransition(bool freeze)
    {
        PlayerBody.Instance().StopParts(!freeze,!freeze);

        if (freeze)
        {
            for (int i = 0; i < room.waveSpawners.Length; i++)
            {
                if (room.waveSpawners[i] != bossWaveSpawnerInScene)
                {
                    room.waveSpawners[i].StopSpawning(true);
                }
            }
        }
        
        foreach (EnemyGun gun in FindObjectsOfType<EnemyGun>(true))
        {
            if (freeze)
            {
                gun.enabled = false;
                gun.StopAllCoroutines();
            }
            else
            {
                gun.enabled = true;
                gun.StartCoroutine(gun.FireOnRepeat());
            }
        }
        
        foreach (BasicBullet bullet in FindObjectsOfType<BasicBullet>(true))
        {
            if (freeze)
            {
                bullet.StopAllCoroutines();
                if (bullet.gameObject.activeSelf) bullet.StartCoroutine(bullet.AnimationTimer());
                
            }
            else
            { 
                if (bullet.gameObject.activeSelf)  bullet.StartCoroutine(bullet.AutoReset());
            }
        }
        
        foreach (MoveProjectile bullet in FindObjectsOfType<MoveProjectile>(true))
        {
            if (freeze)
            {
                Destroy(bullet);
            }
        }
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
