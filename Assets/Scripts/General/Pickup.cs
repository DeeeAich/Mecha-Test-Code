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
    MovementChip
}

public class Pickup : MonoBehaviour
{
    [Header("Editor Only")] [SerializeField]
    private bool EDITORTriggerSpawn = false;

    [Header("Pickup Info")]
    [FormerlySerializedAs("PlayerPickups")] public PlayerPickup[] playerPickups;

    public UnityEvent onPickedUpEvent;
    public UnityEvent onMenuOpened;
    public UnityEvent onChoiceMenuOpened;

    [Header("Ui Stuff")] 
    public bool mouseControls = true;
    public GameObject uiPopup;

    [SerializeField] private Button initiallySelectedButton;
    [SerializeField] private Button choiceMenuNavigationButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private RectTransform[] buttonsWithAssociatedAnimators;
    [SerializeField] private Animator[] associatedAnimators;

    [Header("Loot Options Menu")] 
    [SerializeField] private GameObject lootOptionsMenu;

    [SerializeField] private GameObject[] lootOptionsWeaponsImages;
    [SerializeField] private GameObject[] lootOptionsChipsImages;
    
    [SerializeField] private Image[] lootOptionsImages;
    [SerializeField] private TMP_Text[] lootOptionsNames;
    [SerializeField] private TMP_Text[] lootOptionsDescriptions;
    
    [SerializeField] private Button[] lootOptionsButtonsWithAssociatedAnimators;
    [SerializeField] private Animator[] lootOptionsAssociatedAnimators;

    [Header("Current Gear Display")]
    [SerializeField] private GameObject weaponsChipsDisplay;
    [SerializeField] private GameObject mechChipsDisplay;

    [SerializeField] private GameObject choiceButtons;
    [SerializeField] private GameObject weaponsChoiceButtons;
    [SerializeField] private GameObject mechChoiceButton;
    
    [SerializeField] private Image[] currentMechChipImages;
    
    [Header("Current Weapons")]
    [SerializeField] private Image[] currentLeftWeaponImages;
    [SerializeField] private Image[] currentRightWeaponImages;
    [SerializeField] private TMP_Text currentLeftWeaponName;
    [SerializeField] private TMP_Text currentRightWeaponName;
    [SerializeField] private Image[] currentLeftWeaponChipImages;
    [SerializeField] private Image[] currentRightWeaponChipImages;


    [Header("Other References")] 
    public Animator[] boxAnimators;
    [SerializeField] private GameObject[] imageDisplayPoints;
    [SerializeField] private GameObject[] hologramSpawnPoints;
    public SpriteRenderer[] itemDisplayImagesOverBoxes;

    [SerializeField] private bool RigOnStart = false;

    [HideInInspector] public bool open;
    private float buttonInteractBlockTimer;
    private float closeUiTimer;
    public bool canUse = true;
    private int pickupIndex = 0;

    private InputAction backAction;

    private void Start()
    { 
        backAction = PlayerBody.Instance().GetComponent<PlayerInput>().actions["Back"];
        backAction.performed += BackButtonPressed;

        mouseControls = !PlayerBody.Instance().isGamepad;
        
        if(RigOnStart) RigLootBox();
    }

    private void OnDisable()
    {
        backAction.performed -= BackButtonPressed;
    }

