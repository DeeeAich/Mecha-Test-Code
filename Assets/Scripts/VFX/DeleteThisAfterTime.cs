using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteThisAfterTime : MonoBehaviour
{
    public float timeToDestroy;
    public bool unParentOnEnable;
    private void OnEnable()
    {
        Destroy(gameObject, timeToDestroy);
        if (unParentOnEnable)
        {
            transform.parent = null;
        }
    }


}
