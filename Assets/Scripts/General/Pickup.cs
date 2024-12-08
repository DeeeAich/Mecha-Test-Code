using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public pickupType pickupType = pickupType.ChassisChip;
    public GameObject itemReference;
    public ScriptableObject ItemScriptableReference;

    public GameObject uiPopup;
    public Sprite itemDisplayImage;

    public void TryPickup()
    {
        switch (pickupType)
        {
            case pickupType.Weapon:
                PlayerBody.PlayBody().StopParts(false,false);
                uiPopup.SetActive(true);
                break;
            
            case pickupType.Chassis:
                PlayerBody.PlayBody().StopParts(false,false);
                uiPopup.SetActive(true);
                break;
            
            case pickupType.Ordinance :
                PlayerBody.PlayBody().StopParts(false,false);
                uiPopup.SetActive(true);
                break;
            
            
            case pickupType.WeaponChip:
                OnPickup(0);
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
                
                if (optionalData == 0)
                {
                    FindObjectOfType<PlayerBody>().SetWeapon(itemReference, true);
                }
                
                if (optionalData == 1)
                {
                    FindObjectOfType<PlayerBody>().SetWeapon(itemReference, false);
                }
                
                break;
            
            case pickupType.Chassis:
                
                break;
            
            case pickupType.Ordinance :
                
                break;
            
            
            case pickupType.WeaponChip:
                
                break;
            
            case pickupType.ChassisChip:
                
                break;
            
            case pickupType.OrdinanceChip:
                
                break;
            
            case pickupType.MovementChip:
                
                break;
        }
        
        Destroy(gameObject);
    }
}
