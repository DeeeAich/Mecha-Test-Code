using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootingRange : MonoBehaviour
{
    public UnityEvent onComplete;
    public UnityEvent onFirstTargetDeath;
    
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject[] spawnPoints;

    [Header("Internal")]
    [SerializeField] private List<Health> spawnedTargets;
    [SerializeField] private bool firstTargetDead;

    public void SpawnTargets()
    {
        spawnedTargets = new List<Health>();
        
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnedTargets.Add(Instantiate(target, spawnPoints[i].transform).GetComponent<Health>());
            spawnedTargets[^1].onDeath.AddListener(UpdateTargetsList);
        }
    }

    public void UpdateTargetsList()
    {
        for (int i = 0; i < spawnedTargets.Count; i++)
        {
            if (spawnedTargets[i] == null || spawnedTargets[i].health <= 0)
            {
                if (firstTargetDead == false)
                {
                    firstTargetDead = true;
                    onFirstTargetDeath.Invoke();
                }
                spawnedTargets.RemoveAt(i);
                break;
            }
        }

        if (spawnedTargets == null || spawnedTargets.Count <= 0)
        {
            Debug.Log("Completed Shooting Range");
            onComplete.Invoke();
        }
    }
}
