using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float damage;
    [SerializeField] LayerMask walls; 

    [SerializeField] LayerMask hitOptions;

    public override void Start()
    {
        base.Start();

        lineGen = GetComponent<LineRenderer>();

        lineGen.material = materialOptions[0];
        lineGen.enabled = false;

        lineGen.SetPosition(0, firePoint.position);
    }

    private void FixedUpdate()
    {

        if (fireHeld)
        {

            RaycastHit hit;

            lineGen.SetPosition(0, firePoint.position);

            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, beamRange, layerMask: walls))
                lineGen.SetPosition(1, hit.point);
            else
                lineGen.SetPosition(1, firePoint.position + firePoint.forward * beamRange);

            charge += Time.fixedDeltaTime;

            if(charges != maxCharge && charges != curAmmo && charge >= chargeTime * charges + 0.5f)
            {

                charges++;
                myAnim.SetInteger("ChargeLevel", charges);

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
            return;
        }

        myAnim.SetTrigger("Fire");
        curAmmo -= charges * shotCost;

        myAnim.SetBool("Charge", false);

        RaycastHit[] hits = Physics.SphereCastAll(firePoint.position, beamwidth,
                            firePoint.forward, beamRange, layerMask: hitOptions, QueryTriggerInteraction.Ignore);

        if(hits.Length > 0)
            foreach (RaycastHit hit in hits)
            {

                if (hit.collider.TryGetComponent<Health>(out Health health))
                {

                    health.TakeDamage(damage * charges);

                    GameObject newSparks =  GameObject.Instantiate(sparks, hit.point,
                                        Quaternion.LookRotation(hit.normal), null);

                }
                else
                {


                    GameObject newSparks = GameObject.Instantiate(sparks, hit.point,
                                        Quaternion.LookRotation(hit.normal), null);

                    break;

                }

            }

        StartCoroutine(FlashBeam());

    }

    private IEnumerator FlashBeam()
    {

        lineGen.material = materialOptions[1];

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

        yield return null;
    }

}
