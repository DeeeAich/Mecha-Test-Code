using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExplosionEffect
{
    noEffect,
    hack,
    fire
}



public class Explosion : MonoBehaviour
{
    public float damage;

    public float explosionTime;
    public float linearScale;
    private float timer = 0;

    public ExplosionEffect explosionEffect = ExplosionEffect.noEffect;
    public MeshRenderer aoeRingMeshRenderer;
    public List<Material> aoeRadiusMaterialType;


    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.localScale *= linearScale;
        transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);

        Collider[] hits = Physics.OverlapSphere(transform.position, linearScale / 2);
        foreach(Collider c in hits)
        {
            if(c.TryGetComponent<Health>(out Health hp))
            {
                hp.TakeDamage(damage);
            }
        }

        switch (explosionEffect)
        {
            case ExplosionEffect.noEffect:
                aoeRingMeshRenderer.material = aoeRadiusMaterialType[0];
                break;
            case ExplosionEffect.hack:
                aoeRingMeshRenderer.material = aoeRadiusMaterialType[1];
                break;
            case ExplosionEffect.fire:
                aoeRingMeshRenderer.material = aoeRadiusMaterialType[2];
                break;
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
