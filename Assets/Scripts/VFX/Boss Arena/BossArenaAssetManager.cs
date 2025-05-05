using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArenaAssetManager : MonoBehaviour
{
    public Animator phaseOneArena;
    public Animator fallingAnim;

    public GameObject cameraPivot;
    public RoomUpperLevelToggle roomUpperLevelToggle;

    public bool test;
    void Update()
    {
        if (test)
        {
            StartTransition();
            test = false;
        }
    }

    public void StartTransition()
    {
        if(roomUpperLevelToggle.roomAngle == 45)
        {
            cameraPivot.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else 
        {
            cameraPivot.transform.localRotation = Quaternion.Euler(0, -90, 0);
        }


        phaseOneArena.SetTrigger("Transition");
        fallingAnim.SetTrigger("Transition");
    }

}
