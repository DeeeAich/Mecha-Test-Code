using System;
using System.Collections;
using System.Collections.Generic;
using AITree;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DevKitCheats : MonoBehaviour
{
    [SerializeField] private bool usesDevkitButtons = true;

    [SerializeField] private Animator screenBorderAnimator;
    
    [Header("Loot addition stuff")]
    [SerializeField] private LootPoolScriptable[] lootPools;

    [SerializeField] private TMP_Dropdown leftGunDropdown;
    [SerializeField] private TMP_Dropdown rightGunDropdown;
    [SerializeField] private TMP_Dropdown chassisDropdown;

    [SerializeField] private TMP_Dropdown addChipDropdownLeft;
    [SerializeField] private TMP_Dropdown addChipDropdownRight;

    [SerializeField] private TMP_Dropdown bodyChipsDropdown;
    
    private bool timeIsGoing = true;

    private void Start()
    {
        leftGunDropdown.onValueChanged.AddListener(delegate{ AddWeapon(true); });
        rightGunDropdown.onValueChanged.AddListener(delegate{ AddWeapon(false); });
        
        addChipDropdownLeft.onValueChanged.AddListener(delegate{ AddChip(true);});
        addChipDropdownRight.onValueChanged.AddListener(delegate{ AddChip(false);});
        bodyChipsDropdown.onValueChanged.AddListener(delegate{AddBodyChip();});
        
        UpdateDropdowns();
    }

    public void UpdateDropdowns()
    {
        lootPools = LevelGenerator.instance.levelInfo.lootPools;
        
        leftGunDropdown.options = new List<TMP_Dropdown.OptionData>();
        rightGunDropdown.options = new List<TMP_Dropdown.OptionData>();
        chassisDropdown.options = new List<TMP_Dropdown.OptionData>();
        addChipDropdownLeft.options = new List<TMP_Dropdown.OptionData>();
        addChipDropdownRight.options = new List<TMP_Dropdown.OptionData>();
        bodyChipsDropdown.options = new List<TMP_Dropdown.OptionData>();
        
        leftGunDropdown.options.Add(new TMP_Dropdown.OptionData( "Change Left Weapon"));
        rightGunDropdown.options.Add(new TMP_Dropdown.OptionData("Change Right Weapon"));
        chassisDropdown.options.Add(new TMP_Dropdown.OptionData("Change Chassis"));
        addChipDropdownLeft.options.Add(new TMP_Dropdown.OptionData("Add Chip to Left Slot"));
        addChipDropdownRight.options.Add(new TMP_Dropdown.OptionData("Add Chip to Right Slot"));
        bodyChipsDropdown.options.Add(new TMP_Dropdown.OptionData("Add Chip To Body"));

        if (lootPools.Length > 0)
        {
            for (int i = 0; i < lootPools.Length; i++)
            {
                if (lootPools[i].Weapons.Length > 0)
                {
                    for (int j = 0; j < lootPools[i].Weapons.Length; j++)
                    {
                        leftGunDropdown.options.Add(new TMP_Dropdown.OptionData(
                            lootPools[i].Weapons[j].itemName, lootPools[i].Weapons[j].mySprite));
                        rightGunDropdown.options.Add(new TMP_Dropdown.OptionData(
                            lootPools[i].Weapons[j].itemName, lootPools[i].Weapons[j].mySprite));
                    }
                }

                if (lootPools[i].BodyChips.Length > 0)
                {
                    for (int j = 0; j < lootPools[i].BodyChips.Length; j++)
                    {
                        bodyChipsDropdown.options.Add(new TMP_Dropdown.OptionData(
                            lootPools[i].BodyChips[j].name, lootPools[i].BodyChips[j].mySprite));
                    }
                }

                if (lootPools[i].WeaponChips.Length > 0)
                {
                    for (int j = 0; j < lootPools[i].WeaponChips.Length; j++)
                    {
                        addChipDropdownLeft.options.Add(new TMP_Dropdown.OptionData(
                            lootPools[i].WeaponChips[j].name,
                            lootPools[i].WeaponChips[j].mySprite));
                        addChipDropdownRight.options.Add(new TMP_Dropdown.OptionData(
                            lootPools[i].WeaponChips[j].name,
                            lootPools[i].WeaponChips[j].mySprite));
                    }
                }

            }
        }
        
    }

    private void Update()
    {
        if (usesDevkitButtons && Input.GetKey(KeyCode.RightBracket))
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                ToggleEnemyTime();
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket) || Input.GetKeyDown(KeyCode.P))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            }
        }
    }

    public void KillAllEnemies()
    {
        Health[] healths = FindObjectsOfType<Health>();
        Health playerHealth = PlayerBody.Instance().GetComponent<Health>();
        
        for (int i = 0; i < healths.Length; i++)
        {
            if (healths[i] != playerHealth)
            {
                healths[i].health = 0;
                healths[i].TriggerDeath();
            }
        }
    }

    public void KillAllPlayers()
    {
        PlayerBody.Instance().GetComponent<Health>().health = 0;
    }

    public void ToggleGodmode()
    {
        PlayerBody.Instance().GetComponent<Health>().canDie = false;
    }

    public void CompleteRoom()
    {
        if (LevelGenerator.instance != null && LevelGenerator.instance.currentRoom.GetComponent<Room>().isActive)
        {
            LevelGenerator.instance.currentRoom.GetComponent<Room>().CompleteRoom();
        }
    }

    public void AddWeapon(bool applyToLeft)
    {
        if(LevelGenerator.instance != null) lootPools = LevelGenerator.instance.levelInfo.lootPools;
        
        if (applyToLeft)
        {
            if (leftGunDropdown.value != 0)
            {
                int itemsToSkip = 0;
                
                for (int i = 0; i < lootPools.Length; i++)
                {
                    if (leftGunDropdown.value - 1 - itemsToSkip < lootPools[i].Weapons.Length)
                    {
                        PlayerBody.Instance().SetWeapon((WeaponPickup) lootPools[i].Weapons[leftGunDropdown.value - 1 - itemsToSkip], true);
                        Debug.Log("Added weapon" +  lootPools[i].Weapons[leftGunDropdown.value - 1 - itemsToSkip]);
                    }
                    else
                    {
                        itemsToSkip += lootPools[i].Weapons.Length;
                    }
                }

                leftGunDropdown.value = 0;
            }        
        }
        else
        {
            if (rightGunDropdown.value != 0)
            {
                int itemsToSkip = 0;
                
                for (int i = 0; i < lootPools.Length; i++)
                {
                    if (rightGunDropdown.value - 1 - itemsToSkip < lootPools[i].Weapons.Length)
                    {
                        PlayerBody.Instance().SetWeapon((WeaponPickup) lootPools[i].Weapons[rightGunDropdown.value - 1 - itemsToSkip], false);
                        Debug.Log("Added weapon" +  lootPools[i].Weapons[rightGunDropdown.value - 1 - itemsToSkip].itemName);
                    }
                    else
                    {
                        itemsToSkip += lootPools[i].Weapons.Length;
                    }
                }

                rightGunDropdown.value = 0;
            }
        }
    }

    public void AddChip(bool applyToLeft)
    {
        if(LevelGenerator.instance != null) lootPools = LevelGenerator.instance.levelInfo.lootPools;
        
        if (applyToLeft)
        {
            if (addChipDropdownLeft.value != 0)
            {
                int itemsToSkip = 0;
                
                for (int i = 0; i < lootPools.Length; i++)
                {
                    if (addChipDropdownLeft.value - 1 - itemsToSkip < lootPools[i].WeaponChips.Length)
                    {
                        PlayerBody.Instance().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip) lootPools[i].WeaponChips[addChipDropdownLeft.value -1 - itemsToSkip], true);
                        Debug.Log("Added weapon chip: " + lootPools[i].WeaponChips[addChipDropdownLeft.value -1 - itemsToSkip].itemName);
                    }
                    else
                    {
                        itemsToSkip += lootPools[i].WeaponChips.Length;
                    }
                }

                addChipDropdownLeft.value = 0;
            }
        }
        else
        {
            if (addChipDropdownRight.value != 0)
            {
                int itemsToSkip = 0;
                
                for (int i = 0; i < lootPools.Length; i++)
                {
                    if (addChipDropdownRight.value - 1 - itemsToSkip < lootPools[i].WeaponChips.Length)
                    {
                        PlayerBody.Instance().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip) lootPools[i].WeaponChips[addChipDropdownRight.value -1 - itemsToSkip], false);
                        Debug.Log("Added weapon chip: " + lootPools[i].WeaponChips[addChipDropdownRight.value -1 - itemsToSkip].itemName);
                    }
                    else
                    {
                        itemsToSkip += lootPools[i].WeaponChips.Length;
                    }
                }

                addChipDropdownRight.value = 0;
            }
        }
    }

    public void AddBodyChip()
    {
        if (bodyChipsDropdown.value != 0)
        {
            if(LevelGenerator.instance != null) lootPools = LevelGenerator.instance.levelInfo.lootPools;
            
            int itemsToSkip = 0;
                
            for (int i = 0; i < lootPools.Length; i++)
            {
                if (bodyChipsDropdown.value - 1 - itemsToSkip < lootPools[i].BodyChips.Length)
                {
                    PlayerBody.Instance().ApplyChip((BodyChip)lootPools[i].BodyChips[bodyChipsDropdown.value - 1 - itemsToSkip]);
                    Debug.Log("Added body chip: " + lootPools[i].BodyChips[bodyChipsDropdown.value - 1 - itemsToSkip].itemName);
                }
                else
                {
                    itemsToSkip += lootPools[i].BodyChips.Length;
                }
            }

            bodyChipsDropdown.value = 0;
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
       
        if (GameGeneralManager.instance != null)
        {
            GameGeneralManager.instance.ChangeScene(1);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
    
    public void SetGameSpeedSlider(Slider slider)
    {
        FindObjectOfType<pauseMenu>().gameSpeed = slider.value;
    }

    public void ToggleEnemyTime()
    {
        timeIsGoing = !timeIsGoing;
        bool freeze = !timeIsGoing;

        screenBorderAnimator.SetBool("TimeFreeze", !timeIsGoing);

        foreach (WaveSpawner spawner in FindObjectsOfType<WaveSpawner>(true))
        {
            spawner.spawning = !freeze;
        }
        
        foreach (BehaviourTree enemy in FindObjectsOfType<BehaviourTree>(true))
        {
            if (freeze)
            {
                enemy.Stop();
            }
            else
            {
                enemy.Resume();
            }
        }
        
        foreach (EnemyGun gun in FindObjectsOfType<EnemyGun>(true))
        {
            if (freeze)
            {
                gun.enabled = false;
                gun.StopAllCoroutines();
            }
            else
            {
                gun.enabled = true;
                gun.StartCoroutine(gun.FireOnRepeat());
            }
        }

        foreach (BasicBullet bullet in FindObjectsOfType<BasicBullet>(true))
        {
            if (freeze)
            {
                bullet.enabled = false;
                bullet.GetComponent<BasicBullet>().paused = true;
            }
            else
            {
                bullet.enabled = true;
                if(bullet.gameObject.activeSelf) bullet.GetComponent<BasicBullet>().paused = false;
            }
        }
        
        foreach (MoveProjectile bullet in FindObjectsOfType<MoveProjectile>(true))
        {
            if (freeze)
            {
                bullet.enabled = false;
            }
            else
            {
                bullet.enabled = true;
            }
        }
        
        foreach (TurretLook turret in FindObjectsOfType<TurretLook>(true))
        {
            if (freeze)
            {
                turret.enabled = false;
            }
            else
            {
                turret.enabled = true;
            }
        }
    }
}
