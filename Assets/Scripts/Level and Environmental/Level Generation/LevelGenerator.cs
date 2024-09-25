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

    [SerializeField] private GameObject pastRoom;
    
    [SerializeField] private GameObject StartPosition;
    [SerializeField] private int randomSeed;
    [SerializeField] private bool randomizeSeedOnAwake;
    public RoomPoolScriptable roomPool;
    
    private Random seededRandom;
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
            randomSeed = Mathf.CeilToInt(System.DateTime.Now.Ticks);
            seededRandom = new Random(randomSeed);
        }
    }

    private void Start()
    {
        SpawnRoom(roomPool.standardRooms[seededRandom.Next(0, roomPool.standardRooms.Length)], StartPosition);
        roomIndex = 0;
    }

    public GameObject[] NextRoomSelection(int count)
    {
        GameObject[] selection = new GameObject[count];
        List<GameObject> possibleRooms = new List<GameObject>();

        if (roomIndex == roomsInThisFloor - 1)
        {
            possibleRooms = roomPool.bossRooms.ToList();
        }
        else if (minibossRoomIndexes.Contains(roomIndex))
        {
            possibleRooms = roomPool.miniBossRooms.ToList();
        }
        else
        {
            possibleRooms = roomPool.standardRooms.ToList();
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

    public void SpawnRoom(GameObject room, GameObject targetPosition)
    {
        GameObject newRoom = Instantiate(room, targetPosition.transform.position, targetPosition.transform.rotation);
        newRoom.GetComponent<Room>().onStartRoom.AddListener(delegate
        {
            if (pastRoom != null)
            {
                Destroy(pastRoom);
            }

            pastRoom = newRoom;
        });
        roomIndex++;
    }
}
