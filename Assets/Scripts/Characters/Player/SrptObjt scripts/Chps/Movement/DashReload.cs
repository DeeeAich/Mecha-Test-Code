using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDashTrigger", menuName = "Player/Chip/Dash/Trigger/DashReload")]
public class DashReload : MovementTriggerChip
{

    public int ammoUpCount;

    public override void Trigger(PlayerLegs playerLegs)
    {

        PlayerWeaponControl.instance.leftWeapon.ExternalReload(ammoUpCount);
        PlayerWeaponControl.instance.rightWeapon.ExternalReload(ammoUpCount);

    }


}
