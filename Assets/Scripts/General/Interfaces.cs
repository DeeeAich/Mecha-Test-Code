using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHackable
{
    public void Hack(float percentage, float chance, float duration);
    public IEnumerator HackDecay();
}

public interface IShortCircuitable
{
    public void ShortCircuit(float chance, float time);
}

public interface IBurnable
{

}
