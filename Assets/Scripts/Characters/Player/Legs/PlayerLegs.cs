using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegs : MonoBehaviour, ILegModifiable
{
    public PlayerBody myBody;
    public PlayerBody.LegInfo curLegs;
    public static Vector2 curSpeed;
    public GameObject myLegs;
    public bool dashing = false;
    public Vector3 dashDirection;
    public Rigidbody ridBy;
    public List<MovementChip> legChips;
    [SerializeField] internal LegStatChange legMods;
    [SerializeField] internal DashStatChange dashMods;

    public virtual void Movement(Vector2 stickAmount)
    {

        if (stickAmount.magnitude != 0 && !dashing)
        {
            curSpeed += stickAmount * myBody.legStats.speed * legMods.speed;

            if (curSpeed.magnitude > (stickAmount * myBody.legStats.speed * legMods.speed).magnitude)
                curSpeed = stickAmount * myBody.legStats.speed;
        }
        else if (!dashing)
        {
            curSpeed = new Vector2();
        }

        ridBy.velocity = new Vector3(curSpeed.x, 0, curSpeed.y);
    }

    public virtual IEnumerator Dash(Vector2 stickAmount)
    {
        if (dashing || myBody.legStats.dashCharges == 0)
            yield break;

        dashDirection = new Vector2();

        if(stickAmount.magnitude != 0)
        {
            dashDirection = (stickAmount * 10).normalized;
        }
        else if(curSpeed.magnitude != 0)
        {
            dashDirection = curSpeed.normalized;
        }
        else
        {
            yield break;
        }

        myLegs.GetComponent<MultipleLegIkMover>().ToggleDashParticles(true);
        myBody.myUI.Dashed();

        yield return null;

    }

    public virtual void Start()
    {
        myBody = GetComponent<PlayerBody>();
        ridBy = GetComponent<Rigidbody>();
    }

    public virtual void OnCollisionEnter(Collision collision)
    {

        if(dashing && collision.gameObject.layer == 0)
        {



        }

    }

    public void ApplyChip(MovementChip newChip)
    {

        legChips.Add(newChip);

        switch(newChip.moveType)
        {
            case (MovementChip.MovementType.LegStat):
                LegStatChip legStatChip = (LegStatChip)newChip;
                ApplyLegStats(legStatChip.statChange);
                break;
            case (MovementChip.MovementType.DashStat):
                DashStatChip dashStatChip = (DashStatChip)newChip;
                ApplyDashStats(dashStatChip.statChange);
                break;
            case (MovementChip.MovementType.Trigger):
                ApplyTriggerChip((MovementTriggerChip)newChip);
                break;
        }

    }

    public void ApplyLegStats(LegStatChange chipChange)
    {
        chipChange.AddStats(legMods);
    }

    public void RemoveLegStats(LegStatChange chipChange)
    {
        chipChange.RemoveStats(legMods);
    }

    public void ApplyDashStats(DashStatChange chipChange)
    {
        chipChange.AddStats(dashMods);

        if (dashMods.dashCharges > 0)
            PlayerUI.instance.DashChanged(dashMods.dashCharges + curLegs.dashCharges);

    }

    public void RemoveDashStats(DashStatChange chipChange)
    {
        chipChange.RemoveStats(dashMods);
    }

    public void ApplyTriggerChip(MovementTriggerChip triggerChip)
    {

        triggerChip.ChipTriggerSetter(this);

    }

}
