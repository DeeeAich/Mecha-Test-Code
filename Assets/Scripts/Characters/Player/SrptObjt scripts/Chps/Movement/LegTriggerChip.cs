using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLegTrigger", menuName = "Player/Chip/Leg/Trigger")]
public class LegTriggerChip : MovementTriggerChip
{

    public LegStatChange statChange;
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

        legs.ApplyLegStats(statChange);

        yield return new WaitForSeconds(timer);

        legs.RemoveLegStats(statChange);

        yield return null;
    }

}
