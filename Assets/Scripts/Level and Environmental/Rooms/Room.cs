using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    public bool isActive;

    [Header("Objectives")]
    public GameObject[] possiblePrimaryObjectives;
    public GameObject[] possibleSecondaryObjectives;
    public Objective primaryObjective;
    public Objective secondaryObjective;

    [Header("Public References")]
    public GameObject[] enemySpawnPoints;
    public WaveSpawner[] waveSpawners;
    public CaptureZone[] captureZones;

    [Header("Internal References")]
    public GameObject[] nextRooms;
    [SerializeField] private Door entryDoor;
    [SerializeField] private Door[] exitDoors;
    [SerializeField] private GameObject[] nextRoomSpawnPoints;

    [Header("Events")] 
    public UnityEvent onStartRoom;
    public UnityEvent onCompleteRoom;

    private void Start()
    {
        if (exitDoors.Length > 0)
        {
            nextRooms = LevelGenerator.instance.NextRoomSelection(exitDoors.Length);

            for (int i = 0; i < exitDoors.Length; i++)
            {
                for (int j = 0; j < exitDoors.Length; j++)
                {
                    if (j != i)
                    {
                        int index = j;
                        exitDoors[i].onOpen.AddListener(delegate { exitDoors[index].LockDoor();
                        });
                    }
                }

                int exitIndex = i;
                exitDoors[i].onOpen.AddListener(delegate { LevelGenerator.instance.SpawnRoom(nextRooms[exitIndex], nextRoomSpawnPoints[exitIndex]); });
            }
        }
        
    }
    

    public void startRoom()
    {
        Debug.Log("Starting Room: " + name);
        
        entryDoor.CloseDoor();
        entryDoor.LockDoor();
        
        for (int i = 0; i < exitDoors.Length; i++)
        {
            exitDoors[i].CloseDoor();
            exitDoors[i].LockDoor();
        }
        
        if(primaryObjective == null) primaryObjective = Instantiate(possiblePrimaryObjectives[LevelGenerator.instance.seededRandom.Next(0, possiblePrimaryObjectives.Length)], transform).GetComponent<Objective>();
        primaryObjective.onComplete.AddListener(completeRoom);

        isActive = true;
        onStartRoom.Invoke();
    }

    public void completeRoom()
    {
        if (isActive)
        {
            onCompleteRoom.Invoke();
            
            for (int i = 0; i < exitDoors.Length; i++)
            {
                exitDoors[i].UnlockDoor();
            }
            
            Debug.Log("Finished Room: " + gameObject.name);
        }
    }
}
