using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteThisAfterTime : MonoBehaviour
{
    public float timeToDestroy;
    private void OnEnable()
    {
        Destroy(gameObject, timeToDestroy);
    }


}
