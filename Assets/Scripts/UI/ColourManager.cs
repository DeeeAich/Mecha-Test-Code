using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColourManager : MonoBehaviour
{
    public StandardisedColourScriptable standardColours;

    public static ColourManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    /*
  

    public static ColourManager Instance()
    {
        if (instance == null)
        {
            instance = Instantiate(new GameObject()).AddComponent<ColourManager>();
        }

        return instance;
    }
    */
}
