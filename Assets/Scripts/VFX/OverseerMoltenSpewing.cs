using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerMoltenSpewing : MonoBehaviour
{
    public GameObject moltenPrefab;
    public Transform moltenSpawnPos;
    public ParticleSystem moltenParticle;

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
        moltenParticle.Emit(5);
        GameObject newMolten = GameObject.Instantiate(moltenPrefab);
        newMolten.transform.position = moltenSpawnPos.position;
        newMolten.transform.parent = null;
    }
}
