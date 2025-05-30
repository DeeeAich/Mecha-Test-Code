using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDebrisExplosion : MonoBehaviour
{
    public bool explosionTrigger = false;


    public GameObject explosionVFX;
    public Animator animatorToPause;
    public List<Animator> animatorsToPause;
    public List<GameObject> objectsToPush;
    public List<GameObject> objectsToDelete;
    public List<Renderer> meshRenderersToDisable;

    public float explosionForce = 1;
    public float explosionRadius = 1;

    public float timeToDelete = 2;
    public float timeBeforeScaleDebris = 1;
    public float scaleSpeed = 1;

    bool scaleDown = false;
    float scale = 1;

    void LateUpdate()
    {
        if (explosionTrigger)
        {
            explosionTrigger = false;
            TriggerExplosion();
        }
        if (scaleDown && scale>0.005)
        {
            scale -= Time.deltaTime * scaleSpeed;
            for (int i = 0; i < objectsToPush.Count; i++)
            {
                if(objectsToPush[i] != null)
                objectsToPush[i].transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        if (scale <= 0)
        {
            for (int i = 0; i < objectsToPush.Count; i++)
            {
                Destroy(objectsToPush[i]);
            }
        }
    }


    public void TriggerExplosion()
    {
        Destroy(gameObject, timeToDelete);
        if (explosionVFX != null && !explosionVFX.activeInHierarchy) { explosionVFX.SetActive(true); explosionVFX.transform.parent = null; }
        if (animatorToPause!= null && animatorToPause.enabled) { animatorToPause.speed = 0; }
        if (animatorsToPause != null)
        {
            for (int i = 0; i < animatorsToPause.Count; i++)
            {
                if (animatorsToPause[i].enabled)
                {
                    animatorsToPause[i].speed = 0;
                }
            }
        }
        for (int i = 0; i < meshRenderersToDisable.Count; i++)
        {
            meshRenderersToDisable[i].enabled = false;
        }
        for (int i = 0; i < objectsToDelete.Count; i++)
        {
            Destroy(objectsToDelete[i]);
        }
        for (int i = 0; i < objectsToPush.Count; i++)
        {
            if (objectsToPush[i] == null)
                continue;
            objectsToPush[i].SetActive(true);
            objectsToPush[i].layer = LayerMask.NameToLayer("Debris");
            objectsToPush[i].transform.parent = transform.parent;
            
            if (objectsToPush[i].TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                
                if (explosionVFX != null)
                {
                    rb.AddExplosionForce(explosionForce, explosionVFX.transform.position, explosionRadius);
                } 
                else
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
            
            if(DebrisManager.instance != null) DebrisManager.instance.AddDebris(objectsToPush[i]);
        }
        StartCoroutine(WaitToScale());
    }

    IEnumerator WaitToScale()
    {
        yield return new WaitForSeconds(timeBeforeScaleDebris);
        scaleDown = true;
    }
}
