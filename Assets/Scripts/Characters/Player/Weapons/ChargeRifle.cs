using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChargeRifle : Weapon
{

    private LineRenderer lineGen;

    [Tooltip("0 for charging, 1 for firing")]
    [SerializeField] List<Material> materialOptions;
    [Tooltip("On hit Sparks")]
    [SerializeField] GameObject sparks;

    private float charge;
    private int charges;
    public int maxCharge = 3;
    public float chargeTime = 1f;
    public float beamRange;
    public float beamwidth;
    public float beamMaxWidth;
    private float lineWidth;
    public float damageScale;
    [SerializeField] LayerMask walls; 

    [SerializeField] LayerMask hitOptions;

    public List<ProjectileMod> myMods;

    private bool isFiring;

    private BeamParticles myParticles;

    public override void Start()
    {
        base.Start();

        lineGen = GetComponent<LineRenderer>();

        lineGen.material = materialOptions[0];
        lineGen.enabled = false;

        lineGen.SetPosition(0, firePoint.position);


        myController.ReApplyChips(this == myController.leftWeapon);

        myParticles = GetComponentInChildren<BeamParticles>();

    }

    private float hitDistance;

    private void FixedUpdate()
    {
        
        
        if (fireHeld && !isFiring)
        {
            RaycastHit hit;
            
            lineGen.SetPosition(0, firePoint.position);

            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, beamRange, layerMask: walls))
            {
                lineGen.SetPosition(1, hit.point);
                hitDistance = hit.distance;
            }
            else
            {
                lineGen.SetPosition(1, firePoint.position + firePoint.forward * beamRange);
                hitDistance = hit.distance;
            }
            
            //particleSystem.shape.scale = new Vector3(particleSystem.shape.scale.x, particleSystem.shape.scale.y, Vector3.Distance(firePoint.position, hit.point));

            if(!isFiring) charge += Time.fixedDeltaTime;
            if (charge > maxCharge) charge = maxCharge;

            
            //lineWidth = (beamMaxWidth - beamwidth) * (charge / (chargeTime * maxCharge + 0.5f)) + beamMaxWidth;
            lineWidth = beamwidth + ((beamMaxWidth - beamwidth) / maxCharge) * Mathf.Clamp(charges - 1, 0, maxCharge);
            lineGen.startWidth = lineWidth;
            lineGen.endWidth = lineWidth;

            if(charges != maxCharge && charges != curAmmo && charge >= chargeTime * charges + 0.5f)
            {

                charges++;
                myAnim.SetInteger("ChargeLevel", charges);
                lineGen.material = materialOptions[charges];

            }

        }

    }

    public override void FirePress()
    {
        if (waitOnShot || reloading)
            return;
        else if (curAmmo == 0)
        {
            StartCoroutine(Reload());
            return;
        }
        fireHeld = true;

        lineGen.enabled = true;
        lineGen.SetPosition(0, firePoint.position);
        lineGen.SetPosition(1, firePoint.position);
        charges = 0;

        myAnim.SetInteger("ChargeLevel", 0);
        myAnim.SetBool("Charge", true);
    }

    public override void FireRelease()
    {

        if (reloading || charges == 0)
        {
            fireHeld = false;
            lineGen.enabled = false;
            myAnim.SetBool("Charge", false);
            return;
        }

        myParticles.LaunchParticles(hitDistance);
        myAnim.SetTrigger("Fire");
        curAmmo -= charges * shotCost;

        myAnim.SetBool("Charge", false);

        RaycastHit[] hits = Physics.SphereCastAll(firePoint.position, lineWidth * 2,
                            firePoint.forward, beamRange, layerMask: hitOptions, QueryTriggerInteraction.Ignore);

        float damageModed = GetComponent<Critical>().AdditiveDamage(damage[charges - 1]);

        if (hits.Length > 0)
            foreach (RaycastHit hit in hits)
            {

                if (hit.collider.TryGetComponent<Health>(out Health health))
                {


                    health.TakeDamage(damageModed);
                    foreach (ProjectileMod mod in myMods)
                        mod.AttemptApply(hit.collider.gameObject);

                    GameObject newSparks =  GameObject.Instantiate(sparks, hit.point,
                                        Quaternion.LookRotation(hit.normal), null);

                }
                else
                {


                    foreach (ProjectileMod mod in myMods)
                        mod.AttemptApply(hit.collider.gameObject);
                    GameObject newSparks = GameObject.Instantiate(sparks, hit.point,
                                        Quaternion.LookRotation(hit.normal), null);

                    break;

                }

            }

        StartCoroutine(FlashBeam());

    }

    private IEnumerator FlashBeam()
    {
        isFiring = true;
        lineGen.material = materialOptions[materialOptions.Count - 1];
        lineWidth = beamMaxWidth * 1.2f;

        yield return new WaitForSeconds(0.3f);

        lineGen.material = materialOptions[0];
        lineGen.enabled = false;

        fireHeld = false;


        charge = 0;
        charges = 0;


        if (curAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

        isFiring = false;
        yield return null;
    }

    public override void AddMod(StatusInfo ModInfo)
    {

        string modName = ModInfo.statusType.ToString();

        if (GetComponent(modName) == null)
            foreach (ProjectileMod mod in GetComponents<ProjectileMod>())
                mod.AddModifiers(ModInfo);
        else
        {
            gameObject.AddComponent(Type.GetType(modName));
            foreach (ProjectileMod mod in GetComponents<ProjectileMod>())
                mod.AddModifiers(ModInfo);
        }

    }

}
