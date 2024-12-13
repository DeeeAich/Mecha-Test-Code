using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class LevelGenerator : MonoBehaviour
{
    public int floor;
    public int roomIndex;
    public int roomsInThisFloor;
    public int[] minibossRoomIndexes;
    
    public GameObject currentRoom;
    public GameObject oldRoom;
    
    [SerializeField] private GameObject StartPosition;
    [SerializeField] private int randomSeed;
    [SerializeField] private bool randomizeSeedOnAwake;
    public LevelScriptable levelInfo;

    public Random seededRandom;
    public static LevelGenerator instance;

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
        currentRoom.GetComponent<Room>().roomLoot = NextLootSelection(1)[0];
        roomIndex = 0;
    }

    public GameObject[] NextRoomSelection(int count)
    {
        GameObject[] selection = new GameObject[count];
        List<GameObject> possibleRooms = new List<GameObject>();

        if (roomIndex == roomsInThisFloor - 1)
        {
            possibleRooms = levelInfo.roomPool.bossRooms.ToList();
        }
        else if (minibossRoomIndexes.Contains(roomIndex))
        {
            possibleRooms = levelInfo.roomPool.miniBossRooms.ToList();
        }
        else
        {
            possibleRooms = levelInfo.roomPool.standardRooms.ToList();
        }

        int totalPossibleRooms = possibleRooms.Count;
        for (int i = 0; i < selection.Length; i++)
        {
            int rand = seededRandom.Next(0, possibleRooms.Count);
            selection[i] = possibleRooms[rand];
            if(totalPossibleRooms > selection.Length) possibleRooms.RemoveAt(rand);
        }

        return selection;
    }

    public LootType[] NextLootSelection(int count)
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

    public GameObject[] GenerateLootPickups(int count, LootType type)
    {
        GameObject[] selection = new GameObject[count];
        List<GameObject> possibleSelection = new List<GameObject>();
        
        switch (type)
        {
            case LootType.weapon:
                possibleSelection.AddRange(levelInfo.lootPool.standardWeapons);
                break;
            
            case LootType.combatChip:
                possibleSelection.AddRange(levelInfo.lootPool.standardChips);
                break;
            
            case LootType.ordinance:
                possibleSelection.AddRange(levelInfo.lootPool.standardOrdinance);
                break;
            
            case LootType.chassis:
                possibleSelection.AddRange(levelInfo.lootPool.standardChassis); 
                break;
        }

        int totalPossiblePickups = possibleSelection.Count;
        
        for (int i = 0; i < selection.Length; i++)
        {
            int rand = seededRandom.Next(0, possibleSelection.Count);
            selection[i] = possibleSelection[rand];
            if(totalPossiblePickups > selection.Length) possibleSelection.RemoveAt(rand);
        }

        return selection;
    }

    public void SpawnRoom(GameObject room, GameObject targetPosition)
    {
        if(oldRoom != null) Destroy(oldRoom);
        if(currentRoom != null) oldRoom = currentRoom;
        currentRoom = Instantiate(room, targetPosition.transform.position, targetPosition.transform.rotation);
        roomIndex++;
    }
}
