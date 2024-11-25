using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum ObjectiveType
{
    exterminate,
    survival,
    capturePoint
}

public class Room : MonoBehaviour
{
    [Header("~~~~~~~~~~~ Required References ~~~~~~~~~~~")]
    [SerializeField] private Door entryDoor;
    [SerializeField] private ObjectiveType[] possiblePrimaryObjectives;
    [SerializeField] private ObjectiveType[] possibleSecondaryObjectives;
    [SerializeField] private GameObject[] nextRoomSpawnPoints;
    
    [Header("~~~~~~~~~~~ Dont Touch ~~~~~~~~~~~")]
    public bool isActive;

    [Header("Objectives")] 
    [SerializeField] private GameObject[] allPrimaryObjectives;
    [SerializeField] private GameObject[] allSecondaryObjectives;
    public Objective primaryObjective;
    public Objective secondaryObjective;

    [Header("Public References")]
    public EnemySpawnPoint[] enemySpawnPoints;
    public WaveSpawner[] waveSpawners;
    public CaptureZone[] captureZones;

    [Header("Internal References")]
    public GameObject[] nextRooms;

    [SerializeField] private Door[] exitDoors;


    [Header("Events")] 
    public UnityEvent onStartRoom;
    public UnityEvent onCompleteRoom;


    private void Awake()
    {
        captureZones = GetComponentsInChildren<CaptureZone>(true);
        enemySpawnPoints = GetComponentsInChildren<EnemySpawnPoint>(true);
    }

    private void Start()
    {
        List<Door> allDoors = GetComponentsInChildren<Door>().ToList();
        allDoors.Remove(entryDoor);
        exitDoors = allDoors.ToArray();


        
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

        if (primaryObjective == null)
        {
            List<GameObject> possibleChoices = new List<GameObject>();
            for (int i = 0; i < allPrimaryObjectives.Length; i++)
            {
                if (possiblePrimaryObjectives.Contains(allPrimaryObjectives[i].GetComponent<Objective>().objectiveType))
                {
                    possibleChoices.Add(allPrimaryObjectives[i]);
                }
            }

            if (possibleChoices.Count > 0)
            {
                primaryObjective = Instantiate(possibleChoices[LevelGenerator.instance.seededRandom.Next(0, possibleChoices.Count)], transform).GetComponent<Objective>();
                primaryObjective.onComplete.AddListener(completeRoom);
            }
            else
            {
                Debug.LogWarning("Room Not Have Objective?");
            }
            
        }
        else
        {
            primaryObjective.onComplete.AddListener(completeRoom);
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
            
            Debug.Log("Finished Room: " + LevelGenerator.instance.roomIndex);

            Destroy(primaryObjective);
            
            for (int i = 0; i < waveSpawners.Length; i++)
            {
                waveSpawners[i].isComplete = true;
            }
        }
    }
}
