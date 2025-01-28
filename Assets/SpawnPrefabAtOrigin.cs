using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabAtOrigin : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public bool unparent;
    public bool spawnOnEnable;
    public bool spawnOnDisable;

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
    }

    public void SpawnPrefab()
    {
       GameObject newprefab = Instantiate(prefabToSpawn);
       newprefab.transform.position = transform.position;
        if (unparent)
        {
            newprefab.transform.parent = null;
        }
    }
}
