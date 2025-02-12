using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    PlayerBody myBody;
    Health myHealth;
    public Legs myLegs;
    public LegStatList curLegStats;
    //place leg chips here
    //place percentage changes here
    PlayerWeaponControl myWeapons;

    private void Start()
    {
        myBody = GetComponent<PlayerBody>();
        myWeapons = GetComponent<PlayerWeaponControl>();
        myBody.myUI = FindObjectOfType<PlayerUI>();
        LoadStats();
    }
    public void LoadStats()
    {

        //set curstats to the myLegs
        myLegs = myBody.legStats;
        curLegStats = myLegs.myStats.SetStats();
        SetStats();

    }

    public void SetStats()
    {

        myBody.myUI.LockAndLoad(myHealth.maxHealth, myHealth.health,
            myWeapons.leftWeapon.curAmmo, myWeapons.rightWeapon.curAmmo,
            curLegStats.dashRecharge, curLegStats.dashCharges);

    }

}
