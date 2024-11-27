using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBodyModifiable
{

    void ApplyChip(Chip newChip);

}

public interface ILegModifiable
{
    void ApplyChip(Chip newChip);
}

public interface IWeaponModifiable
{

    void ApplyChip(Chip newChip);

}