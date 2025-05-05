using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class OverseerMoltenSpewing : MonoBehaviour
{
    public GameObject moltenPrefab;
    public Transform moltenSpawnPos;
    public ParticleSystem moltenParticle;
    public EventReference moltenSpewSFX;

    public float spawnRate;
    public bool isSpawning;
    private float spawnTime;

    void FixedUpdate()
    {
        if (isSpawning)
        {
            if (spawnTime < spawnRate)
            {
                spawnTime += Time.deltaTime;
            }
            else
            {
                SpawnMolten();
                spawnTime = 0;
            }
        }
    }


    public void SpawnMolten()
    {
        AudioManager.instance.PlayOneShotSFX(moltenSpewSFX, transform.position);
        moltenParticle.Emit(5);
        GameObject newMolten = GameObject.Instantiate(moltenPrefab);
        newMolten.transform.position = moltenSpawnPos.position;
        newMolten.transform.parent = null;
    }
}
