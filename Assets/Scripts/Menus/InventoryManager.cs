using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        FindObjectOfType<pauseMenu>().onPause.AddListener(UpdateInventory);
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

        List<BodyChip> bodyChips = new List<BodyChip>();
        // = PlayerBody.PlayBody().myMods;

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
}
