using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisManager : MonoBehaviour
{
    [SerializeField] private int maxDebris = 100;
    public static DebrisManager instance;
    private GameObject[] debris;
    private int currentIndex;

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
    }

    public void AddDebris(GameObject newDebris)
    {
        if (debris[currentIndex] != null) Destroy(debris[currentIndex]);
        debris[currentIndex] = newDebris;
        currentIndex++;
        if (currentIndex >= maxDebris) currentIndex = 0;
    }
}
