using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "Airstrike", menuName = "Player/Ultimates/AirStrike")]
public class Airstike : Ultimate
{

    public int enemiesToHit = 9;
    public float betweenShots = 0.2f;
    public float locationRandom = 0.5f;

    public override void ActivateUltimate()
    {
        if (FindObjectsOfType<AITree.BehaviourTree>().Length == 0)
            return;

        PlayerUltyControl.instance.recharging = true;

        StartCoroutine(Firing());
        
    }

    public IEnumerator Firing()
    {

        PlayerUltyControl.instance.RunAnimation("Fire");

        yield return new WaitForSeconds(castTime);

        PlayerUI.instance.UltUsed();

        AITree.BehaviourTree[] enemies = FindObjectsOfType<AITree.BehaviourTree>();
        int listLoop = 0;
        List<GameObject> missiles = new();
        for (int i = 0; i < enemiesToHit; i++)
        {
            if (i == enemies.Length * (1 + listLoop))
                listLoop++;

            Vector3 location = enemies[i - (listLoop * enemies.Length)].transform.position
                + new Vector3(Random.Range(-locationRandom, locationRandom), 0, Random.Range(-locationRandom, locationRandom));
            GameObject newMissile = GameObject.Instantiate(ultObject, location, ultObject.transform.rotation, enemies[i - (listLoop * enemies.Length)].transform);
            missiles.Add(newMissile);

            newMissile.GetComponentInChildren<BasicBullet>().damage = damages[0];

        }

        foreach (GameObject missile in missiles)
        {

            missile.SetActive(true);

            missile.transform.parent = null;

            yield return new WaitForSeconds(betweenShots);

        }

        yield return new WaitForSeconds(rechargeTime);

        PlayerUltyControl.instance.RunAnimation("Reload");
        PlayerUltyControl.instance.recharging = false;

        yield return null;

    }

}
