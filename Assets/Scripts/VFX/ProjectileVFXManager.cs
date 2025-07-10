using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVFXManager : MonoBehaviour
{
    public bool isBurn;
    public bool isHack;
    public bool isShock;
    public bool isCrit;
    public bool isLaser;

    public GameObject defaultAsset;
    public GameObject burnAsset;
    public GameObject hackAsset;
    public GameObject shockAsset;
    public GameObject critAsset;
    public GameObject laserAsset;


    private void OnEnable()
    {
        if (isBurn) { burnAsset.SetActive(true); } else { burnAsset.SetActive(false); }
        if (isHack) { hackAsset.SetActive(true); } else { hackAsset.SetActive(false); }
        if (isShock) { shockAsset.SetActive(true); } else { shockAsset.SetActive(false); }
        if (isCrit) { critAsset.SetActive(true); } else { critAsset.SetActive(false); }
        if (isLaser) { laserAsset.SetActive(true); defaultAsset.SetActive(false); } else { laserAsset.SetActive(false); defaultAsset.SetActive(true); }
    }
    private void OnDisable()
    {
        isBurn = false;
        isHack = false;
        isShock = false;
        isCrit = false;
        isLaser = false;
    }

}
