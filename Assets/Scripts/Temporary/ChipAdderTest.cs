using DepreciaatedCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipAdderTest : MonoBehaviour
{

    public Chip[] chips;

    private void Start()
    {


        foreach (Chip chip in chips)
        {

            switch (chip.chipType)
            {

                case Chip.ChipTypes.Body:



                    break;
                case Chip.ChipTypes.Weapon:

                    PlayerBody.Instance().weaponHolder.ApplyChip((WeaponChip)chip, true);

                    break;
                case Chip.ChipTypes.Movement:


                    break;

            }
        }
    }

}
