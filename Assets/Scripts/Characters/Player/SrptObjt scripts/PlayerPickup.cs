using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : ScriptableObject
{
    [Header("Pickup Info")]
    public string itemName;
    public pickupType PickupType;
    public int rarity;
    public int spawnRate;
    [TextArea(3, 5)]
    public string description;
    public Sprite mySprite;
    
    [Header("Optional GameObject Reference")]
    public GameObject objectReference;

    protected void StartCoroutine(IEnumerator corout)
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Can not run coroutine outside of play mode.");
            return;
        }

        PlayerBody.Instance().Preform(corout);
    }

}
