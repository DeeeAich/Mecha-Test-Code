using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class DashTriggerChip : MovementTriggerChip
{

    public DashStatChange statChange;
    [Tooltip("0 = 100%")]
    public float chance;
    public float timer;

    public override void Trigger(PlayerLegs playerLegs)
    {
        
        

    }

    public IEnumerator TriggerTimed(PlayerLegs legs)
    {

        legs.ApplyDashStats(statChange);

        yield return new WaitForSeconds(timer);



        yield return null;
    }

}
