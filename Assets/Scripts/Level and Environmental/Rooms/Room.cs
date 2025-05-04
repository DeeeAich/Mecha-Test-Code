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
    weaponChip,
    mechChip,
    ordinance,
    chassis
}

public class Room : MonoBehaviour
{
    [Header("~~~~~~~~~~~ Required References ~~~~~~~~~~~")]
    [SerializeField] private Door entryDoor;
    [SerializeField] private GameObject[] possiblePrimaryObjectives;
    [SerializeField] private bool triggersMusic = true;

    [Header("~~~~~~~~~~~ Dont Touch ~~~~~~~~~~~")]
    public bool isActive;

    [Header("Objectives")] 

    public Objective primaryObjective;

    [Header("Public References")]
    public EnemySpawnPoint[] enemySpawnPoints;
    public WaveSpawner[] waveSpawners;
    public CaptureZone[] captureZones;
    public GameObject lootSpawnPoint;
    public Pickup spawnedLoot;
    [SerializeField] private GameObject playerAttentionGrabberPrefab;
    public GameObject currentAttentionGrabber;
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

    private void Awake() // leave objectives and waveSpawners turned off if already in room, this script will turn em on
    {
        captureZones = GetComponentsInChildren<CaptureZone>(true);
        enemySpawnPoints = GetComponentsInChildren<EnemySpawnPoint>(true);
        waveSpawners = GetComponentsInChildren<WaveSpawner>(true);
        primaryObjective = GetComponentInChildren<Objective>(true);
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
    
    public void StartRoom()
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
        
        if (waveSpawners != null || waveSpawners.Length > 0)
        {
            for (int i = 0; i < waveSpawners.Length; i++)
            {
                waveSpawners[i].gameObject.SetActive(true);
            }
        }

        if (primaryObjective == null)
        {
            if (possiblePrimaryObjectives.Length > 0)
            {
                primaryObjective = Instantiate(possiblePrimaryObjectives[LevelGenerator.instance.seededRandom.Next(0, possiblePrimaryObjectives.Length)], transform).GetComponent<Objective>();
                primaryObjective.onComplete.AddListener(CompleteRoom);
            }
            else
            {
                Debug.LogWarning("Room Not Have Objective?");
            }
            
        }
        else
        {
            primaryObjective.gameObject.SetActive(true);
            primaryObjective.onComplete.AddListener(CompleteRoom);
        }
        
        if(LevelGenerator.instance.oldRoom != null && LevelGenerator.instance.oldRoom.GetComponent<Room>().currentAttentionGrabber != null) Destroy(LevelGenerator.instance.oldRoom.GetComponent<Room>().currentAttentionGrabber);

        CamRoom3D camRoom3D = GetComponentInChildren<CamRoom3D>();
        if (camRoom3D != null)
        {
            FindObjectOfType<CinemachineVirtualCamera>().Follow = camRoom3D.tracker.transform;
        }

        isActive = true;
        onStartRoom.Invoke();
    }

    public void CompleteRoom()
    {
        // goodbye tom
        // goodbye jacob
        
        if (isActive)
        {
            //cleaning up internal stuff
            
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
                    if (waveSpawners[i] != null)
                    {
                        waveSpawners[i].isComplete = true;
                        Destroy(waveSpawners[i].gameObject);
                    }
                }
            }
            
            foreach (var pickup in FindObjectsOfType<Pickup>())
            {
                pickup.SpawnLootBox();
                
                
                /*
                for (int i = 0; i < pickup.animators.Length; i++)
                {
                    pickup.animators[i].SetTrigger("spawnLoot");
                }
                pickup.GetComponentInChildren<Interactable>(true).gameObject.SetActive(true);
                */
            }

         
            if (lootSpawnPoint != null)
            {
                if(currentAttentionGrabber != null) Destroy(currentAttentionGrabber);
                currentAttentionGrabber = Instantiate(playerAttentionGrabberPrefab, lootSpawnPoint.transform);
                currentAttentionGrabber.transform.localScale = new Vector3(25, 15, 10);
            }

            //setting external stuff
            if (GameGeneralManager.instance != null) GameGeneralManager.instance.difficulty += 0.5f / LevelGenerator.instance.roomsInThisFloor;
            
            //if(MetricsTracker.instance != null) MetricsTracker.instance.RoomCompleted(this);
            
            if(triggersMusic) AudioManager.instance.ChangeMusicState(musicState.idle);

            FindObjectOfType<CinemachineVirtualCamera>().Follow = PlayerBody.Instance().transform;
            
            onCompleteRoom.Invoke();
        }
    }

    private void SpawnLoot()
    {
        Debug.Log("Spawning Loot");
        
        PlayerPickup[] pickupsToSpawn = LevelGenerator.instance.GenerateLootPickups(lootCount, roomLootType);

        spawnedLoot = Instantiate(LevelGenerator.instance.levelInfo.lootPool.pickupPrefab, lootSpawnPoint.transform.position, lootSpawnPoint.transform.rotation).GetComponent<Pickup>();
        spawnedLoot.onPickedUpEvent.AddListener(delegate { Destroy(this.currentAttentionGrabber); });
        spawnedLoot.transform.SetParent(transform);
        spawnedLoot.PlayerPickups = pickupsToSpawn;
    }
}
