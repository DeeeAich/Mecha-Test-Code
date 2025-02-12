using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "newEndOfRoomChip", menuName = "Player/Chip/EndRoomChip")]
public class BEndChip : BodyChip
{

    public override void TriggerAbility()
    {

        Debug.Log("I have activated myself");

    }

}