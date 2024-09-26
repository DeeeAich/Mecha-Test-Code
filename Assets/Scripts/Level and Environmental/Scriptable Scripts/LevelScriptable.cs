using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Info", menuName = "ScriptableObjects/Level Scriptables/Level Info")]
public class LevelScriptable : ScriptableObject
{
    public string levelName;

    public RoomPoolScriptable roomPool;
    public EnemyPoolScriptable enemyPool;
}
