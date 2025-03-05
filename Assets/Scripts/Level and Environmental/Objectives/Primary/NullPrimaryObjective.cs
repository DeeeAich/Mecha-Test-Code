using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullPrimaryObjective : Objective
{
    
    void Start()
    {
        room.onStartRoom.AddListener(TriggerComplete);
    }
}
