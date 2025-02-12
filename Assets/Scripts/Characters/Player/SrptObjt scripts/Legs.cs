using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Leg data", menuName = "Player/Leg Data")]
public class Legs : ScriptableObject
{
    public LegStatList myStats;

    [Header("Attachable")]
    public GameObject legPrefab;
    public enum Scripts
    {
        PlayerLegs,
        Treads,
        Hover
    }

}

[Serializable]
public class LegStatList
{
    [Header("Basic Info")]
    public float speed;
    public float accelleration;
    public float turnSpeed;

    [Header("Dash Info")]
    public float dashTime;
    public float dashDistance;
    public int dashCharges;
    public float dashRecharge;

    public LegStatList SetStats()
    {

        LegStatList newStatList = new();

        newStatList.speed = speed;
        newStatList.accelleration = accelleration;
        newStatList.turnSpeed = turnSpeed;
        newStatList.dashTime = turnSpeed;
        newStatList.dashDistance = dashDistance;
        newStatList.dashRecharge = dashRecharge;
        newStatList.dashCharges = dashCharges;

        return newStatList;
    }

    public void ModifyLegs(LegStatList statList, LegStatChange legChange)
    {


    }

    public void ModifyDash(LegStatList statList, DashStatChange dashChange)
    {


    }

}
