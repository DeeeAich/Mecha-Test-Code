using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    public bool mouseControls = true;
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
    
    [Header("Other References")]
    public Animator animator;
    [SerializeField] private GameObject imageDisplayPoint;
    [SerializeField] private GameObject hologramSpawnPoint;
    
    private bool open;
    private float buttonInteractBlockTimer;
    private float closeUiTimer;
    private Button buttonToInitiallySelect;

    private void Start()
    {
        Animator[] anims = GetComponentsInChildren<Animator>();
        for (int i = 0; i < anims.Length; i++)
        {
            if (anims[i] != animator)
            {
            }
        }
        
        RigLootBox();
    }

    private void RigLootBox()
    {
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

        switch (pickupType)
        {
            case pickupType.Weapon:
                animator.SetBool("isWeapon", true);
                imageDisplayPoint.SetActive(false);
                if(PlayerPickup.hologramReference != null) Instantiate(PlayerPickup.hologramReference, hologramSpawnPoint.transform);
                break;
        }
    }

    private void Update()
    {
        if (uiPopup.activeSelf)
        {
            GameObject curSelected = EventSystem.current.currentSelectedGameObject;

            if (mouseControls)
            {
                if (CheckMouseInBounds(leftSelectButton.GetComponent<RectTransform>()))
                {
                    print("left select");
                    leftSelectButton.Select();
                    uiPopupAnimator.SetInteger("Selected", 1);
                }
                else if (CheckMouseInBounds(initiallySelectedButton.GetComponent<RectTransform>()))
                {
                    print("entry select");
                    initiallySelectedButton.Select();
                    uiPopupAnimator.SetInteger("Selected", 2);
                }
                else if (CheckMouseInBounds(rightSelectButton.GetComponent<RectTransform>()))
                {
                    rightSelectButton.Select();
                    print("Right select");
                    uiPopupAnimator.SetInteger("Selected", 3);
                }

            }
            else
            {
                if (curSelected == leftSelectButton.gameObject)
                {
                    print("left select");
                    uiPopupAnimator.SetInteger("Selected", 1);
                }
                else if (curSelected == initiallySelectedButton.gameObject)
                {
                    print("entry select");
                    uiPopupAnimator.SetInteger("Selected", 2);
                }
                else if (curSelected == rightSelectButton.gameObject)
                {
                    print("Right select");
                    uiPopupAnimator.SetInteger("Selected", 3);
                }
            }

            if (buttonInteractBlockTimer > 0)
            {
                buttonInteractBlockTimer -= Time.deltaTime;

                if (buttonInteractBlockTimer <= 0)
                {
                    leftSelectButton.interactable = true;
                    rightSelectButton.interactable = true;
                    initiallySelectedButton.interactable = true;
                    
                    buttonToInitiallySelect.Select();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (closeUiTimer > 0)
        {
            closeUiTimer -= Time.fixedDeltaTime;

            if (closeUiTimer <= 0)
            {
                uiPopup.SetActive(false);
            }
        }
    }

    private bool CheckMouseInBounds(RectTransform bounds)
    {
        bool inBounds = true;

        if (Mouse.current.position.x.value < bounds.position.x - bounds.rect.width / 2) inBounds = false;
        if (Mouse.current.position.x.value > bounds.position.x + bounds.rect.width / 2) inBounds = false;
        if (Mouse.current.position.y.value < bounds.position.y - bounds.rect.height / 2) inBounds = false;
        if (Mouse.current.position.y.value > bounds.position.y + bounds.rect.height / 2) inBounds = false;

        return inBounds;
    }

    public void TryPickup()
    {
        singleItemPopup.SetActive(false);
        twoOptionItemPopup.SetActive(false);

        if (!open)
        {
            switch (pickupType)
            {
                case pickupType.Weapon:
                    PlayerBody.Instance().StopParts(false,false);

                    DisplayWeaponChips();

                    uiPopup.SetActive(true);
                    twoOptionItemPopup.SetActive(true);
                    weaponPopup.SetActive(true);
                
                    buttonToInitiallySelect = initiallySelectedButton;
                    break;
            
                case pickupType.Chassis:
                    PlayerBody.Instance().StopParts(false,false);
                
                    uiPopup.SetActive(true);
                    singleItemPopup.SetActive(true);
                
                    buttonToInitiallySelect = singleItemInitiallySelectedButton;
                    break;
            
                case pickupType.Ordinance :
                    PlayerBody.Instance().StopParts(false,false);
                
                    uiPopup.SetActive(true);
                    singleItemPopup.SetActive(true);
                    ordinancePopup.SetActive(true);
                
                    buttonToInitiallySelect = singleItemInitiallySelectedButton;
                    break;
            
                case pickupType.WeaponChip:
                    PlayerBody.Instance().StopParts(false,false);
                
                    DisplayWeaponChips();
                
                    uiPopup.SetActive(true);
                    twoOptionItemPopup.SetActive(true);
                    weaponChipPopup.SetActive(true);
                
                    buttonToInitiallySelect = initiallySelectedButton;
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
            
            GetComponentInChildren<Interactable>(true).canInteract = false;
            
            leftSelectButton.interactable = false;
            rightSelectButton.interactable = false;
            initiallySelectedButton.interactable = false;
            buttonInteractBlockTimer = 0.25f;
            open = true;
        }


    }
    
    public void OnPickup(int optionalData)
    {
        PlayerBody.Instance().StopParts(true,true);
        Debug.Log("Picking up " + name);
        
        switch (pickupType)
        {
            case pickupType.Weapon:

                PlayerPickup newPickup;
                if (optionalData == 0)
                {
                    newPickup = PlayerBody.Instance().weaponHolder.leftWInfo;
                }
                else
                {
                    newPickup = PlayerBody.Instance().weaponHolder.rightWInfo;
                }
                
                PlayerBody.Instance().SetWeapon((WeaponPickup)PlayerPickup, optionalData == 0);

                PlayerPickup = newPickup;
                RigLootBox();
                
                break;
            
            case pickupType.Chassis:
                
                break;
            
            case pickupType.Ordinance :
                
                break;

            case pickupType.WeaponChip:
                PlayerBody.Instance().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip) PlayerPickup, optionalData == 0);
                break;
            
            case pickupType.ChassisChip:
                PlayerBody.Instance().ApplyChip((BodyChip)PlayerPickup);
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
        
        if(uiPopup != null) closeUiTimer = 0.5f;
    }

    public void CancelPickup()
    {
        open = false;
        PlayerBody.Instance().StopParts(true,true);
        GetComponentInChildren<Interactable>(true).canInteract = true;
        if(uiPopup != null) uiPopup.SetActive(false);
    }

    private void DisplayWeaponChips()
    {
        if (PlayerBody.Instance().weaponHolder.leftWeapon != null)
        {
            currentLeftWeaponImage.sprite = PlayerBody.Instance().weaponHolder.leftWInfo.mySprite;
            currentLeftWeaponImage.enabled = true;
            
            List<WeaponChip> leftChips = PlayerBody.Instance().GetComponent<PlayerWeaponControl>().leftMods;
                
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

        if (PlayerBody.Instance().weaponHolder.rightWeapon != null)
        {
            currentRightWeaponImage.sprite = PlayerBody.Instance().weaponHolder.rightWInfo.mySprite;
            currentRightWeaponImage.enabled = true;

            List<WeaponChip> rightChips = PlayerBody.Instance().GetComponent<PlayerWeaponControl>().rightMods;
                
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
