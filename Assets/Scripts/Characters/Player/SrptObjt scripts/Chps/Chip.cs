using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : PlayerPickup
{

    public enum ChipTypes
    {
        Body, Weapon,
        Movement
    }
    public enum ChipRarity
    {
        Common, Uncommon,
        Rare, UltraRare
    }
    [Header("Chip Type")]
    public ChipTypes chipType = ChipTypes.Body;


}
