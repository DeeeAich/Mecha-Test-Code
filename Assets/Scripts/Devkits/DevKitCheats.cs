using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevKitCheats : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKey(KeyCode.Slash))
        {
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                Health[] healths = FindObjectsOfType<Health>();
                for (int i = 0; i < healths.Length; i++)
                {
                    if (healths[i].entityType == EntityType.ENEMY)
                    {
                        healths[i].TriggerDeath();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                Health[] healths = FindObjectsOfType<Health>();
                for (int i = 0; i < healths.Length; i++)
                {
                    if (healths[i].entityType == EntityType.PLAYER)
                    {
                        healths[i].TriggerDeath();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Room[] rooms = FindObjectsOfType<Room>();
                for (int i = 0; i < rooms.Length; i++)
                {
                    if (rooms[i].isActive)
                    {
                        rooms[i].completeRoom();
                    }
                }
            }
        }
    }
}
