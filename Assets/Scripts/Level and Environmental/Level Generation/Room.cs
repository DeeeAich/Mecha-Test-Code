using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool isActive;
    
    public Door entryDoor;
    public Door[] exitDoors;

    private void Start()
    {
        for (int i = 0; i < exitDoors.Length; i++)
        {
            for (int j = 0; j < exitDoors.Length; j++)
            {
                if (j != i)
                {
                    int index = j;
                    exitDoors[i].onOpen.AddListener(delegate { exitDoors[index].LockDoor(); });
                }
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
    }

    public void completeRoom()
    {
        for (int i = 0; i < exitDoors.Length; i++)
        {
            exitDoors[i].UnlockDoor();
        }
    }
}
