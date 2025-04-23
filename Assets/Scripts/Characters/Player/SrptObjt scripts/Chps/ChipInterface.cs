using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBodyModifiable
{

    public void ApplyChip(BodyChip newChip);

    public void ApplyStats(BodyStats newStats);

    public void RemoveStats(BodyStats newStats);

}

public interface ILegModifiable
{
    public void ApplyChip(MovementChip newChip);

    public void ApplyLegStats(LegStatChange chipChange);

    public void RemoveLegStats(LegStatChange chipChange);

    public void ApplyDashStats(DashStatChange chipChange);

    public void RemoveDashStats(DashStatChange chipChange);

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

public interface IMod
{

    public void AddModifiers(StatusInfo statusInfo, bool percentage = false);

}