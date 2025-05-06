using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    
    public bool opened;
    public bool locked;
    public GameObject nextRoomSpawnPoint;
    
    [Header("Loot Display")]
    public LootType lootType;
    [SerializeField] private Image nextRoomLootDisplayImage;

    
    [Header("Opening and closing")]
    public UnityEvent onOpen;
    public UnityEvent onClose;
    
    [Header("Lock states")]
    public UnityEvent onUnlock;
    public UnityEvent onLock;
    public UnityEvent onFailToOpen;
    
    
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if(animator != null) animator.SetBool("IsOpen", opened);
        if(animator != null) animator.SetBool("IsLocked", locked);
    }

    public void SetDoorLootType(LootType type)
    {
        lootType = type;

        switch (type)
        {
            case LootType.weapon:
                nextRoomLootDisplayImage.sprite = LevelGenerator.instance.levelInfo.weaponLootImage;
                break;
            
            case LootType.weaponChip:
                nextRoomLootDisplayImage.sprite = LevelGenerator.instance.levelInfo.weaponChipLootImage;
                break;
            
            case LootType.mechChip:
                nextRoomLootDisplayImage.sprite = LevelGenerator.instance.levelInfo.chassisChipLootImage;
                break;
            
            case LootType.ordinance:
                nextRoomLootDisplayImage.sprite = LevelGenerator.instance.levelInfo.ordinanceLootImage;
                break;
            
            case LootType.chassis:
                nextRoomLootDisplayImage.sprite = LevelGenerator.instance.levelInfo.chassisLootImage;
                break;
        }
    }

    public void OpenDoor()
    {
        if (locked)
        {
            if(animator != null) animator.SetTrigger("AttemptToOpenLockedDoor");
            onFailToOpen.Invoke();
        }
        else
        {
            if (!opened)
            {
                opened = true;
                if(animator != null) animator.SetBool("IsOpen", true);
                onOpen.Invoke();
            }

        }
    }

    public void CloseDoor()
    {
        opened = false;
        if(animator != null) animator.SetBool("IsOpen", false);
        onClose.Invoke();
    }

    public void UnlockDoor()
    {
        locked = false;
        if(animator != null) animator.SetBool("IsLocked", false);
        onUnlock.Invoke();
    }

    public void LockDoor()
    {
        locked = true;
        if(animator != null) animator.SetBool("IsLocked", true);
        onLock.Invoke();
    }
}