    public void RigLootBox()
    {
        Debug.Log("Rigging Loot Box With " + playerPickups.Length + " Loot Choices");
        
        for (int i = 0; i < playerPickups.Length; i++)
        {
            itemDisplayImagesOverBoxes[i].sprite = playerPickups[i].mySprite;
            if(boxAnimators[i].gameObject.activeInHierarchy) boxAnimators[i].SetInteger("lootRarity", playerPickups[i].rarity);


            lootOptionsImages[i].sprite = playerPickups[i].mySprite;
            lootOptionsImages[i].color = ColourManager.instance.standardColours.LootRarityColours[playerPickups[i].rarity];
            lootOptionsWeaponsImages[i].GetComponent<Image>().sprite = playerPickups[i].mySprite;
            //lootOptionsWeaponsImages[i].GetComponent<Image>().color = FindObjectOfType<InventoryManager>(true).rarityColors[playerPickups[i].rarity];
            
            lootOptionsNames[i].text = playerPickups[i].itemName;
            lootOptionsDescriptions[i].text = playerPickups[i].description;

            switch (playerPickups[i].PickupType)
            {
                case pickupType.Weapon:
                    boxAnimators[i].SetBool("isWeapon", true);
                    //imageDisplayPoints[i].SetActive(false);
                    if (playerPickups[i].hologramReference != null) Instantiate(playerPickups[i].hologramReference, hologramSpawnPoints[i].transform);
                    
                    lootOptionsWeaponsImages[i].SetActive(true);
                    lootOptionsChipsImages[i].SetActive(false);
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
        
        Navigation nav = new Navigation();

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

            nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            
            nav.selectOnUp = exitButton;
            nav.selectOnDown = choiceMenuNavigationButton;
            nav.selectOnLeft = lootOptionsButtonsWithAssociatedAnimators[i].navigation.selectOnLeft;
            nav.selectOnRight = lootOptionsButtonsWithAssociatedAnimators[i].navigation.selectOnRight;
            
            lootOptionsButtonsWithAssociatedAnimators[i].navigation = nav;
        }
        
        nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = choiceMenuNavigationButton;
        nav.selectOnDown = choiceMenuNavigationButton;

        exitButton.navigation = nav;
        
        choiceMenuNavigationButton.Select();

        onChoiceMenuOpened.Invoke();
    }

    public void UnRigChoiceMenu()
    {
        choiceButtons.SetActive(false);

        for (int i = 0; i < lootOptionsButtonsWithAssociatedAnimators.Length; i++)
        {
            if(lootOptionsAssociatedAnimators[i].gameObject.activeInHierarchy) lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsSelected", false);
            if(lootOptionsAssociatedAnimators[i].gameObject.activeInHierarchy)lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsGreyedOut", false);
            if(lootOptionsAssociatedAnimators[i].gameObject.activeInHierarchy)lootOptionsAssociatedAnimators[i].GetComponent<Animator>().SetBool("IsReturn", false);
            
            Navigation nav = new Navigation();
            
            nav.mode = Navigation.Mode.Explicit;
            
            nav.selectOnUp = exitButton;
            nav.selectOnDown = exitButton;
            nav.selectOnLeft = lootOptionsButtonsWithAssociatedAnimators[i].navigation.selectOnLeft;
            nav.selectOnRight = lootOptionsButtonsWithAssociatedAnimators[i].navigation.selectOnRight;
            
            lootOptionsButtonsWithAssociatedAnimators[i].navigation = nav;
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
                    foreach (Button button in GetComponentsInChildren<Button>())
                    {
                        button.interactable = true;
                    }
                    
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
        if (!open)
        {
            UnRigChoiceMenu();
            
            pickupType pickupType = playerPickups[0].PickupType;
            
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
                    DisplayMechChips();
                    break;
            }

            PlayerBody.Instance().PauseSystem(PlayerSystems.AllParts, true);
            uiPopup.SetActive(true);

            GetComponentInChildren<Interactable>(true).canInteract = false;
            
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = lootOptionsButtonsWithAssociatedAnimators[0];
            nav.selectOnDown = lootOptionsButtonsWithAssociatedAnimators[0];

            exitButton.navigation = nav;

            initiallySelectedButton.Select();
            buttonInteractBlockTimer = 0.5f;
            
            foreach (Button button in GetComponentsInChildren<Button>())
            {
                button.interactable = false;
            }
            
            onMenuOpened.Invoke();
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
        PlayerBody.Instance().PauseSystem(PlayerSystems.AllParts, false);
        GetComponentInChildren<Interactable>(true).canInteract = true;
        
        UnRigChoiceMenu();
        
        if (uiPopup != null) uiPopup.SetActive(false);
        
        lootOptionsMenu.SetActive(true);
    }

    public void PickupItem(bool optionalDataApplyToLeft)
    {
        if (!canUse) return;
        
        pickupType pickupType = playerPickups[pickupIndex].PickupType;

        PlayerBody.Instance().PauseSystem(PlayerSystems.AllParts, false);
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

                PlayerBody.Instance().SetWeapon((WeaponPickup) playerPickups[pickupIndex], optionalDataApplyToLeft);

                playerPickups[pickupIndex] = newPickup;
                if (playerPickups[pickupIndex] != null)
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
                    .ApplyChip((WeaponChip) playerPickups[pickupIndex], optionalDataApplyToLeft);
                break;

            case pickupType.ChassisChip:
                PlayerBody.Instance().ApplyChip((BodyChip) playerPickups[pickupIndex]);
                break;

            case pickupType.OrdinanceChip:

                break;

            case pickupType.MovementChip:
                PlayerBody.Instance().GetComponent<ILegModifiable>().ApplyChip((MovementChip)playerPickups[pickupIndex]);
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
        open = false;

        onPickedUpEvent.Invoke();
    }

    private void DisplayWeaponChips()
    {
        if (PlayerBody.Instance().weaponHolder.leftWeapon != null)
        {
            for (int i = 0; i < currentLeftWeaponImages.Length; i++)
            {
                currentLeftWeaponImages[i].sprite = PlayerBody.Instance().weaponHolder.leftWInfo.mySprite;
            }

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

            currentLeftWeaponName.text = PlayerBody.Instance().weaponHolder.leftWInfo.itemName;
        }

        if (PlayerBody.Instance().weaponHolder.rightWeapon != null)
        {
            for (int i = 0; i < currentRightWeaponImages.Length; i++)
            {
                currentRightWeaponImages[i].sprite = PlayerBody.Instance().weaponHolder.rightWInfo.mySprite;
            }

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
            
            currentRightWeaponName.text = PlayerBody.Instance().weaponHolder.rightWInfo.itemName;
        }

        weaponsChipsDisplay.SetActive(true);
        weaponsChoiceButtons.SetActive(true);
        choiceMenuNavigationButton = weaponsChoiceButtons.GetComponentInChildren<Button>();
    }

    private void DisplayMechChips()
    {

        PlayerBody body = PlayerBody.Instance();
        PlayerLegs legs = body.myMovement;

        for (int i = 0; i < currentMechChipImages.Length; i++)
        {
            currentMechChipImages[i].enabled = false;
            
            if (body.myMods.Count > 0 && i < body.myMods.Count)
            {
                currentMechChipImages[i].sprite = PlayerBody.Instance().myMods[i].mySprite;
                currentMechChipImages[i].enabled = true;
            }
            else if (legs.legChips.Count > 0 && i - body.myMods.Count < legs.legChips.Count)
            {
                currentMechChipImages[i].sprite = legs.legChips[i - body.myMods.Count].mySprite;
                currentMechChipImages[i].enabled = true;
            }
        }

        mechChipsDisplay.SetActive(true);
        mechChoiceButton.SetActive(true);
        choiceMenuNavigationButton = mechChoiceButton.GetComponent<Button>();
    }

    public void SpawnLootBox()
    {
        for (int i = 0; i < boxAnimators.Length; i++)
        {
            boxAnimators[i].SetTrigger("spawnLoot");
        }

        GetComponentInChildren<Interactable>(true).gameObject.SetActive(true);
    }

    private void BackButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed && uiPopup.activeSelf)
        {
            ClosePickupMenu();
        }
    }

    public void TrySelectInitialButton()
    {
        if (uiPopup.activeSelf)
        {
            
        }
    }
}