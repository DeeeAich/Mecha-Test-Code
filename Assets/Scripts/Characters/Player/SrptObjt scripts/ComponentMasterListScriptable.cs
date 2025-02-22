using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Components Master List", menuName = "ScriptableObjects/Components Master List")]
public class ComponentMasterListScriptable : ScriptableObject
{
    public PlayerPickup[] chassis;
    public PlayerPickup[] weapons;
    public PlayerPickup[] ultimates;
    public PlayerPickup[] chips;
}
