using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class LevelGenerator : MonoBehaviour
{
    [Header("Generation Numbers")]
    public int floor;
    public int roomIndex;
    public int roomsInThisFloor;
    public int[] minibossRoomIndexes;
    
    [Header("Public References")]
    public GameObject currentRoom;
    public GameObject oldRoom;
    private GameObject mostrecentyUsedRoomPrefab;
    
    [Header("Internal References")]
    [SerializeField] private GameObject StartPosition;
    [SerializeField] private int randomSeed;
    [SerializeField] private bool randomizeSeedOnAwake;
    public LevelScriptable levelInfo;

    public Random seededRandom;
    public static LevelGenerator instance;

    public UnityEvent onSpawnRoom;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if(instance != this) Destroy(gameObject);
        }
        
        if (randomizeSeedOnAwake)
        {
            randomSeed = (int)(System.DateTime.Now.Ticks);
            Debug.Log("Starting game with seed: " + randomSeed);
        }
        
        seededRandom = new Random(randomSeed);
    }

    private void Start()
    {
        SpawnRoom(levelInfo.roomPool.entryRooms[seededRandom.Next(0, levelInfo.roomPool.entryRooms.Length)], StartPosition);
        currentRoom.GetComponent<Room>().roomLootType = GenerateNextLootType(1)[0];
        roomIndex = 0;
        if(PlayerManager.instance != null)
            PlayerManager.instance.SetStats();
    }

    public GameObject[] NextRoomSelection(int count)
    {
        GameObject[] selection = new GameObject[count];
        List<GameObject> possibleRooms = new List<GameObject>();

        if (roomIndex == roomsInThisFloor - 1)
        {
            possibleRooms = levelInfo.roomPool.bossRooms.ToList();
        }
        else if (roomIndex == roomsInThisFloor)
        {
            possibleRooms = levelInfo.roomPool.finalRooms.ToList();
        }
        else if (minibossRoomIndexes.Contains(roomIndex))
        {
            possibleRooms = levelInfo.roomPool.miniBossRooms.ToList();
        }
        else
        {
            possibleRooms = levelInfo.roomPool.standardRooms.ToList();
        }

        possibleRooms.Remove(mostrecentyUsedRoomPrefab);
        
        int totalPossibleRooms = possibleRooms.Count;
        for (int i = 0; i < selection.Length; i++)
        {
            int rand = seededRandom.Next(0, possibleRooms.Count);
            selection[i] = possibleRooms[rand];
            if(totalPossibleRooms > selection.Length) possibleRooms.RemoveAt(rand);
        }

        return selection;
    }

    public LootType[] GenerateNextLootType(int count)
    {
        LootType[] selection = new LootType[count];
        List<LootType> possibleLoots = new List<LootType>();
        
        possibleLoots.Add(LootType.weapon);
        possibleLoots.Add(LootType.combatChip);
        possibleLoots.Add(LootType.combatChip);

        //possibleLoots.Add(LootType.ordinance);
        //possibleLoots.Add(LootType.chassis);

        int totalPossibleLoots = possibleLoots.Count;
        for (int i = 0; i < selection.Length; i++)
        {
            int rand = seededRandom.Next(0, possibleLoots.Count);
            selection[i] = possibleLoots[rand];
            if(totalPossibleLoots > selection.Length) possibleLoots.RemoveAt(rand);
        }
        
        return selection;
    }

    public PlayerPickup[] GenerateLootPickups(int count, LootType type)
    {
        PlayerPickup[] selection = new PlayerPickup[count];
        List<PlayerPickup> possibleSelection = new List<PlayerPickup>();
        
        switch (type)
        {
            case LootType.weapon:
                possibleSelection.AddRange(levelInfo.lootPool.Weapons);
                break;
            
            case LootType.combatChip:
                possibleSelection.AddRange(levelInfo.lootPool.WeaponChips);
                break;
            
            case LootType.ordinance:
                possibleSelection.AddRange(levelInfo.lootPool.Ordinance);
                break;
            
            case LootType.chassis:
                possibleSelection.AddRange(levelInfo.lootPool.Chassis); 
                break;
        }

    
        int totalSpawnChance = 0;

        for (int i = 0; i < possibleSelection.Count; i++)
        {
            totalSpawnChance += possibleSelection[i].spawnRate;
        }
        
        int totalPossiblePickups = possibleSelection.Count;
        
        for (int i = 0; i < selection.Length; i++)
        {
            int rand = seededRandom.Next(0, totalSpawnChance);
            float currentSpawnChance = 0;

            for (int j = 0; j < possibleSelection.Count; j++)
            {
                currentSpawnChance += possibleSelection[j].spawnRate;
                if (currentSpawnChance > rand)
                {
                    selection[i] = possibleSelection[j];
                    if (totalPossiblePickups > selection.Length)
                    {
                        totalSpawnChance -= possibleSelection[j].spawnRate;
                        possibleSelection.RemoveAt(j);
                    }
                    break;
                }
            }
        }

        return selection;
    }
    
    
    public void SpawnRoom(GameObject room, GameObject targetPosition)
    {
        mostrecentyUsedRoomPrefab = room;
        if(oldRoom != null) Destroy(oldRoom);
        if(currentRoom != null) oldRoom = currentRoom;
        currentRoom = Instantiate(room, targetPosition.transform.position, targetPosition.transform.rotation);
        onSpawnRoom.Invoke();
        roomIndex++;
    }
}
