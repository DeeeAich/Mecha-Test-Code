using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabAtOrigin : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public bool unparent;
    public bool spawnOnEnable;
    public bool spawnOnDisable;
    public Transform[] missileLocations;
    /*
    private void OnEnable()
    {
        if (spawnOnEnable)
        {
            SpawnPrefab();
        }
    }
    private void OnDisable()
    {
        if (spawnOnDisable)
        {
            SpawnPrefab();
        }
    }*/

    public void SpawnPrefab(int missileNum)
    {
       GameObject newprefab = Instantiate(prefabToSpawn, missileLocations[missileNum].position, transform.rotation);
    }
}
