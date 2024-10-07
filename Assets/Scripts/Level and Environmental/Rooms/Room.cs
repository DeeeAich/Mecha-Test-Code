using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    public bool isActive;
    public GameObject[] nextRooms;

    [SerializeField] private Objective[] possibleObjectives;
    public Objective primaryObjective;
    public Objective secondaryObjective;

    [Header("References")]
    [SerializeField] private Door entryDoor;
    [SerializeField] private Door[] exitDoors;
    [SerializeField] private GameObject[] nextRoomSpawnPoints;
    public GameObject[] enemySpawnPoints;
    
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
        
        primaryObjective.onComplete.AddListener(completeRoom);
        
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
