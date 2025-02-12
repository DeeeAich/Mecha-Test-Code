using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    PlayerBody myBody;
    public Legs myLegs;
    public LegStats curLegStats;
    //place leg chips here
    //place percentage changes here

    private void Start()
    {
        myBody = GetComponent<PlayerBody>();
    }
    public void LoadStats()
    {

        //set curstats to the myLegs

    }

    public void SetStats()
    {

        //.myUI.LockAndLoad(myHealth.maxHealth, myHealth.health,
        //    weaponHolder.leftWeapon.curAmmo, weaponHolder.rightWeapon.curAmmo,
        //    legStats.dashRecharge, legStats.dashCharges);

    }

}
