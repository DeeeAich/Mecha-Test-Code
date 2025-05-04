using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisManager : MonoBehaviour
{
    [SerializeField] private int maxDebris = 100;
    [SerializeField] private float debrisDestroyTime = 1f;
    [SerializeField] private float debrisSinkDistance = 4f;
    
    public static DebrisManager instance;
    private GameObject[] debris;
    private int currentIndex;
    
    private List<GameObject> debrisGettingDestroyed;
    private List<float> debrisGettingDestroyedTimers;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        debris = new GameObject[maxDebris];
        debrisGettingDestroyed = new List<GameObject>();
        debrisGettingDestroyedTimers = new List<float>();
    }

    public void AddDebris(GameObject newDebris)
    {
        if (debris[currentIndex] != null)
        {
            if (debris[currentIndex].TryGetComponent(out Rigidbody rb))
            {
                Destroy(rb);
            }
            
            debrisGettingDestroyed.Add(debris[currentIndex]);
            debrisGettingDestroyedTimers.Add(debrisDestroyTime);
        }
        
        
        debris[currentIndex] = newDebris;
        currentIndex++;
        if (currentIndex >= maxDebris) currentIndex = 0;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < debrisGettingDestroyed.Count; i++)
        {
            debrisGettingDestroyedTimers[i] -= Time.fixedDeltaTime;

            if (debrisGettingDestroyedTimers[i] <= 0)
            {
                Destroy(debrisGettingDestroyed[i]);
                debrisGettingDestroyed.RemoveAt(i);
                debrisGettingDestroyedTimers.RemoveAt(i);
            }
            else
            {
                debrisGettingDestroyed[i].transform.position -= Vector3.up * Time.fixedDeltaTime * debrisSinkDistance/debrisDestroyTime;
            }
        }
    }
}
