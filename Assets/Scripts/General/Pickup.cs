using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum pickupType
{
    Weapon,
    Chassis,
    Ordinance,
    
    WeaponChip,
    ChassisChip,
    OrdinanceChip,
    MovementChip,
    LegsChip
}

public class Pickup : MonoBehaviour
{
    public int pickupRarity = 0;
    public pickupType pickupType = pickupType.ChassisChip;
    public PlayerPickup PlayerPickup;
    
    [Header("Ui Stuff")]
    public SpriteRenderer itemDisplayImage;
    public GameObject uiPopup;
    public Animator uiPopupAnimator;
    
    [SerializeField] private GameObject singleItemPopup;
    [SerializeField] private GameObject twoOptionItemPopup;
    [SerializeField] private GameObject weaponPopup;
    [SerializeField] private GameObject weaponChipPopup;
    [SerializeField] private GameObject ordinancePopup;
    [SerializeField] private GameObject mechChipPopup;

    [SerializeField] private Button leftSelectButton;
    [SerializeField] private Button rightSelectButton;
    [SerializeField] private Button initiallySelectedButton;
    [SerializeField] private Button singleItemInitiallySelectedButton;

    [SerializeField] private Image[] newLootImages;
    [SerializeField] private TMP_Text[] newLootNames;
    [SerializeField] private TMP_Text[] newLootDescriptions;

    [SerializeField] private Image currentLeftWeaponImage;
    [SerializeField] private Image currentRightWeaponImage;
    [SerializeField] private Image[] currentLeftWeaponChipImages;
    [SerializeField] private Image[] currentRightWeaponChipImages;

    public Animator animator;

    private void Start()
    {
        Animator[] anims = GetComponentsInChildren<Animator>();
        for (int i = 0; i < anims.Length; i++)
        {
            if (anims[i] != animator)
            {
            }
        }

        pickupRarity = PlayerPickup.rarity;
        itemDisplayImage.sprite = PlayerPickup.mySprite;
        pickupType = PlayerPickup.PickupType;
        
        animator.SetInteger("lootRarity", pickupRarity);
        
        for (int i = 0; i < newLootImages.Length; i++)
        {
            newLootImages[i].sprite = PlayerPickup.mySprite;
        }

        for (int i = 0; i < newLootNames.Length; i++) 
        {
            newLootNames[i].text = PlayerPickup.itemName;
        }
        
        for (int i = 0; i < newLootDescriptions.Length; i++)
        {
            newLootDescriptions[i].text = PlayerPickup.description;
        }
        
    }

    private void Update()
    {
        if (uiPopup.activeSelf)
        {
            GameObject curSelected = EventSystem.current.currentSelectedGameObject;

            if (curSelected == leftSelectButton.gameObject)
            {
                print("animating pickup");
                uiPopupAnimator.SetInteger("Selected", 1);
            }
            
            if (curSelected == initiallySelectedButton.gameObject)
            {
                uiPopupAnimator.SetInteger("Selected", 2);
            }

            if (curSelected == rightSelectButton.gameObject)
            {
                uiPopupAnimator.SetInteger("Selected", 3);
            }
        }
    }

