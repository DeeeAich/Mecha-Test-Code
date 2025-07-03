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
    
    [SerializeField] private WaveSpawner bossWaveSpawnerInScene;
    [SerializeField] private GameObject phase1WaveSpawnerPrefab;
    [SerializeField] private GameObject phase2WaveSpawnerPrefab;

    [SerializeField] private EnemySpawnPoint[] phase1SpawnPoints;
    [SerializeField] private EnemySpawnPoint[] phase2SpawnPoints;

    [SerializeField] private float phaseTransitionTime = 10;
    private float phaseTransitionTimer;

    private float postPhaseTransitionWaveSpawnerStartTimer;

    private Health bossHealth;
    private bool hasRegisteredEvents = false;

    public int currentPhase;

    private void Start()
    {
        if (room.spawnedLoot != null)
        {
            currentPhase = 0;
            //room.spawnedLoot.gameObject.SetActive(false);
        }

        room.enemySpawnPoints = phase1SpawnPoints;
        room.waveSpawners = new WaveSpawner[] {bossWaveSpawnerInScene, Instantiate(phase1WaveSpawnerPrefab, room.transform).GetComponent<WaveSpawner>()};
        room.waveSpawners[1].StartSpawning();
    }

    private void StartPhaseTransition()
    {
        if(currentPhase != 0) return;
        
        ToggleGameFrozenForPhaseTransition(true);
        
        room.waveSpawners[1].StopSpawning(true);

        phaseTransitionTimer = phaseTransitionTime;
        
        Phase2Start.Invoke();
    }

    private void StopPhaseTransition()
    {
        ToggleGameFrozenForPhaseTransition(false);
        
        Destroy(room.waveSpawners[1].gameObject);
        room.enemySpawnPoints = phase2SpawnPoints;
        room.waveSpawners = new WaveSpawner[] {bossWaveSpawnerInScene, Instantiate(phase2WaveSpawnerPrefab, room.transform).GetComponent<WaveSpawner>()};
        room.waveSpawners[1].StartSpawning();

        if(bossHealth != null) bossHealth.canTakeDamage = true;

        if (room.spawnedLoot != null)
        {
            //room.spawnedLoot.gameObject.SetActive(true);
        }
        
        Phase2End.Invoke(true);
        currentPhase = 1;
    }

    private void ToggleGameFrozenForPhaseTransition(bool freeze)
    {
        PlayerBody.Instance().PauseSystem(PlayerSystems.AllParts, freeze);

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
                //if (bullet.gameObject.activeSelf)  bullet.StartCoroutine(bullet.AutoReset());
            }
        }
        
        foreach (MoveProjectile bullet in FindObjectsOfType<MoveProjectile>(true))
        {
            if (freeze)
            {
                Destroy(bullet.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isComplete)
        {

            if (bossWaveSpawnerInScene.isComplete)
            {
                TriggerComplete();
            }
            else
            {
                if (bossHealth == null)
                {
                    if (!bossWaveSpawnerInScene.isComplete && bossWaveSpawnerInScene.spawnedEnemies.Count > 0)
                    {
                        if (bossWaveSpawnerInScene.spawnedEnemies[0].TryGetComponent(out bossHealth))
                        {
                            bossHealth = FindObjectOfType<OverseerBT>().GetComponent<Health>();
                            bossHealth.GetComponent<OverseerBT>().onPhaseTransition.AddListener(StartPhaseTransition);
                            bossHealth.onDeath.AddListener(StartPhaseTransition);
                        }
                    }
                }
                else
                {
     
                    progressBar.fillAmount = bossHealth.health / bossHealth.maxHealth;
            
                    /*
                    if (bossHealth != null && bossHealth.health <= 0)
                    {
                        TriggerComplete();
                    }
                    */
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
}
