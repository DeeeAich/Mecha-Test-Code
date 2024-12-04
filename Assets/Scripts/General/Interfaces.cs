using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHackable
{
    public void Hack(float percentage, float chance, float duration);
    //apply multi if overchance
}

public interface IShortCircuitable
{
    public void ShortCircuit(float chance, float time);
    //multi time 
}

public interface IBurnable
{
    public void Burn(float chance, float damageTick, int tickCount);
    //apply multi burn if overchance
}
