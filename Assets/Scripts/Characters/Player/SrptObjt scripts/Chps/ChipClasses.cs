using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ChipClasses
{

    [Serializable]
    public class StatusInfo
    {
        public WeaStaEftChip.StatusType statusType;
        [Header("If Applicable to the Status")]
        public float effectChance;
        public float effectDamage;
        public float effectTime;

    }


}

