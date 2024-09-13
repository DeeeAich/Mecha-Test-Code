using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDebrisExplosion : MonoBehaviour
{
    public bool testTrigger = false;

    public GameObject explosionVFX;
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
        if (testTrigger)
        {
            testTrigger = false;
            TriggerExplosion();
        }
        if (scaleDown && scale>0)
        {
            scale -= Time.deltaTime * scaleSpeed;
            for (int i = 0; i < objectsToPush.Count; i++)
            {
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
        explosionVFX.SetActive(true);
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
            objectsToPush[i].SetActive(true);
            objectsToPush[i].GetComponent<Rigidbody>().isKinematic = false;
            objectsToPush[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionVFX.transform.position, explosionRadius);
        }
        StartCoroutine(WaitToScale());
    }

    IEnumerator WaitToScale()
    {
        yield return new WaitForSeconds(timeBeforeScaleDebris);
        scaleDown = true;
    }
}
