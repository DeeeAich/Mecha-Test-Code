using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

[CreateAssetMenu(fileName = "newDashTrigger", menuName = "Player/Chip/Dash/Trigger")]
public class DashTriggerChip : MovementTriggerChip
{

    public DashStatChange statChange;
    [Tooltip("0 = 100%")]
    public float chance;
    public float timer;

    public override void Trigger(PlayerLegs playerLegs)
    {
        
        if (chance == 0 || chance < Random.Range(0.0f, 100.0f))
            StartCoroutine(TriggerTimed(playerLegs));

    }

    public IEnumerator TriggerTimed(PlayerLegs legs)
    {

        legs.ApplyDashStats(statChange);

        yield return new WaitForSeconds(timer);

        legs.RemoveDashStats(statChange);

        yield return null;
    }

}
