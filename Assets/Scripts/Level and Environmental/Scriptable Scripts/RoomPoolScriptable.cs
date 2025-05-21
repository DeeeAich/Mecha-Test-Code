using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Room Pool", menuName = "ScriptableObjects/Level Scriptables/Room Pool")]
public class RoomPoolScriptable : ScriptableObject
{
    public GameObject[] entryRooms;
    public GameObject[] standardRooms;
    public GameObject[] miniBossRooms;
    public GameObject[] preBossRooms;
    public GameObject[] bossRooms;
    public GameObject[] postBossRooms;
}
