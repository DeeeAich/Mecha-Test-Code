using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Components Master List", menuName = "ScriptableObjects/Components Master List")]
public class ComponentMasterListScriptable : ScriptableObject
{
    public GameObject[] chassis;
    public GameObject[] weapons;
    public GameObject[] ultimates;
}
