using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUpperLevelToggle : MonoBehaviour
{
    public GameObject localNorth;
    public GameObject localEast;
    public GameObject localSouth;
    public GameObject localWest;

    public GameObject worldDirection;
    public float roomAngle;

    void Start()
    {
        localNorth.SetActive(false);
        localEast.SetActive(false);
        localSouth.SetActive(false);
        localWest.SetActive(false);

        worldDirection.transform.rotation = Quaternion.Euler(transform.forward);


        roomAngle = Mathf.RoundToInt(worldDirection.transform.localEulerAngles.y);

        switch (roomAngle)
        {
            case 45:
                localNorth.SetActive(true);
                localEast.SetActive(true);
                break;

            case 315:
                localNorth.SetActive(true);
                localWest.SetActive(true);
                break;

            case 225:
                localSouth.SetActive(true);
                localWest.SetActive(true);
                break;

            case 135:
                localSouth.SetActive(true);
                localEast.SetActive(true);
                break;

        }


    }

}
