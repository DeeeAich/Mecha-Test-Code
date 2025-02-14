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
    [SerializeField] private Button initiallySelectedButton;
    
    public Sprite itemDisplaySprite;
    public SpriteRenderer itemDisplayImage;
    public TMP_Text pickupName;
    public TMP_Text pickupDescription;
    
    public GameObject itemReference;
    public ScriptableObject ItemScriptableReference;



    public Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetInteger("lootRarity", pickupRarity);
        if(itemDisplayImage != null && itemDisplaySprite != null) itemDisplayImage.sprite = itemDisplaySprite;
    }

    public void TryPickup()
    {
        switch (pickupType)
        {
            case pickupType.Weapon:
                PlayerBody.PlayBody().StopParts(false,false);
                uiPopup.SetActive(true);
                initiallySelectedButton.Select();
                break;
            
            case pickupType.Chassis:
                PlayerBody.PlayBody().StopParts(false,false);
                uiPopup.SetActive(true);
                initiallySelectedButton.Select();
                break;
            
            case pickupType.Ordinance :
                PlayerBody.PlayBody().StopParts(false,false);
                uiPopup.SetActive(true);
                initiallySelectedButton.Select();
                break;
            
            case pickupType.WeaponChip:
                PlayerBody.PlayBody().StopParts(false,false);
                uiPopup.SetActive(true);
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
                pickup.GetComponentInChildren<Interactable>().enabled = false;
            }
        }
        
        if(uiPopup != null) uiPopup.SetActive(false);

        GetComponentInChildren<Interactable>().enabled = false;
    }

    public void CancelPickup()
    {
        PlayerBody.PlayBody().StopParts(true,true);
        if(uiPopup != null) uiPopup.SetActive(false);
    }
}
