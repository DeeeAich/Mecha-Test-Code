using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevKitCheats : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown[] loadoutDropdowns;
    public GameObject devkitCheatMenu;
    private int[] loadout;

    private void Start()
    {
        loadout = new int[loadoutDropdowns.Length]; 
        devkitCheatMenu = GetComponentInChildren<Canvas>(true).gameObject;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Slash))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                devkitCheatMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                devkitCheatMenu.SetActive(false);
            }
        }
    }

    public void KillAllEnemies()
    {
        Health[] healths = FindObjectsOfType<Health>();
        for (int i = 0; i < healths.Length; i++)
        {
            if (healths[i].entityType == EntityType.ENEMY)
            {
                healths[i].health = 0;
                healths[i].TriggerDeath();
            }
        }
    }

    public void KillAllPlayers()
    {
        Health[] healths = FindObjectsOfType<Health>();
        for (int i = 0; i < healths.Length; i++)
        {
            if (healths[i].entityType == EntityType.PLAYER)
            {
                healths[i].health = 0;
                healths[i].TriggerDeath();
            }
        }
    }

    public void CompleteRoom()
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

    public void SetLoadout()
    {
        int[] newLoadout = new int[loadoutDropdowns.Length];
        for (int i = 0; i < newLoadout.Length; i++)
        {
            newLoadout[i] = loadoutDropdowns[i].value;
        }

        if (newLoadout == loadout)
        {
            loadout = newLoadout;
        }
        
        
    }


}
