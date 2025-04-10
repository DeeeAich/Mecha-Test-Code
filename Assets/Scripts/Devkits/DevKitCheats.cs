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
    [SerializeField] private LootPoolScriptable lootPool;
    [SerializeField] private LootPoolScriptable[] additionalLootPools;

    [SerializeField] private TMP_Dropdown leftGunDropdown;
    [SerializeField] private TMP_Dropdown rightGunDropdown;
    [SerializeField] private TMP_Dropdown chassisDropdown;

    [SerializeField] private TMP_Dropdown addChipDropdownLeft;
    [SerializeField] private TMP_Dropdown addChipDropdownRight;

    [SerializeField] private TMP_Dropdown bodyChipsDropdown;

    private int[] loadout;
    private bool timeIsGoing = true;

    private void Start()
    {

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
            leftGunDropdown.options.Add(new TMP_Dropdown.OptionData( lootPool.Weapons[i].itemName, lootPool.Weapons[i].mySprite));
            rightGunDropdown.options.Add(new TMP_Dropdown.OptionData( lootPool.Weapons[i].itemName, lootPool.Weapons[i].mySprite));
        }

        for (int i = 0; i <  lootPool.WeaponChips.Length; i++)
        {
            addChipDropdownLeft.options.Add(new TMP_Dropdown.OptionData(lootPool.WeaponChips[i].name, lootPool.WeaponChips[i].mySprite));
            addChipDropdownRight.options.Add(new TMP_Dropdown.OptionData(lootPool.WeaponChips[i].name, lootPool.WeaponChips[i].mySprite));
        }

        for (int i = 0; i < lootPool.BodyChips.Length; i++)
        {
            bodyChipsDropdown.options.Add(new TMP_Dropdown.OptionData(lootPool.BodyChips[i].name, lootPool.BodyChips[i].mySprite));
        }

        if (additionalLootPools.Length > 0)
        {
            for (int i = 0; i < additionalLootPools.Length; i++)
            {
                if (additionalLootPools[i].Weapons.Length > 0)
                {
                    for (int j = 0; j <  additionalLootPools[i].Weapons.Length; j++)
                    {
                        leftGunDropdown.options.Add(new TMP_Dropdown.OptionData( additionalLootPools[i].Weapons[j].itemName, additionalLootPools[i].Weapons[j].mySprite));
                        rightGunDropdown.options.Add(new TMP_Dropdown.OptionData( additionalLootPools[i].Weapons[j].itemName, additionalLootPools[i].Weapons[j].mySprite));
                    }
                }

                if (additionalLootPools[i].BodyChips.Length > 0)
                {
                    for (int j = 0; j < additionalLootPools[i].BodyChips.Length; j++)
                    {
                        bodyChipsDropdown.options.Add(new TMP_Dropdown.OptionData(additionalLootPools[i].BodyChips[j].name, additionalLootPools[i].BodyChips[j].mySprite));
                    }
                }

                if (additionalLootPools[i].WeaponChips.Length > 0)
                {
                    for (int j = 0; j <  additionalLootPools[i].WeaponChips.Length; j++)
                    {
                        addChipDropdownLeft.options.Add(new TMP_Dropdown.OptionData(additionalLootPools[i].WeaponChips[j].name, additionalLootPools[i].WeaponChips[j].mySprite));
                        addChipDropdownRight.options.Add(new TMP_Dropdown.OptionData(additionalLootPools[i].WeaponChips[j].name, additionalLootPools[i].WeaponChips[j].mySprite));
                    }
                }

            }
        }


        loadout = new int[3];
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
                if (leftGunDropdown.value < lootPool.Weapons.Length)
                {
                    PlayerBody.Instance().SetWeapon((WeaponPickup) lootPool.Weapons[leftGunDropdown.value - 1], true);
                }
                else
                {
                    PlayerBody.Instance().SetWeapon((WeaponPickup) additionalLootPools[0].Weapons[leftGunDropdown.value - 1 - lootPool.Weapons.Length], true);
                }
        

                leftGunDropdown.value = 0;
            }        
        }
        else
        {
            if (rightGunDropdown.value != 0)
            {
                if (rightGunDropdown.value < lootPool.Weapons.Length)
                {
                    PlayerBody.Instance().SetWeapon((WeaponPickup) lootPool.Weapons[rightGunDropdown.value - 1], false);
                }
                else
                {
                    PlayerBody.Instance().SetWeapon((WeaponPickup) additionalLootPools[0].Weapons[rightGunDropdown.value - 1 - lootPool.Weapons.Length], false);
                }

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
                PlayerBody.Instance().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip) lootPool.WeaponChips[addChipDropdownLeft.value -1], true);
                addChipDropdownLeft.value = 0;
            }

        }
        else
        {
            if (addChipDropdownRight.value != 0)
            {
                PlayerBody.Instance().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip) lootPool.WeaponChips[addChipDropdownRight.value - 1], false);
                addChipDropdownRight.value = 0;
            }
        }
    }

    public void AddBodyChip()
    {
        if (bodyChipsDropdown.value != 0)
        {
            if(LevelGenerator.instance != null) lootPool = LevelGenerator.instance.levelInfo.lootPool;
        
            PlayerBody.Instance().ApplyChip((BodyChip)lootPool.BodyChips[bodyChipsDropdown.value - 1]);

            addChipDropdownLeft.value = 0;
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
                bullet.StopAllCoroutines();
            }
            else
            {
                bullet.enabled = true;
                if(bullet.gameObject.activeSelf) bullet.StartCoroutine(bullet.AutoReset());
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
