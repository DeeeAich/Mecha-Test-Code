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

    public void ApplyChip(WeaponChip newChip, bool left);

    public void ApplyMods(WeaStaEftChip applyChip, Weapon target);

}

public interface IModable
{

    public void AddMod(StatusInfo mod);

}