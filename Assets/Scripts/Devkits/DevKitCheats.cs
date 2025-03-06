using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DevKitCheats : MonoBehaviour
{
    [SerializeField] private LootPoolScriptable lootPool;

    [SerializeField] private TMP_Dropdown leftGunDropdown;
    [SerializeField] private TMP_Dropdown rightGunDropdown;
    [SerializeField] private TMP_Dropdown chassisDropdown;

    [SerializeField] private TMP_Dropdown addChipDropdownLeft;
    [SerializeField] private TMP_Dropdown addChipDropdownRight;

    [SerializeField] private TMP_Dropdown bodyChipsDropdown;
    
    public GameObject devkitCheatMenu;
    private int[] loadout;

    private void Start()
    {
        
        Canvas childedCanvas = GetComponentInChildren<Canvas>(true);
        if(childedCanvas!=null) devkitCheatMenu = childedCanvas.gameObject;

        leftGunDropdown.options = new List<TMP_Dropdown.OptionData>();
        rightGunDropdown.options = new List<TMP_Dropdown.OptionData>();
        chassisDropdown.options = new List<TMP_Dropdown.OptionData>();
        addChipDropdownLeft.options = new List<TMP_Dropdown.OptionData>();
        addChipDropdownRight.options = new List<TMP_Dropdown.OptionData>();
        bodyChipsDropdown.options = new List<TMP_Dropdown.OptionData>();
        
        leftGunDropdown.onValueChanged.AddListener(delegate{ AddWeapon(true); });
        rightGunDropdown.onValueChanged.AddListener(delegate{ AddWeapon(false); });
        
        addChipDropdownLeft.onValueChanged.AddListener(delegate{ AddChip(true);});
        addChipDropdownRight.onValueChanged.AddListener(delegate{ AddChip(false);});
        bodyChipsDropdown.onValueChanged.AddListener(delegate{AddBodyChip();});

        leftGunDropdown.options.Add(new TMP_Dropdown.OptionData( "Change Left Weapon"));
        rightGunDropdown.options.Add(new TMP_Dropdown.OptionData("Change Right Weapon"));
        chassisDropdown.options.Add(new TMP_Dropdown.OptionData("Change Chassis"));
        addChipDropdownLeft.options.Add(new TMP_Dropdown.OptionData("Add Chip to Left Slot"));
        addChipDropdownRight.options.Add(new TMP_Dropdown.OptionData("Add Chip to Right Slot"));
        bodyChipsDropdown.options.Add(new TMP_Dropdown.OptionData("Add Chip To Body"));

        if(LevelGenerator.instance != null) lootPool = LevelGenerator.instance.levelInfo.lootPool;
        
        for (int i = 0; i <  lootPool.Weapons.Length; i++)
        {
            leftGunDropdown.options.Add(new TMP_Dropdown.OptionData( lootPool.Weapons[i].itemName));
            rightGunDropdown.options.Add(new TMP_Dropdown.OptionData( lootPool.Weapons[i].itemName));
        }

        for (int i = 0; i <  lootPool.WeaponChips.Length; i++)
        {
            addChipDropdownLeft.options.Add(new TMP_Dropdown.OptionData(lootPool.WeaponChips[i].name));
            addChipDropdownRight.options.Add(new TMP_Dropdown.OptionData(lootPool.WeaponChips[i].name));
        }

        for (int i = 0; i < lootPool.BodyChips.Length; i++)
        {
            bodyChipsDropdown.options.Add(new TMP_Dropdown.OptionData(lootPool.BodyChips[i].name));
        }

        loadout = new int[3];
    }
    

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Slash))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                PlayerBody.PlayBody().StopParts(false,false);
                devkitCheatMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                PlayerBody.PlayBody().StopParts(true,true);
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
        PlayerBody.PlayBody().GetComponent<Health>().health = 0;
    }

    public void ToggleGodmode()
    {
        PlayerBody.PlayBody().GetComponent<Health>().canDie = false;
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
        int[] newLoadout = new int[3];

        newLoadout[0] = chassisDropdown.value;
        newLoadout[1] = leftGunDropdown.value;
        newLoadout[2] = rightGunDropdown.value;
        
        if(LevelGenerator.instance != null) lootPool = LevelGenerator.instance.levelInfo.lootPool;

        if (newLoadout != loadout)
        {
            PlayerBody body = FindObjectOfType<PlayerBody>();
            
            if(newLoadout[1] != loadout[1]) body.SetWeapon((WeaponPickup)lootPool.Weapons[newLoadout[1]], true);
            if(newLoadout[2] != loadout[2]) body.SetWeapon((WeaponPickup)lootPool.Weapons[newLoadout[2]], false);
            
            loadout = newLoadout;
        }
    }

    public void AddWeapon(bool applyToLeft)
    {
        if(LevelGenerator.instance != null) lootPool = LevelGenerator.instance.levelInfo.lootPool;
        
        if (applyToLeft)
        {
            if (leftGunDropdown.value != 0)
            {
                PlayerBody.PlayBody().SetWeapon((WeaponPickup) lootPool.Weapons[leftGunDropdown.value - 1], true);

                leftGunDropdown.value = 0;
            }        
        }
        else
        {
            if (rightGunDropdown.value != 0)
            {
                PlayerBody.PlayBody().SetWeapon((WeaponPickup) lootPool.Weapons[rightGunDropdown.value - 1], false);

                rightGunDropdown.value = 0;
            }
        }
    }

    public void AddChip(bool applyToLeft)
    {
        if(LevelGenerator.instance != null) lootPool = LevelGenerator.instance.levelInfo.lootPool;
        
        if (applyToLeft)
        {
            if (addChipDropdownLeft.value != 0)
            {
                PlayerBody.PlayBody().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip) lootPool.WeaponChips[addChipDropdownLeft.value -1], true);
                addChipDropdownLeft.value = 0;
            }

        }
        else
        {
            if (addChipDropdownRight.value != 0)
            {
                PlayerBody.PlayBody().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip) lootPool.WeaponChips[addChipDropdownRight.value - 1], false);
                addChipDropdownRight.value = 0;
            }
        }
    }

    public void AddBodyChip()
    {
        if (bodyChipsDropdown.value != 0)
        {
            if(LevelGenerator.instance != null) lootPool = LevelGenerator.instance.levelInfo.lootPool;
        
            PlayerBody.PlayBody().ApplyChip((BodyChip)lootPool.BodyChips[bodyChipsDropdown.value - 1]);

            addChipDropdownLeft.value = 0;
        }

    }
}
