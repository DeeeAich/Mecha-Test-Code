using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [Header("Editor Only")] [SerializeField]
    private bool EDITORTriggerSpawn = false;

    [Header("Pickup Info")] public pickupType pickupType = pickupType.ChassisChip;
    public PlayerPickup[] PlayerPickups;

    public UnityEvent onPickedUpEvent;

    [Header("Ui Stuff")] public bool mouseControls = true;
    public SpriteRenderer[] itemDisplayImagesOverBoxes;
    public GameObject uiPopup;
    [SerializeField] private RectTransform[] buttonsWithAssociatedAnimators;
    [SerializeField] private Animator[] associatedAnimators;

    [Header("Loot Options Menu")] [SerializeField]
    private GameObject lootOptionsMenu;

    [SerializeField] private Image[] lootOptionsImages;
    [SerializeField] private TMP_Text[] lootOptionsNames;
    [SerializeField] private TMP_Text[] lootOptionsDescriptions;

    [FormerlySerializedAs("lootOptionsButtons")] [SerializeField]
    private Button[] lootOptionsButtonsWithAssociatedAnimators;

    [SerializeField] private Animator[] lootOptionsAssociatedAnimators;

    [Header("Current Gear Display")] [SerializeField]
    private GameObject weaponsChipsDisplay;

    [SerializeField] private GameObject mechChipsDisplay;

    [SerializeField] private GameObject choiceButtons;
    [SerializeField] private GameObject weaponsChoiceButtons;
    [SerializeField] private GameObject mechChoiceButton;

    [SerializeField] private Image currentLeftWeaponImage;
    [SerializeField] private Image currentRightWeaponImage;
    [SerializeField] private Image[] currentLeftWeaponChipImages;
    [SerializeField] private Image[] currentRightWeaponChipImages;
    [SerializeField] private Image[] currentMechChipImages;

    [Header("Other References")] public Animator[] boxAnimators;
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
            boxAnimators[i].SetInteger("lootRarity", PlayerPickups[i].rarity);

            lootOptionsImages[i].sprite = PlayerPickups[i].mySprite;
            lootOptionsNames[i].text = PlayerPickups[i].itemName;
            lootOptionsDescriptions[i].text = PlayerPickups[i].description;

            switch (pickupType)
            {
                case pickupType.Weapon:
                    boxAnimators[i].SetBool("isWeapon", true);
                    imageDisplayPoints[i].SetActive(false);
                    if (PlayerPickups[i].hologramReference != null)
                        Instantiate(PlayerPickups[i].hologramReference, hologramSpawnPoints[i].transform);
                    break;
            }
        }

        for (int i = 0; i < associatedAnimators.Length; i++)
        {
            associatedAnimators[i].keepAnimatorStateOnDisable = true;
        }
    }

    public void RigChoiceMenu(int index)
    {
        pickupIndex = index;

        choiceButtons.SetActive(true);

        for (int i = 0; i < lootOptionsButtonsWithAssociatedAnimators.Length; i++)
        {
            lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsSelected", false);
            lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsGreyedOut", false);
            lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsReturn", false);

            if (i == index)
            {
                lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsReturn", true);
            }
            else
            {
                lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsGreyedOut", true);
            }
        }
    }

    private void Update()
    {
        if (uiPopup.activeSelf)
        {
            if (mouseControls)
            {
                for (int i = 0; i < buttonsWithAssociatedAnimators.Length; i++)
                {
                    if (buttonsWithAssociatedAnimators[i].gameObject.activeInHierarchy &&
                        associatedAnimators[i].gameObject.activeInHierarchy)
                    {
                        associatedAnimators[i].SetBool("IsSelected",
                            CheckMouseInBounds(buttonsWithAssociatedAnimators[i]));
                    }
                }
            }
            else
            {
                for (int i = 0; i < buttonsWithAssociatedAnimators.Length; i++)
                {
                    if (buttonsWithAssociatedAnimators[i].gameObject.activeInHierarchy &&
                        associatedAnimators[i].gameObject.activeInHierarchy)
                    {
                        associatedAnimators[i].GetComponent<Animator>().SetBool("IsSelected",
                            EventSystem.current.currentSelectedGameObject ==
                            buttonsWithAssociatedAnimators[i].gameObject);
                    }
                }
            }

            if (buttonInteractBlockTimer > 0)
            {
                buttonInteractBlockTimer -= Time.deltaTime;

                if (buttonInteractBlockTimer <= 0)
                {
                    lootOptionsButtonsWithAssociatedAnimators[1].Select();
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
        if (!open)
        {
            switch (pickupType)
            {
                case pickupType.Weapon:
                    DisplayWeaponChips();
                    break;

                case pickupType.Chassis:
                    DisplayMechChips();
                    break;

                case pickupType.Ordinance:
                    break;

                case pickupType.WeaponChip:

                    DisplayWeaponChips();
                    break;

                case pickupType.ChassisChip:
                    DisplayMechChips();
                    break;

                case pickupType.OrdinanceChip:
                    break;

                case pickupType.MovementChip:
                    break;
            }

            PlayerBody.Instance().StopParts(false, false);
            uiPopup.SetActive(true);

            GetComponentInChildren<Interactable>(true).canInteract = false;
            buttonInteractBlockTimer = 0.5f;
            open = true;
        }
    }

    public void ClosePickupMenu()
    {
        for (int i = 0; i < associatedAnimators.Length; i++)
        {
            if (associatedAnimators[i].gameObject.activeSelf)
            {
                associatedAnimators[i].GetComponent<Animator>().SetBool("IsSelected", false);
            }
        }

        for (int i = 0; i < lootOptionsAssociatedAnimators.Length; i++)
        {
            if (lootOptionsAssociatedAnimators[i].gameObject.activeSelf)
            {
                lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsGreyedOut", false);
                lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsReturn", false);
            }
        }

        /*
        for (int i = 0; i < lootOptionsButtonsWithAssociatedAnimators.Length; i++)
        {
            
            lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsSelected", false);
            lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsGreyedOut", false);
            lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsReturn", false);
        }
        */

        open = false;
        PlayerBody.Instance().StopParts(true, true);
        GetComponentInChildren<Interactable>(true).canInteract = true;
        if (uiPopup != null) uiPopup.SetActive(false);


        choiceButtons.SetActive(false);
        lootOptionsMenu.SetActive(true);
    }

    public void PickupItem(bool optionalDataApplyToLeft)
    {
        if (!canUse) return;

        PlayerBody.Instance().StopParts(true, true);
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

                PlayerBody.Instance().SetWeapon((WeaponPickup) PlayerPickups[pickupIndex], optionalDataApplyToLeft);

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

            case pickupType.Ordinance:

                break;

            case pickupType.WeaponChip:
                PlayerBody.Instance().GetComponent<IWeaponModifiable>()
                    .ApplyChip((WeaponChip) PlayerPickups[pickupIndex], optionalDataApplyToLeft);
                break;

            case pickupType.ChassisChip:
                PlayerBody.Instance().ApplyChip((BodyChip) PlayerPickups[pickupIndex]);
                break;

            case pickupType.OrdinanceChip:

                break;

            case pickupType.MovementChip:

                break;

            case pickupType.LegsChip:

                break;
        }

        boxAnimators[pickupIndex].SetTrigger("openLoot");
        GetComponentInChildren<Interactable>(true).canInteract = false;

        for (int i = 0; i < boxAnimators.Length; i++)
        {
            boxAnimators[i].SetTrigger("lootLocked");
        }


        if (uiPopup != null) closeUiTimer = 0.5f;

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

        weaponsChipsDisplay.SetActive(true);
        weaponsChoiceButtons.SetActive(true);
    }

    private void DisplayMechChips()
    {
        for (int i = 0; i < currentMechChipImages.Length; i++)
        {
            if (PlayerBody.Instance().chipsInserted.Count < i)
            {
                currentMechChipImages[i].sprite = PlayerBody.Instance().chipsInserted[i].mySprite;
                currentMechChipImages[i].enabled = true;
            }

            currentMechChipImages[i].enabled = false;
        }

        mechChipsDisplay.SetActive(true);
        mechChoiceButton.SetActive(false);
    }

    public void SpawnLootBox()
    {
        for (int i = 0; i < boxAnimators.Length; i++)
        {
            boxAnimators[i].SetTrigger("spawnLoot");
        }

        GetComponentInChildren<Interactable>(true).gameObject.SetActive(true);
    }
}