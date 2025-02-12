using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : ScriptableObject
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
    public string chipName;
    public Sprite chipImage;
    public Material chipTexture;
    public ChipTypes chipType = ChipTypes.Body;
    [TextArea(3,5)]
    public string chipDes;


    public virtual void TriggerAbility()
    {

    }

}
