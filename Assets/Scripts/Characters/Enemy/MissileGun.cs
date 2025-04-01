using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileGun : EnemyGun
{
    public override IEnumerator FireOnRepeat()
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
            MoveProjectile[] bullets = spawned.GetComponentsInChildren<MoveProjectile>();
            foreach (MoveProjectile m in bullets)
            {
                m.damage = damage; //I might want to seperate bullet movement and collision, moveProj and bulletHit
            }
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
}
