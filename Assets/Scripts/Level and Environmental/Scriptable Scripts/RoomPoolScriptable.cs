using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Room Pool", menuName = "ScriptableObjects/Level Scriptables/Room Pool")]
public class RoomPoolScriptable : ScriptableObject
{
    public GameObject[] entryRooms;
    public GameObject[] standardRooms;
    public GameObject[] rareRooms;
    public GameObject[] miniBossRooms;
    public GameObject[] bossRooms;
    public GameObject[] finalRooms;
}
