using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public GameObject gunPoint;
    public GameObject shotPattern;
    public float startDelayMin, startDelayMax;
    public float minDelay, maxDelay;
    public float damage;
    public Animator anim;
    public int ammoPerReload = 0;
    internal int currentAmmo;
    public float reloadTime = 1f;
    [SerializeField] EnemyStats stats;

    private void OnEnable()
    {
        //Subscribe to Event
        if (stats == null)
        {
            if (TryGetComponent<EnemyStats>(out stats))
            {
                damage = stats.damage;
                stats.onStatsChanged.AddListener(FetchDamage);
            }
            else
            {
                //stats is not on same level
            }
        }
        else
        {
            damage = stats.damage;
            stats.onStatsChanged.AddListener(FetchDamage);
        }
    }

    private void OnDisable()
    {
        //Event posted cringe, unsub
        if (stats == null)
        {
            if (TryGetComponent<EnemyStats>(out stats))
            {
                damage = stats.damage;
                stats.onStatsChanged.RemoveListener(FetchDamage);
            }
            else
            {
                //stats is not on same level
            }
        }
        else
        {
            damage = stats.damage;
            stats.onStatsChanged.RemoveListener(FetchDamage);
        }
    }

    void FetchDamage()
    {
        damage = stats.damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = ammoPerReload;
        StartCoroutine(StartDelay());
    }

    IEnumerator StartDelay()
    {
        float timer = 0f;
        float randTime = Random.Range(startDelayMin, startDelayMax);
        while (timer < randTime)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        StartCoroutine(FireOnRepeat());
    }

    public virtual IEnumerator FireOnRepeat()
    {
        float timer = 0f;
        float randTime;
        while (true)
        {
            randTime = Random.Range(minDelay, maxDelay);
            while (timer < randTime)
            {
                yield return null;
                timer += Time.deltaTime;
            }
            GameObject spawned = Instantiate(shotPattern, gunPoint.transform.position, gunPoint.transform.rotation, transform);
            MoveProjectile[] bullets = GetComponentsInChildren<MoveProjectile>();
            foreach(MoveProjectile m in bullets)
            {
                m.damage = damage; //I might want to seperate bullet movement and collision, moveProj and bulletHit
            }
            spawned.transform.parent = null;
            anim.SetTrigger("shoot");
            timer = 0f;
            currentAmmo--;
            if (currentAmmo <= 0 && ammoPerReload != 0)
            {
                yield return null;
                anim.SetTrigger("reload");
                yield return new WaitForSeconds(reloadTime);
                currentAmmo = ammoPerReload;
            }

        }
    }

    public void BeGone()
    {
        StopAllCoroutines();
    }
}
