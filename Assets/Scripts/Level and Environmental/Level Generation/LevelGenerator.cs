using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class LevelGenerator : MonoBehaviour
{
    public int floor;
    public int roomIndex;
    public int roomsInThisFloor;
    public int[] minibossRoomIndexes;

    public RoomPoolScriptable roomPool;
    
    [SerializeField] private GameObject StartPosition;

    [SerializeField] private int randomSeed;
    [SerializeField] private bool randomizeSeedOnAwake;
    private Random seededRandom;

    private void Awake()
    {
        if (randomizeSeedOnAwake)
        {
            randomSeed = Mathf.CeilToInt(System.DateTime.Now.Ticks);
            seededRandom = new Random(randomSeed);
        }
    }

    public void StartGeneratingLevel()
    {
        CreateRoom(StartPosition.transform.position);
    }

    public void CreateRoom(Vector3 position)
    {
        
    }
}
