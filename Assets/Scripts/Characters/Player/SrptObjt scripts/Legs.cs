using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Leg data", menuName = "Player/Leg Data")]
public class Legs : PlayerPickup
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
    public bool invincibleDuringDash;

    [Header("Attachable")]
    public GameObject legPrefab;
    public enum Scripts
    {
        PlayerLegs,
        Treads,
        Hover
    }
    public Scripts myScript;

    public PlayerBody.LegInfo LoadLegs()
    {
        PlayerBody.LegInfo legsToFill = new PlayerBody.LegInfo();

        legsToFill.speed = speed;
        legsToFill.accelleration = accelleration;
        legsToFill.dashDistance = dashDistance;
        legsToFill.dashTime = dashTime;
        legsToFill.dashCharges = dashCharges;
        legsToFill.dashRecharge = dashRecharge;

        return legsToFill;
    }
}
