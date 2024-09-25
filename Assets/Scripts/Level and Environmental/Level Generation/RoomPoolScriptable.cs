using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room Pool Scriptable", menuName = "Room Pool")]
public class RoomPoolScriptable : ScriptableObject
{
    public GameObject[] standardRooms;
    public GameObject[] rareRooms;
    public GameObject[] miniBossRooms;
    public GameObject[] bossRooms;
    public GameObject[] finalRooms;
}
