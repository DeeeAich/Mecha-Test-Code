using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyDamageNumberSpawner : MonoBehaviour
{
    public static EnemyDamageNumberSpawner instance;


    public GameObject damageNumberInstance;
    public Color32 critColor;
    public Color32 shieldColor;
    public bool testSpawn;
    public int minFont;
    public int maxFont;
    public float testDamage;
    public bool testCrit;

    public List<GameObject> damageNumbers;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        if (testSpawn)
        {
            testSpawn = false;
            SpawnDamageNumber(testDamage, PlayerBody.Instance().transform.position, testCrit);

        }



    }

    public void SpawnDamageNumber(float damageAmount, Vector3 myLocation, bool isCritical = false, bool hitShield = false)
    {

        if(damageAmount <= 0)
            return;

        int damage = Mathf.FloorToInt(damageAmount);

        Vector3 canvasPoint = Camera.main.WorldToScreenPoint(myLocation);

        GameObject newDamageNumberInstance = Instantiate(damageNumberInstance, transform);

        newDamageNumberInstance.transform.position = canvasPoint;
        newDamageNumberInstance.hideFlags = HideFlags.HideInHierarchy;
        newDamageNumberInstance.GetComponent<Animator>().SetFloat("Horizontal", Random.Range(0f, 1f));
        newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();


        if (hitShield)
        {
            newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().color = shieldColor;
        } 
        else if (isCritical)
        {
            newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().color = critColor;
        }


        int fontSize = 10;

        if (damage > 500)
            fontSize = maxFont;
        else
            fontSize = Mathf.RoundToInt((damage / 500) * (maxFont - minFont)) + minFont;

        newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = fontSize;
        
        damageNumbers.Add(newDamageNumberInstance);

        if (damageNumbers.Count > 5)
        {
            GameObject oldestnumber = damageNumbers[0];
            damageNumbers.Remove(oldestnumber);
            Destroy(oldestnumber);
        }
    }

    public void SpawnDamageNumber(Health.DamageEventInfo info, Vector3 myLocation)
    {
        int damage = (int)info.totalDamge;

        Vector3 canvasPoint = Camera.main.WorldToScreenPoint(myLocation);

        GameObject newDamageNumberInstance = Instantiate(damageNumberInstance, transform);

        newDamageNumberInstance.transform.position = canvasPoint;
        newDamageNumberInstance.hideFlags = HideFlags.HideInHierarchy;
        newDamageNumberInstance.GetComponent<Animator>().SetFloat("Horizontal", Random.Range(0f, 1f));
        newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString(); 
        
        
        
        if (info.isShielded)
        {
            newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().color = shieldColor;
        }
        else if (info.critCount > 0)
        {
            newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().color = critColor;
        }


        int fontSize = 10;

        if (damage > 500)
            fontSize = maxFont;
        else
            fontSize = Mathf.RoundToInt((damage / 500) * (maxFont - minFont)) + minFont;

        newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = fontSize;

        damageNumbers.Add(newDamageNumberInstance);

        if (damageNumbers.Count > 5)
        {
            GameObject oldestnumber = damageNumbers[0];
            damageNumbers.Remove(oldestnumber);
            Destroy(oldestnumber);
        }
    }
}
