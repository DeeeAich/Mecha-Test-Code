using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float damage;

    public float explosionTime;
    public float linearScale;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.localScale *= linearScale;

        Collider[] hits = Physics.OverlapSphere(transform.position, linearScale / 2);
        foreach(Collider c in hits)
        {
            if(c.TryGetComponent<Health>(out Health hp))
            {
                hp.TakeDamage(damage);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if(timer > explosionTime)
        {
            Destroy(gameObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Health target;
    //    if(other.TryGetComponent<Health>(out target))
    //    {
    //        target.TakeDamage(damage);
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, linearScale/2);
    }
}
