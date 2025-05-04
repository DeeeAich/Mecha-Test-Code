using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "newHealToPercent", menuName = "Player/Chip/Body/Trigger/HealToPercent")]
public class HealOverTimer : BodyTriggerChip
{

    public float healPercent = 0.2f;
    public float healPerTick = 2;
    public int tickPerSecond = 4;
    private float tickTimer = 0;
    private bool betweemTicks = false;

    public float pauseTimeForDamage = 2f;

    private UnityAction pauseAction;
    private bool pauseForDamge = false;
    private float pauseTimer = 0;


    public override void ChipTriggerSetter()
    {
        if (!addedAction)
        {
            startAction += () => HealAbility(PlayerBody.Instance().GetComponent<Health>());
            pauseAction += () => PauseForDmg();

            addedAction = true;
        }
        PlayerBody.Instance().triggers.constant += startAction;
        PlayerBody.Instance().triggers.damaged += pauseAction;

    }

    public void HealAbility(Health myHealth)
    {

        if (!pauseForDamge && !betweemTicks && myHealth.health / myHealth.maxHealth < healPercent)
        {
            myHealth.TakeDamage(-healPerTick);
            tickTimer = 0;
            betweemTicks = true;

        }
        else if(betweemTicks && !pauseForDamge)
        {
            tickTimer += Time.deltaTime;
            if(tickTimer > 1 / tickPerSecond)
                betweemTicks = false;
        }
        else
        {

            pauseTimer += Time.deltaTime;
            if(pauseTimer > pauseTimeForDamage)
            {
                Debug.Log("Unpaused");
                betweemTicks = false;
                pauseForDamge = false;
            }

        }

    }

    public void PauseForDmg()
    {

        pauseForDamge = true;
        pauseTimer = 0;

    }

}
