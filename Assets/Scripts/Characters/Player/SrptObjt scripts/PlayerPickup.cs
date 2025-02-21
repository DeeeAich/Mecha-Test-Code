using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : ScriptableObject
{

    public string itemName;
    [TextArea(3, 5)]
    public string description;
    public GameObject pickup;
    public Sprite mySprite;
    public Sprite myMenuIcon;

    protected void StartCoroutine(IEnumerator corout)
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Can not run coroutine outside of play mode.");
            return;
        }

        PlayerBody.PlayBody().Preform(corout);
    }

}
