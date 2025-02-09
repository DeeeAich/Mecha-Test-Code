using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DevKitCheats : MonoBehaviour
{
    [SerializeField] private ComponentMasterListScriptable componentMasterListScriptable;
    
    [SerializeField] private TMP_Dropdown leftGunDropdown;
    [SerializeField] private TMP_Dropdown rightGunDropdown;
    [SerializeField] private TMP_Dropdown chassisDropdown;

    [SerializeField] private TMP_Dropdown addChipDropdownLeft;
    [SerializeField] private TMP_Dropdown addChipDropdownRight;
    
    public GameObject devkitCheatMenu;
    private int[] loadout;

    private void Start()
    {
        
        Canvas childedCanvas = GetComponentInChildren<Canvas>(true);
        if(childedCanvas!=null) devkitCheatMenu = childedCanvas.gameObject;
        DontDestroyOnLoad(gameObject);

        leftGunDropdown.options = new List<TMP_Dropdown.OptionData>();
        rightGunDropdown.options = new List<TMP_Dropdown.OptionData>();
        chassisDropdown.options = new List<TMP_Dropdown.OptionData>();
        addChipDropdownLeft.options = new List<TMP_Dropdown.OptionData>();
        addChipDropdownRight.options = new List<TMP_Dropdown.OptionData>();
        
        for (int i = 0; i < componentMasterListScriptable.weapons.Length; i++)
        {
            leftGunDropdown.options.Add(new TMP_Dropdown.OptionData(componentMasterListScriptable.weapons[i].name));
            rightGunDropdown.options.Add(new TMP_Dropdown.OptionData(componentMasterListScriptable.weapons[i].name));
        }

        for (int i = 0; i < componentMasterListScriptable.chips.Length; i++)
        {
            addChipDropdownLeft.options.Add(new TMP_Dropdown.OptionData(componentMasterListScriptable.chips[i].name));
            addChipDropdownRight.options.Add(new TMP_Dropdown.OptionData(componentMasterListScriptable.chips[i].name));
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
        int[] newLoadout = new int[3];

        newLoadout[0] = chassisDropdown.value;
        newLoadout[1] = leftGunDropdown.value;
        newLoadout[2] = rightGunDropdown.value;

        if (newLoadout != loadout)
        {
            PlayerBody body = FindObjectOfType<PlayerBody>();
            
            if(newLoadout[1] != loadout[1]) body.SetWeapon(componentMasterListScriptable.weapons[newLoadout[1]], true);
            if(newLoadout[2] != loadout[2]) body.SetWeapon(componentMasterListScriptable.weapons[newLoadout[2]], false);
            
            loadout = newLoadout;
        }
    }

    public void AddChip(bool applyToLeft)
    {
        if (applyToLeft)
        {
            PlayerBody.PlayBody().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip)componentMasterListScriptable.chips[addChipDropdownLeft.value], applyToLeft);
        }
        else
        {
            PlayerBody.PlayBody().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip)componentMasterListScriptable.chips[addChipDropdownRight.value], applyToLeft);
        }

    }
}
