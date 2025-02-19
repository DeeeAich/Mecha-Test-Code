using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : ScriptableObject
{


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
