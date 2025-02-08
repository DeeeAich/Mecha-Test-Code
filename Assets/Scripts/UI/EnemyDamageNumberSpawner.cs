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
            SpawnDamageNumber(testDamage, testCrit, PlayerBody.PlayBody().transform.position);

        }



    }

    public void SpawnDamageNumber(float damageAmount, bool isCritical = false, Vector3 myLocation = new Vector3())
    {
        int damage = Mathf.FloorToInt(damageAmount);

        Vector3 canvasPoint = Camera.main.WorldToScreenPoint(myLocation);

        GameObject newDamageNumberInstance = Instantiate(damageNumberInstance, transform);

        newDamageNumberInstance.transform.position = canvasPoint;
        newDamageNumberInstance.hideFlags = HideFlags.HideInHierarchy;
        newDamageNumberInstance.GetComponent<Animator>().SetFloat("Horizontal", Random.Range(0f, 1f));
        newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();

        if (isCritical)
            newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().color = critColor;

        /*if (damage <= 10) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 10f; }

        if (damage <= 20 && damage > 10) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 15f; }
        if (damage <= 30 && damage > 20) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20f; }
        if (damage <= 40 && damage > 30) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 25f; }
        if (damage <= 50 && damage > 40) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 30f; }
        if (damage <= 60 && damage > 50) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 35f; }
        if (damage <= 70 && damage > 60) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 40f; }
        if (damage <= 80 && damage > 70) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 45f; }
        if (damage <= 90 && damage > 80) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 50f; }
        if (damage <= 100 && damage > 90) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 55f; }
        if (damage <= 200 && damage > 100) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 60f; }
        if (damage <= 300 && damage > 200) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 65f; }
        if (damage <= 400 && damage > 300) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 70f; }
        if (damage <= 500 && damage > 400) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 75f; }
        if (damage <= 600 && damage > 500) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 80f; }
        if (damage <= 700 && damage > 600) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 85f; }
        if (damage <= 800 && damage > 700) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90f; }
        if (damage <= 900 && damage > 800) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 95f; }

        if (damage > 1000) { newDamageNumberInstance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 100f; } */

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
            DestroyImmediate(oldestnumber);
        }
    }
}
