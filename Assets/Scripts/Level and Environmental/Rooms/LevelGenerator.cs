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
        instance = this;

        if (GameGeneralManager.instance == null)
        {
            GameObject GM = new GameObject("Spawned Game General Manager");
            GM.AddComponent<GameGeneralManager>();
            GameGeneralManager.instance.difficulty = 1;
            GameGeneralManager.instance.CreateNewSeed();
        }

        if (randomizeSeedOnAwake)
        {
            if (GameGeneralManager.instance != null && GameGeneralManager.instance.seededRandom != null)
            {
                randomSeed = GameGeneralManager.instance.seededRandom.Next();
            }
            else
            {
                randomSeed = (int)(System.DateTime.Now.Ticks);
            }
      
            Debug.Log("Starting game with seed: " + randomSeed);
        }
        
        seededRandom = new Random(randomSeed);
    }

    private void Start()
    {
        SpawnRoom(levelInfo.roomPool.entryRooms[0], StartPosition);
        currentRoom.GetComponent<Room>().roomLootType = GenerateNextLootType(1)[0];
        if(PlayerManager.instance != null) PlayerManager.instance.SetStats();

        //ResetChips();
    }

    public GameObject[] NextRoomSelection(int count)
    {
        GameObject[] selection = new GameObject[count];
        List<GameObject> possibleRooms = new List<GameObject>();

        if (roomIndex  < levelInfo.roomPool.entryRooms.Length)
        {
            print("Using entry rooms");
            possibleRooms.Add(levelInfo.roomPool.entryRooms[roomIndex]);
        }
        else if (roomIndex == roomsInThisFloor)
        {
            possibleRooms = levelInfo.roomPool.bossRooms.ToList();
        }
        else if (roomIndex == roomsInThisFloor + 1)
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
        
        Debug.Log("There are " + possibleRooms.Count + " rooms available");

        
        if (possibleRooms.Count > 0 && possibleRooms.Contains(mostrecentyUsedRoomPrefab))
        {
            print("Removing " + mostrecentyUsedRoomPrefab.name);
            possibleRooms.Remove(mostrecentyUsedRoomPrefab);
        }
        
        
        if(possibleRooms.Count == 0) Debug.LogError("FUCK AHH THERE ARE NO FUCKING ROOMS TO GENERATE");
        
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
        possibleLoots.Add(LootType.weaponChip);
        possibleLoots.Add(LootType.mechChip);

        //possibleLoots.Add(LootType.ordinance);
        //possibleLoots.Add(LootType.chassis);

        int totalPossibleLoots = possibleLoots.Count;
        for (int i = 0; i < selection.Length; i++)
        {
            int rand = seededRandom.Next(0, possibleLoots.Count);
            selection[i] = possibleLoots[rand];
            if(totalPossibleLoots > selection.Length) possibleLoots.RemoveAt(rand);
        }

        if (roomIndex == 1)
        {
            for (int i = 0; i < selection.Length; i++)
            {
                selection[i] = LootType.weapon;
            }
        }
        
        return selection;
    }

    public PlayerPickup[] GenerateLootPickups(int count, LootType type)
    {
        PlayerPickup[] selection = new PlayerPickup[count];
        List<PlayerPickup> possibleSelection = new List<PlayerPickup>();

        for (int i = 0; i < levelInfo.lootPools.Length; i++)
        {
            switch (type)
            {
                case LootType.weapon:
                    possibleSelection.AddRange(levelInfo.lootPools[i].Weapons);
                    break;
            
                case LootType.weaponChip:
                    possibleSelection.AddRange(levelInfo.lootPools[i].WeaponChips);
                    break;
            
                case LootType.mechChip:
                    possibleSelection.AddRange(levelInfo.lootPools[i].BodyChips);
                    break;
            
                case LootType.ordinance:
                    possibleSelection.AddRange(levelInfo.lootPools[i].Ordinance);
                    break;
            
                case LootType.chassis:
                    possibleSelection.AddRange(levelInfo.lootPools[i].Chassis); 
                    break;
            }
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
        roomIndex++;
        mostrecentyUsedRoomPrefab = room;
        if(oldRoom != null) Destroy(oldRoom);
        if(currentRoom != null) oldRoom = currentRoom;
        currentRoom = Instantiate(room, targetPosition.transform.position, targetPosition.transform.rotation);
        onSpawnRoom.Invoke();
    }

    public void ResetChips()
    {
        
        foreach (LootPoolScriptable lootPool in levelInfo.lootPools)
        {

            foreach(PlayerPickup chip in lootPool.BodyChips)
            {
                if(chip.PickupType == pickupType.ChassisChip)
                {
                    BodyChip bodyChip = (BodyChip)chip;
                    if (bodyChip.bodyType == BodyChip.BodyType.Trigger && !PlayerBody.Instance().myMods.Contains(bodyChip))
                    {
                        BodyTriggerChip bodyTriggerChip = (BodyTriggerChip)chip;
                        bodyTriggerChip.ChipTriggerUnsetter();
                    }

                }
                else
                {
                    MovementChip movementChip = (MovementChip)chip;
                    if (movementChip.moveType == MovementChip.MovementType.Trigger && !PlayerBody.Instance().myMovement.legChips.Contains(movementChip))
                    {
                        MovementTriggerChip movementTriggerChip = (MovementTriggerChip)chip;
                        movementTriggerChip.ChipTriggerUnsetter();
                    }
                }
            }

            foreach(PlayerPickup chip in lootPool.WeaponChips)
            {
                WeaponChip weaponChip = (WeaponChip)chip;
                if(weaponChip.supType == WeaponChip.WeaponSubType.Trigger &&
                    !PlayerWeaponControl.instance.leftMods.Contains(weaponChip) && !PlayerWeaponControl.instance.leftMods.Contains(weaponChip))
                {
                    WeaponTriggerChip weaponTriggerChip = (WeaponTriggerChip)weaponChip;
                    weaponTriggerChip.ChipTriggerUnsetter(PlayerWeaponControl.instance.leftWeapon);
                    weaponTriggerChip.ChipTriggerUnsetter(PlayerWeaponControl.instance.rightWeapon);
                }
            }


        }

    }

}
