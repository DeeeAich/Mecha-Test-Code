using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Pool", menuName = "ScriptableObjects/Level Scriptables/Enemy Pool")]
public class EnemyPoolScriptable : ScriptableObject
{
    public GameObject[] standardEnemies;
    public GameObject[] rareEnemies;
    public GameObject[] miniBosses;
    public GameObject[] bosses;
}
