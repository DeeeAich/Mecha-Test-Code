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

            lineGen.SetPosition(0, firePoint.position);
            lineGen.SetPosition(1, firePoint.position + firePoint.forward * beamRange);

            charge += Time.deltaTime;

            if(charges != maxCharge && charges != curAmmo && charge >= chargeTime * (charges + 1))
            {

                charges++;
                
            }

        }

    }

    public override void FirePress()
    {
        if (waitOnShot || reloading)
            return;

        fireHeld = true;

        lineGen.enabled = true;
    }

    public override void FireRelease()
    {

        if (reloading || charges == 0)
        {
            fireHeld = false;
            lineGen.enabled = false;
            return;
        }
        curAmmo -= charges;
        if (curAmmo == 0)
            StartCoroutine(Reload());

        RaycastHit[] hits = Physics.SphereCastAll(firePoint.position, beamwidth,
                            firePoint.forward, beamRange, layerMask: hitOptions);

        charge = 0;
        charges = 0;

        if(hits.Length == 0)
            for (int i = 0; i < charges; i++)
            {

                if (hits[i].collider.TryGetComponent<Health>(out Health health))
                {

                    health.TakeDamage(damage);

                    GameObject newSparks =  GameObject.Instantiate(sparks, hits[i].point,
                                        Quaternion.LookRotation(hits[i].normal), null);


                }
                else
                {


                    GameObject newSparks = GameObject.Instantiate(sparks, hits[i].point,
                                        Quaternion.LookRotation(hits[i].normal), null);


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


        yield return null;
    }

}
