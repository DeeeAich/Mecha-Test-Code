using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBodyModifiable
{

    public void ApplyChip(BodyChip newChip);

}

public interface ILegModifiable
{
    public void ApplyChip(MovementChip newChip);
}

public interface IWeaponModifiable
{

    public void ApplyChip(WeaponChip newChip);

    public void ApplyMods(WeaStaEftChip applyChip);

}