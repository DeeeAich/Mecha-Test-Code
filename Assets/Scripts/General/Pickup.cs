using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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
    [Header("Editor Only")]
    [SerializeField] private bool EDITORTriggerSpawn = false;
    
    [Header("Pickup Info")]
    public pickupType pickupType = pickupType.ChassisChip;
    public PlayerPickup[] PlayerPickups;

    public UnityEvent onPickedUpEvent;

    [Header("Ui Stuff")] 
    public bool mouseControls = true;
    public SpriteRenderer[] itemDisplayImagesOverBoxes;
    public GameObject uiPopup;

    [Header("Loot Options Menu")]
    [SerializeField] private GameObject lootOptionsMenu;

    [SerializeField] private Image[] lootOptionsImages;
    [SerializeField] private TMP_Text[] lootOptionsNames;
    [SerializeField] private TMP_Text[] lootOptionsDescriptions;
    
    [Header("Apllication Choice Menu")]
    [SerializeField] private GameObject choiceMenu;
    [FormerlySerializedAs("uiPopupAnimator")] public Animator choiceMenuAnimator;
    
    [SerializeField] private Image[] newLootImages;
    [SerializeField] private TMP_Text[] newLootNames;
    [SerializeField] private TMP_Text[] newLootDescriptions;
    
    [SerializeField] private Image currentLeftWeaponImage;
    [SerializeField] private Image currentRightWeaponImage;
    [SerializeField] private Image[] currentLeftWeaponChipImages;
    [SerializeField] private Image[] currentRightWeaponChipImages;
    
    [SerializeField] private Button leftSelectButton;
    [SerializeField] private Button rightSelectButton;
    [SerializeField] private Button initiallySelectedButton;
    
    [SerializeField] private GameObject singleItemPopup;
    [SerializeField] private GameObject twoOptionItemPopup;
    [SerializeField] private GameObject weaponPopup;
    [SerializeField] private GameObject weaponChipPopup;
    [SerializeField] private GameObject ordinancePopup;
    [SerializeField] private GameObject mechChipPopup;
    
    [Header("Other References")]
    public Animator[] animators;
    [SerializeField] private GameObject[] imageDisplayPoints;
    [SerializeField] private GameObject[] hologramSpawnPoints;
    
    [HideInInspector] public bool open;
    private float buttonInteractBlockTimer;
    private float closeUiTimer;
    private bool canUse = true;
    private int pickupIndex = 0;
    

    private void Start()
    {
        RigLootBox();
    }

    private void RigLootBox()
    {
        pickupType = PlayerPickups[0].PickupType;

        for (int i = 0; i < PlayerPickups.Length; i++)
        {
            itemDisplayImagesOverBoxes[i].sprite = PlayerPickups[i].mySprite;
            animators[i].SetInteger("lootRarity", PlayerPickups[i].rarity);

            lootOptionsImages[i].sprite = PlayerPickups[i].mySprite;
            lootOptionsNames[i].text = PlayerPickups[i].itemName;
            lootOptionsDescriptions[i].text = PlayerPickups[i].description;
            
            switch (pickupType)
            {
                case pickupType.Weapon:
                    animators[i].SetBool("isWeapon", true);
                    imageDisplayPoints[i].SetActive(false);
                    if(PlayerPickups[i].hologramReference != null) Instantiate(PlayerPickups[i].hologramReference, hologramSpawnPoints[i].transform);
                    break;
            }
        }
    }

    public void RigChoiceMenu(int index)
    {
        pickupIndex = index;

        for (int i = 0; i < newLootDescriptions.Length; i++)
        {
            newLootDescriptions[i].text = PlayerPickups[pickupIndex].description;
        }
        
        for (int i = 0; i < newLootNames.Length; i++)
        {
            newLootNames[i].text = PlayerPickups[pickupIndex].itemName;
        }
        
        for (int i = 0; i < newLootImages.Length; i++)
        {
            newLootImages[i].sprite = PlayerPickups[pickupIndex].mySprite;
        }
        
        lootOptionsMenu.SetActive(false);
        choiceMenu.SetActive(true);
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
                    leftSelectButton.Select();
                    choiceMenuAnimator.SetInteger("Selected", 1);
                }
                else if (CheckMouseInBounds(initiallySelectedButton.GetComponent<RectTransform>()))
                {
                    initiallySelectedButton.Select();
                    choiceMenuAnimator.SetInteger("Selected", 2);
                }
                else if (CheckMouseInBounds(rightSelectButton.GetComponent<RectTransform>()))
                {
                    rightSelectButton.Select();
                    choiceMenuAnimator.SetInteger("Selected", 3);
                }

            }
            else
            {
                if (curSelected == leftSelectButton.gameObject)
                {
                    print("left select");
                    choiceMenuAnimator.SetInteger("Selected", 1);
                }
                else if (curSelected == initiallySelectedButton.gameObject)
                {
                    print("entry select");
                    choiceMenuAnimator.SetInteger("Selected", 2);
                }
                else if (curSelected == rightSelectButton.gameObject)
                {
                    print("Right select");
                    choiceMenuAnimator.SetInteger("Selected", 3);
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
                    
                    initiallySelectedButton.Select();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (EDITORTriggerSpawn)
        {
            EDITORTriggerSpawn = false;
            
            SpawnLootBox();
        }
        
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

    public void OpenPickupMenu()
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
                    
                    break;
            
                case pickupType.Chassis:
                    PlayerBody.Instance().StopParts(false,false);
                
                    uiPopup.SetActive(true);
                    singleItemPopup.SetActive(true);
                
        
                    break;
            
                case pickupType.Ordinance :
                    PlayerBody.Instance().StopParts(false,false);
                
                    uiPopup.SetActive(true);
                    singleItemPopup.SetActive(true);
                    ordinancePopup.SetActive(true);
                    
                    break;
            
                case pickupType.WeaponChip:
                    PlayerBody.Instance().StopParts(false,false);
                
                    DisplayWeaponChips();
                
                    uiPopup.SetActive(true);
                    twoOptionItemPopup.SetActive(true);
                    weaponChipPopup.SetActive(true);
                    
                    break;
            
                case pickupType.ChassisChip:
                    break;
            
                case pickupType.OrdinanceChip:
                    break;
            
                case pickupType.MovementChip:
                    break;
            }
            
            GetComponentInChildren<Interactable>(true).canInteract = false;
            
            leftSelectButton.interactable = false;
            rightSelectButton.interactable = false;
            initiallySelectedButton.interactable = false;
            buttonInteractBlockTimer = 0.25f;
            open = true;
            
            InputAction pauseAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Pause"];
        }
    }
    
    public void ClosePickupMenu()
    {
        open = false;
        PlayerBody.Instance().StopParts(true,true);
        GetComponentInChildren<Interactable>(true).canInteract = true;
        if(uiPopup != null) uiPopup.SetActive(false);

        choiceMenu.SetActive(false);
        lootOptionsMenu.SetActive(true);
    }
    
    public void PickupItem(bool optionalDataApplyToLeft)
    {
        if(!canUse) return;

        PlayerBody.Instance().StopParts(true,true);
        Debug.Log("Picking up " + name);
        
        switch (pickupType)
        {
            case pickupType.Weapon:

                PlayerPickup newPickup;
                
                if (optionalDataApplyToLeft)
                {
                    newPickup = PlayerBody.Instance().weaponHolder.leftWInfo;
                }
                else
                {
                    newPickup = PlayerBody.Instance().weaponHolder.rightWInfo;
                }
                
                PlayerBody.Instance().SetWeapon((WeaponPickup)PlayerPickups[pickupIndex], optionalDataApplyToLeft);

                PlayerPickups[pickupIndex] = newPickup;
                if (PlayerPickups[pickupIndex] != null)
                {
                    RigLootBox();
                    // keep box open, swap loot from hand to box
                }
                else
                {
                    // insert animation close box stuff
                }
                
                break;
            
            case pickupType.Chassis:
                
                break;
            
            case pickupType.Ordinance :
                
                break;

            case pickupType.WeaponChip:
                PlayerBody.Instance().GetComponent<IWeaponModifiable>().ApplyChip((WeaponChip) PlayerPickups[pickupIndex], optionalDataApplyToLeft);
                break;
            
            case pickupType.ChassisChip:
                PlayerBody.Instance().ApplyChip((BodyChip)PlayerPickups[pickupIndex]);
                break;
            
            case pickupType.OrdinanceChip:
                
                break;
            
            case pickupType.MovementChip:
                
                break;
            
            case pickupType.LegsChip:
                
                break;
        }
        
        animators[pickupIndex].SetTrigger("openLoot");
        GetComponentInChildren<Interactable>(true).canInteract = false;

        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetTrigger("lootLocked");
        }
        
        
        if(uiPopup != null) closeUiTimer = 0.5f;

        canUse = false;
        
        onPickedUpEvent.Invoke();
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

    public void SpawnLootBox()
    {
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetTrigger("spawnLoot");
        }
        GetComponentInChildren<Interactable>(true).gameObject.SetActive(true);
    }
}
