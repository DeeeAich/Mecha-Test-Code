using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private bool editorUpdateInventory;

    public List<Image> bodyChipsImages;
    public List<Image> WeaponsLeftChipsImages;
    public List<Image> WeaponsRightChipsImages;
    public Sprite weaponChipEmptySprite;

    public Button[] bodyChipsButtons;
    public Button[] weaponLeftChipsButtons;
    public Button[] weaponRightChipsButtons;

    [Header("Inspection Card")] 
    [SerializeField] private GameObject inspectionCard;
    [SerializeField] private TMP_Text inspectionCardTitle;
    [SerializeField] private TMP_Text inspectionCardDescription;
    [SerializeField] private Image inspectionCardImage;
    
    private void Awake()
    {
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
    }

    public void UpdateInventory()
    {
        print("Updating Inventory");
        
        inspectionCard.SetActive(false);
        
        List<WeaponChip> leftMods = PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().leftMods;
        List<WeaponChip> rightMods = PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().rightMods;

        for (int i = 0; i < WeaponsLeftChipsImages.Count; i++)
        {
            if (i < leftMods.Count)
            {
                WeaponsLeftChipsImages[i].sprite = leftMods[i].chipImage;
                WeaponsLeftChipsImages[i].enabled = true;
            }
            else
            {
                WeaponsLeftChipsImages[i].enabled = false;
            }

        }

        for (int i = 0; i < WeaponsRightChipsImages.Count; i++)
        {
            if (i < rightMods.Count)
            {
                WeaponsRightChipsImages[i].sprite = rightMods[i].chipImage;
                WeaponsRightChipsImages[i].enabled = true;
            }
            else
            {
                WeaponsRightChipsImages[i].enabled = false;
            }
        }

        List<Chip> bodyChips = PlayerBody.PlayBody().chipsInserted;

        for (int i = 0; i < bodyChipsImages.Count; i++)
        {
            if (i < bodyChips.Count)
            {
                bodyChipsImages[i].sprite = bodyChips[i].chipImage;
                bodyChipsImages[i].enabled = true;
            }
            else
            {
                bodyChipsImages[i].enabled = false;
            }
        }
    }

    public void OpenInspectionCard(int section, int index)
    {
        inspectionCard.SetActive(false);
        Chip chip = null;
        
        switch (section)
        {
            case 0: // left weapon
                if (index >= PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().leftMods.Count)
                {
                    return;
                }
                
                inspectionCard.transform.position = WeaponsLeftChipsImages[index].transform.position;
                chip = PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().leftMods[index];
                break;
            
            case 1: // right weapon
                if (index >= PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().rightMods.Count)
                {
                    return;
                }
                
                inspectionCard.transform.position = WeaponsRightChipsImages[index].transform.position;
                chip = PlayerBody.PlayBody().GetComponent<PlayerWeaponControl>().rightMods[index];
                break;
            
            case 2: // body chips
                if (index >= PlayerBody.PlayBody().chipsInserted.Count)
                {
                    return;
                }
                
                inspectionCard.transform.position = bodyChipsImages[index].transform.position;
                chip = PlayerBody.PlayBody().chipsInserted[index];
                break;
        }

        if (chip != null)
        {
            inspectionCardImage.sprite = chip.chipImage;
            inspectionCardDescription.text = chip.chipDes;
            inspectionCardTitle.text = chip.chipName;
        }

        inspectionCard.SetActive(true);
    }

    public void CloseInspectionCard()
    {
        
    }
}
