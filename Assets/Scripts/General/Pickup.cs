using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    
    [Header("Ui Stuff")]
    public GameObject uiPopup;
    [SerializeField] private GameObject singleItemPopup;
    [SerializeField] private GameObject twoOptionItemPopup;
    [SerializeField] private GameObject weaponPopup;
    [SerializeField] private GameObject weaponChipPopup;
    [SerializeField] private GameObject ordinancePopup;
    [SerializeField] private GameObject mechChipPopup;
    
    [SerializeField] private Button initiallySelectedButton;
    [SerializeField] private Button singleItemInitiallySelectedButton;

    [SerializeField] private Image[] newLootImages;
    [SerializeField] private TMP_Text[] newLootNames;
    [SerializeField] private TMP_Text[] newLootDescriptions;
    
    [SerializeField] private Image currentLeftWeaponImage;
    [SerializeField] private Image currentRightWeaponImage;
    [SerializeField] private Image[] currentLeftWeaponChipImages;
    [SerializeField] private Image[] currentRightWeaponChipImages;

    [Header("Item Stuff")]
    public Sprite itemDisplaySprite;
    public SpriteRenderer itemDisplayImage;
    public string pickupName;
    public string pickupDescription;
    
    public GameObject itemReference;
    public ScriptableObject ItemScriptableReference;
    
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
        
        animator.SetInteger("lootRarity", pickupRarity);
        
        if(itemDisplayImage != null && itemDisplaySprite != null) itemDisplayImage.sprite = itemDisplaySprite;

        if (itemDisplaySprite != null)
        {
            for (int i = 0; i < newLootImages.Length; i++)
            {
                newLootImages[i].sprite = itemDisplaySprite;
            }
        }

        if (pickupName != "")
        {
            for (int i = 0; i < newLootNames.Length; i++)
            {
                newLootNames[i].text = pickupName;
            }
        }

        if (pickupDescription != "")
        {
            for (int i = 0; i < newLootDescriptions.Length; i++)
            {
                newLootDescriptions[i].text = pickupDescription;
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

                PlayerBody.PlayBody().SetWeapon(itemReference, optionalData == 0);
                
                break;
            
            case pickupType.Chassis:
                
                break;
            
            case pickupType.Ordinance :
                
                break;

            case pickupType.WeaponChip:
                PlayerBody.PlayBody().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip)ItemScriptableReference, optionalData == 0);
                break;
            
            case pickupType.ChassisChip:
                
                break;
            
            case pickupType.OrdinanceChip:
                
                break;
            
            case pickupType.MovementChip:
                
                break;
            
            case pickupType.LegsChip:
                
                break;
        }
        
        animator.SetTrigger("openLoot");

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
        List<WeaponChip> leftChips = PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().leftMods;
                
        for (int i = 0; i < currentLeftWeaponChipImages.Length; i++)
        {
            if (i < leftChips.Count)
            {
                currentLeftWeaponChipImages[i].sprite = leftChips[i].chipImage;
                currentLeftWeaponChipImages[i].enabled = true;
            }
            else
            {
                currentLeftWeaponChipImages[i].enabled = false;
            }
        }

        List<WeaponChip> rightChips = PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().rightMods;
                
        for (int i = 0; i < currentRightWeaponChipImages.Length; i++)
        {
            if (i < rightChips.Count)
            {
                currentRightWeaponChipImages[i].sprite = rightChips[i].chipImage;
                currentRightWeaponChipImages[i].enabled = true;
            }
            else
            {
                currentRightWeaponChipImages[i].enabled = false;
            }
        }
    }
}
