using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private bool editorUpdateInventory;

    private Color[] rarityColors;

    public List<Image> bodyChipsImages;
    public Button[] bodyChipsButtons;

    [SerializeField] private TMP_Text chassisTitle;
    [SerializeField] private TMP_Text chassisDescription;
    [SerializeField] private Image chassisImage;

    [SerializeField] private TMP_Text ordinanceTitle;
    [SerializeField] private TMP_Text ordinanceDescription;
    [SerializeField] private Image ordinanceImage;

    [Header("Weapon Slots")] 
    [SerializeField] private TMP_Text leftWeaponTitle;
    [SerializeField] private TMP_Text rightWeaponTitle;
    
    [SerializeField] private TMP_Text leftWeaponDescription;
    [SerializeField] private TMP_Text rightWeaponDescription;
    
    [SerializeField] private Image leftWeaponImage;
    [SerializeField] private Image rightWeaponImage;

    [SerializeField] private TMP_Text leftWeaponAmmo;
    [SerializeField] private TMP_Text rightWeaponAmmo;
    
    [SerializeField] private List<Image> WeaponsLeftChipsImages;
    [SerializeField] private List<Image> WeaponsRightChipsImages;

    [SerializeField] private Button[] weaponLeftChipsButtons;
    [SerializeField] private Button[] weaponRightChipsButtons;

    [Header("Inspection Card")] 
    [SerializeField] private GameObject inspectionCard;
    [SerializeField] private TMP_Text inspectionCardTitle;
    [SerializeField] private TMP_Text inspectionCardDescription;
    [SerializeField] private Image inspectionCardImage;
    [SerializeField] private Button inspectionCardTargetButton;

    private void Awake()
    {
        rarityColors = ColourManager.instance.standardColours.LootRarityColours;
        
        FindObjectOfType<pauseMenu>().onPause.AddListener(delegate { UpdateInventory(); });

        for (int i = 0; i < weaponLeftChipsButtons.Length; i++)
        {
            int index = i;
            weaponLeftChipsButtons[i].onClick.AddListener(delegate { OpenInspectionCard(0, index); });
        }

        for (int i = 0; i < weaponRightChipsButtons.Length; i++)
        {
            int index = i;
            weaponRightChipsButtons[i].onClick.AddListener(delegate { OpenInspectionCard(1, index); });
        }

        for (int i = 0; i < bodyChipsButtons.Length; i++)
        {
            int index = i;
            bodyChipsButtons[i].onClick.AddListener(delegate { OpenInspectionCard(2, index); });
        }
    }

    private void Update()
    {
        if (editorUpdateInventory)
        {
            editorUpdateInventory = false;
            UpdateInventory();
        }

        if (inspectionCard.activeSelf && EventSystem.current.currentSelectedGameObject != inspectionCardTargetButton.gameObject)
        {
            CloseInspectionCard();
        }
    }

    public void UpdateInventory()
    {
        print("Updating Inventory");
        inspectionCard.SetActive(false);



        PlayerBody playerBody = PlayerBody.Instance();

        chassisTitle.text = playerBody.legStats.itemName;
        chassisDescription.text = playerBody.legStats.description;
        chassisImage.sprite = playerBody.legStats.mySprite;
        chassisImage.color = rarityColors[playerBody.legStats.rarity];

        if (playerBody.ultController != null && playerBody.ultController.currentUltimate != null)
        {
            ordinanceTitle.text = playerBody.ultController.currentUltimate.itemName;
            ordinanceDescription.text = playerBody.ultController.currentUltimate.description;
            ordinanceImage.sprite = playerBody.ultController.currentUltimate.mySprite;
            ordinanceImage.color = rarityColors[playerBody.ultController.currentUltimate.rarity];
        }

        List<BodyChip> bodyChips = playerBody.myMods;
        List<MovementChip> legChips = playerBody.myMovement.legChips;
        
        for (int i = 0; i < bodyChipsImages.Count; i++)
        {
            if (i < bodyChips.Count)
            {
                bodyChipsImages[i].sprite = bodyChips[i].mySprite;
                bodyChipsImages[i].color = rarityColors[bodyChips[i].rarity];
                bodyChipsImages[i].enabled = true;
            }
            else if (i < bodyChips.Count + legChips.Count)
            {
                bodyChipsImages[i - bodyChips.Count].sprite = legChips[i - bodyChips.Count].mySprite;
                bodyChipsImages[i].color = rarityColors[legChips[i - bodyChips.Count].rarity];
                bodyChipsImages[i].enabled = true;
            }
            else
            {
                bodyChipsImages[i].enabled = false;
            }
        }

        PlayerWeaponControl weapons = PlayerBody.Instance().weaponHolder;

        if (weapons.leftWeapon != null)
        {
            leftWeaponTitle.text = weapons.leftWInfo.itemName;
            leftWeaponTitle.gameObject.SetActive(true);
            leftWeaponDescription.text = weapons.leftWInfo.description;
            leftWeaponDescription.gameObject.SetActive(true);
            leftWeaponImage.sprite = weapons.leftWInfo.mySprite;
            leftWeaponAmmo.text = weapons.leftWeapon.curAmmo.ToString();
            leftWeaponImage.gameObject.SetActive(true);
            
            List<WeaponChip> leftMods = weapons.leftMods;
            for (int i = 0; i < WeaponsLeftChipsImages.Count; i++)
            {
                if (i < leftMods.Count)
                {
                    WeaponsLeftChipsImages[i].sprite = leftMods[i].mySprite;
                    WeaponsLeftChipsImages[i].color = rarityColors[leftMods[i].rarity];
                    WeaponsLeftChipsImages[i].enabled = true;
                }
                else
                {
                    WeaponsLeftChipsImages[i].enabled = false;
                }
            }
        }
        else
        {
            leftWeaponTitle.gameObject.SetActive(false);
            leftWeaponDescription.gameObject.SetActive(false);
            leftWeaponImage.gameObject.SetActive(false);
        }

        if (weapons.rightWeapon != null)
        {
            rightWeaponTitle.text = weapons.rightWInfo.itemName;
            rightWeaponTitle.gameObject.SetActive(true);
            rightWeaponDescription.text = weapons.rightWInfo.description;
            rightWeaponDescription.gameObject.SetActive(true);
            rightWeaponImage.sprite = weapons.rightWInfo.mySprite;
            rightWeaponAmmo.text = weapons.rightWeapon.curAmmo.ToString();
            rightWeaponTitle.gameObject.SetActive(true);
            
            List<WeaponChip> rightMods = weapons.rightMods;
            for (int i = 0; i < WeaponsRightChipsImages.Count; i++)
            {
                if (i < rightMods.Count)
                {
                    WeaponsRightChipsImages[i].sprite = rightMods[i].mySprite;
                    WeaponsRightChipsImages[i].color = rarityColors[rightMods[i].rarity];
                    WeaponsRightChipsImages[i].enabled = true;
                }
                else
                {
                    WeaponsRightChipsImages[i].enabled = false;
                }
            }
        }
        else
        {
            rightWeaponDescription.gameObject.SetActive(false);
            rightWeaponTitle.gameObject.SetActive(false);
            rightWeaponTitle.gameObject.SetActive(false);
        }
    }

    public void OpenInspectionCard(int section, int index)
    {
        inspectionCard.SetActive(false);
        Chip chip = null;
        
        switch (section)
        {
            case 0: // left weapon
                if (index >= PlayerBody.Instance().GetComponent<PlayerWeaponControl>().leftMods.Count)
                {
                    return;
                }
                
                inspectionCard.transform.position = WeaponsLeftChipsImages[index].transform.position;
                inspectionCardTargetButton = weaponLeftChipsButtons[index];
                chip = PlayerBody.Instance().GetComponent<PlayerWeaponControl>().leftMods[index];
                break;
            
            case 1: // right weapon
                if (index >= PlayerBody.Instance().GetComponent<PlayerWeaponControl>().rightMods.Count)
                {
                    return;
                }
                
                inspectionCard.transform.position = WeaponsRightChipsImages[index].transform.position;
                inspectionCardTargetButton = weaponRightChipsButtons[index];
                chip = PlayerBody.Instance().GetComponent<PlayerWeaponControl>().rightMods[index];
                break;
            
            case 2: // body chips
                if (index >= PlayerBody.Instance().myMods.Count)
                {
                    return;
                }
                
                inspectionCard.transform.position = bodyChipsImages[index].transform.position;
                inspectionCardTargetButton = bodyChipsButtons[index];
                chip = PlayerBody.Instance().myMods[index];
                break;
        }

        if (chip != null)
        {
            inspectionCardImage.sprite = chip.mySprite;
            inspectionCardImage.color = rarityColors[chip.rarity];
            inspectionCardDescription.text = chip.description;
            inspectionCardTitle.text = chip.itemName;
        }

        inspectionCard.SetActive(true);
    }

    public void CloseInspectionCard()
    {
        inspectionCard.SetActive(false);
        inspectionCardTargetButton = null;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < weaponLeftChipsButtons.Length; i++)
        {
            weaponLeftChipsButtons[i].onClick.RemoveAllListeners();
        }

        for (int i = 0; i < weaponRightChipsButtons.Length; i++)
        {
            weaponRightChipsButtons[i].onClick.RemoveAllListeners();
        }

        for (int i = 0; i < bodyChipsButtons.Length; i++)
        {
            bodyChipsButtons[i].onClick.RemoveAllListeners();
        }
    }
}
