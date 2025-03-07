using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    
    public static PlayerManager instance;

    public List<BodyChip> bodyMods;

    public WeaponPickup leftWeapon;
    public List<WeaponChip> leftWeaponChips;
    public WeaponPickup rightWeapon;
    public List<WeaponChip> rightWeaponChips;

    private PlayerBody playerBody;


    private void Start()
    {
        
        if (instance == null)
            instance = this;
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        playerBody = PlayerBody.Instance();

        bodyMods = playerBody.myMods;

        leftWeapon = playerBody.weaponHolder.leftWInfo;
        leftWeaponChips = playerBody.weaponHolder.leftMods;
        rightWeapon = playerBody.weaponHolder.rightWInfo;
        rightWeaponChips = playerBody.weaponHolder.rightMods;

    }

    public void SetStats()
    {

        playerBody = PlayerBody.Instance();

        playerBody.myMods = bodyMods;

        playerBody.weaponHolder.leftMods = leftWeaponChips;
        playerBody.weaponHolder.rightMods = rightWeaponChips;

        playerBody.SetWeapon(leftWeapon, true);
        playerBody.SetWeapon(rightWeapon, false);

    }

}
