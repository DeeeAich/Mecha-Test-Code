using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newRoomEndHeal", menuName = "Player/Chip/Body/Trigger/Invincibility")]
public class Invincibility : BodyTriggerChip
{

    public float invincibleTime = 0.5f;
    public float coolDown = 10f;
    private bool invincibleActive = false;

    public override void TriggerAbility(PlayerBody myBody)
    {
        if(!invincibleActive)
            StartCoroutine(InvincibleTimer(myBody));

    }

    public IEnumerator InvincibleTimer(PlayerBody myBody)
    {

        invincibleActive = true;

        Health myHealth = myBody.GetComponent<Health>();

        myHealth.canTakeDamage = false;

        Debug.Log("I'm fuckin' invincible");

        yield return new WaitForSeconds(invincibleTime);

        Debug.Log("I'm no longer fuckin' invincible");

        myHealth.canTakeDamage = true;

        yield return new WaitForSeconds(coolDown);

        invincibleActive = false;

        yield return null;

    }
    public override void ChipTriggerSetter()
    {

        invincibleActive = false;

        base.ChipTriggerSetter();

    }
    public override void ChipTriggerUnsetter()
    {

        invincibleActive = false;

        base.ChipTriggerUnsetter();


    }

}
