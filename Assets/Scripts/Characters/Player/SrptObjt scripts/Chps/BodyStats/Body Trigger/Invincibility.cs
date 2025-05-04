using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newRoomEndHeal", menuName = "Player/Chip/Body/Trigger/Invincibility")]
public class Invincibility : BodyTriggerChip
{

    public float invincibleTime = 0.5f;

    public override void TriggerAbility(PlayerBody myBody)
    {

        StartCoroutine(InvincibleTimer(myBody));

    }

    public IEnumerator InvincibleTimer(PlayerBody myBody)
    {

        Health myHealth = myBody.GetComponent<Health>();

        myHealth.canTakeDamage = false;

        yield return new WaitForSeconds(invincibleTime);

        myHealth.canTakeDamage = true;

        yield return null;

    }

}
