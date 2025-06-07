using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorOutdoorTransition : MonoBehaviour
{
    public List<GameObject> outdoorObjects;
    public List<GameObject> indoorObjects;

    

    public void TransitionOutdoors()
    {
        for (int i = 0; i < outdoorObjects.Count; i++)
        {
            outdoorObjects[i].SetActive(true);
        }
        for (int i = 0; i < outdoorObjects.Count; i++)
        {
            indoorObjects[i].SetActive(false);
        }
    }
    public void TransitionIndoors()
    {
        for (int i = 0; i < outdoorObjects.Count; i++)
        {
            outdoorObjects[i].SetActive(false);
        }
        for (int i = 0; i < outdoorObjects.Count; i++)
        {
            indoorObjects[i].SetActive(true);
        }
    }
}
