using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;



public enum LootType
{
    weapon,
    combatChip,
    ordinance,
    chassis
}

public class Room : MonoBehaviour
{
    [Header("~~~~~~~~~~~ Required References ~~~~~~~~~~~")]
    [SerializeField] private Door entryDoor;
    [SerializeField] private ObjectiveType[] possiblePrimaryObjectives;
    [SerializeField] private ObjectiveType[] possibleSecondaryObjectives;


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
    public GameObject lootSpawnPoint;
    public int lootCount = 3;

    [Header("Internal References")]
    public GameObject[] nextRooms;
    public LootType[] nextRoomLootTypes;
    public LootType roomLootType;

    [SerializeField] private Door[] exitDoors;
    private float lootSeperationDistance = 7.5f;

    [Header("Events")] 
    public UnityEvent onStartRoom;
    public UnityEvent onCompleteRoom;

    private void Awake()
    {
        captureZones = GetComponentsInChildren<CaptureZone>(true);
        enemySpawnPoints = GetComponentsInChildren<EnemySpawnPoint>(true);
        waveSpawners = GetComponentsInChildren<WaveSpawner>(true);
    }

    private void Start()
    {
        if (exitDoors.Length == 0)
        {
            List<Door> allDoors = GetComponentsInChildren<Door>(true).ToList();
            if(entryDoor != null) allDoors.Remove(entryDoor);
            exitDoors = allDoors.ToArray();
        }

        if (exitDoors.Length > 0)
        {
            List<Door> possibleExitDoors = exitDoors.ToList();

            for (int i = 0; i < possibleExitDoors.Count; i++)
            {
                if (possibleExitDoors[i].transform.rotation.eulerAngles.y < 90 || possibleExitDoors[i].transform.rotation.eulerAngles.y > 360 -90)
                {
                    possibleExitDoors[i].CloseDoor();
                    possibleExitDoors[i].LockDoor();
                    possibleExitDoors.RemoveAt(i);

                }
            }

            exitDoors = possibleExitDoors.ToArray();
        }

        if (exitDoors.Length > 0)
        {
            nextRooms = LevelGenerator.instance.NextRoomSelection(exitDoors.Length);
            nextRoomLootTypes = LevelGenerator.instance.GenerateNextLootType(exitDoors.Length);

            for (int i = 0; i < exitDoors.Length; i++)
            {
                exitDoors[i].SetDoorLootType(nextRoomLootTypes[i]);
                
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
                exitDoors[i].onOpen.AddListener(delegate
                {
                    LevelGenerator.instance.SpawnRoom(nextRooms[exitIndex], exitDoors[exitIndex].nextRoomSpawnPoint);
                    LevelGenerator.instance.currentRoom.GetComponent<Room>().roomLootType = nextRoomLootTypes[exitIndex];
                });
            }
        }
        
        if (lootSpawnPoint != null)
        {
            SpawnLoot();
        }
    }
    

    public void startRoom()
    {
        Debug.Log("Starting Room: " + name);
        
        if(MetricsTracker.instance != null) MetricsTracker.instance.RoomStarted();
        // hello tom
        // hi jacob :3
        AudioManager.instance.ChangeMusicState(musicState.combat);

        if (entryDoor != null)
        {
            entryDoor.CloseDoor();
            entryDoor.LockDoor();
        }

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

        CamRoom3D camRoom3D = GetComponentInChildren<CamRoom3D>();
        if (camRoom3D != null)
        {
            FindObjectOfType<CinemachineVirtualCamera>().Follow = camRoom3D.tracker.transform;
        }

        isActive = true;
        onStartRoom.Invoke();
    }

    public void completeRoom()
    {
        if (isActive)
        {
            // goodbye tom
            // goodbye jacob
            
            if(MetricsTracker.instance != null) MetricsTracker.instance.RoomCompleted(this);
            
            AudioManager.instance.ChangeMusicState(musicState.idle);

            onCompleteRoom.Invoke();

            for (int i = 0; i < exitDoors.Length; i++)
            {
                exitDoors[i].UnlockDoor();
            }

            if(primaryObjective != null) Destroy(primaryObjective.gameObject);
            Debug.Log("Finished Room: " + LevelGenerator.instance.roomIndex);

            if (waveSpawners.Length > 0)
            {
                for (int i = 0; i < waveSpawners.Length; i++)
                {
                    waveSpawners[i].isComplete = true;
                    Destroy(waveSpawners[i].gameObject);
                }
            }
 

            foreach (var pickup in FindObjectsOfType<Pickup>())
            {
                pickup.animator.SetTrigger("spawnLoot");
                pickup.GetComponentInChildren<Interactable>(true).gameObject.SetActive(true);
            }

            FindObjectOfType<CinemachineVirtualCamera>().Follow = PlayerBody.PlayBody().transform;
        }
    }

    private void SpawnLoot()
    {
        Debug.Log("Spawning Loot");
        
        PlayerPickup[] pickupsToSpawn = LevelGenerator.instance.GenerateLootPickups(lootCount, roomLootType);
        
        for (int i = 0; i < pickupsToSpawn.Length; i++)
        {
            Pickup newLoot = Instantiate(LevelGenerator.instance.levelInfo.lootPool.pickupPrefab,
                lootSpawnPoint.transform.position + lootSpawnPoint.transform.right * lootSeperationDistance * (i - Mathf.FloorToInt(lootCount/2)), 
                lootSpawnPoint.transform.rotation).GetComponent<Pickup>();
            
            newLoot.transform.SetParent(transform);
            newLoot.PlayerPickup = pickupsToSpawn[i];
        }
    }
}