    public void TryPickup()
    {
        singleItemPopup.SetActive(false);
        twoOptionItemPopup.SetActive(false);
        
        switch (pickupType)
        {
            case pickupType.Weapon:
                PlayerBody.PlayBody().StopParts(false,false);

                DisplayWeaponChips();

                uiPopup.SetActive(true);
                twoOptionItemPopup.SetActive(true);
                weaponPopup.SetActive(true);
                
                initiallySelectedButton.Select();
                break;
            
            case pickupType.Chassis:
                PlayerBody.PlayBody().StopParts(false,false);
                
                uiPopup.SetActive(true);
                singleItemPopup.SetActive(true);
                
                singleItemInitiallySelectedButton.Select();
                break;
            
            case pickupType.Ordinance :
                PlayerBody.PlayBody().StopParts(false,false);
                
                uiPopup.SetActive(true);
                singleItemPopup.SetActive(true);
                ordinancePopup.SetActive(true);
                
                singleItemInitiallySelectedButton.Select();
                break;
            
            case pickupType.WeaponChip:
                PlayerBody.PlayBody().StopParts(false,false);
                
                DisplayWeaponChips();
                
                uiPopup.SetActive(true);
                twoOptionItemPopup.SetActive(true);
                weaponChipPopup.SetActive(true);
                
                initiallySelectedButton.Select();
                break;
            
            case pickupType.ChassisChip:
                OnPickup(0);
                break;
            
            case pickupType.OrdinanceChip:
                OnPickup(0);
                break;
            
            case pickupType.MovementChip:
                OnPickup(0);
                break;
        }
    }
    
    public void OnPickup(int optionalData)
    {
        PlayerBody.PlayBody().StopParts(true,true);
        Debug.Log("Picking up " + name);
        
        switch (pickupType)
        {
            case pickupType.Weapon:

                PlayerBody.PlayBody().SetWeapon((WeaponPickup)PlayerPickup, optionalData == 0);
                
                break;
            
            case pickupType.Chassis:
                
                break;
            
            case pickupType.Ordinance :
                
                break;

            case pickupType.WeaponChip:
                PlayerBody.PlayBody().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip) PlayerPickup, optionalData == 0);
                break;
            
            case pickupType.ChassisChip:
                PlayerBody.PlayBody().ApplyChip((BodyChip)PlayerPickup);
                break;
            
            case pickupType.OrdinanceChip:
                
                break;
            
            case pickupType.MovementChip:
                
                break;
            
            case pickupType.LegsChip:
                
                break;
        }
        
        animator.SetTrigger("openLoot");
        GetComponentInChildren<Interactable>(true).canInteract = false;

        foreach (var pickup in FindObjectsOfType<Pickup>())
        {
            if (pickup != this)
            {
                pickup.animator.SetTrigger("lootLocked");
                pickup.GetComponentInChildren<Interactable>(true).canInteract = false;
            }
        }
        
        if(uiPopup != null) uiPopup.SetActive(false);
    }

    public void CancelPickup()
    {
        PlayerBody.PlayBody().StopParts(true,true);
        if(uiPopup != null) uiPopup.SetActive(false);
    }

    private void DisplayWeaponChips()
    {
        if (PlayerBody.PlayBody().weaponHolder.leftWeapon != null)
        {
            currentLeftWeaponImage.sprite = PlayerBody.PlayBody().weaponHolder.leftWInfo.mySprite;
            currentLeftWeaponImage.enabled = true;
            
            List<WeaponChip> leftChips = PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().leftMods;
                
            for (int i = 0; i < currentLeftWeaponChipImages.Length; i++)
            {
                if (i < leftChips.Count)
                {
                    currentLeftWeaponChipImages[i].sprite = leftChips[i].mySprite;
                    currentLeftWeaponChipImages[i].enabled = true;
                }
                else
                {
                    currentLeftWeaponChipImages[i].enabled = false;
                }
            }
        }

        if (PlayerBody.PlayBody().weaponHolder.rightWeapon != null)
        {
            currentRightWeaponImage.sprite = PlayerBody.PlayBody().weaponHolder.rightWInfo.mySprite;
            currentRightWeaponImage.enabled = true;

            List<WeaponChip> rightChips = PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().rightMods;
                
            for (int i = 0; i < currentRightWeaponChipImages.Length; i++)
            {
                if (i < rightChips.Count)
                {
                    currentRightWeaponChipImages[i].sprite = rightChips[i].mySprite;
                    currentRightWeaponChipImages[i].enabled = true;
                }
                else
                {
                    currentRightWeaponChipImages[i].enabled = false;
                }
            }
        }
    }
}
