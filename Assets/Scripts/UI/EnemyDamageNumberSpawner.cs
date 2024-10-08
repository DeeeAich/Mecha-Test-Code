using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyDamageNumberSpawner : MonoBehaviour
{
    public GameObject damageNumberInstance;
    public Color32 critColor;


    public bool testSpawn;
    public float testDamage;
    public bool testCrit;
    void Update()
    {
        if (testSpawn)
        {
            testSpawn = false;
            SpawnDamageNumber(testDamage, testCrit);
        }
    }

    public void SpawnDamageNumber(float damageAmount, bool isCritical = false)
    {
        GameObject newDamageNumberInstance = Instantiate(damageNumberInstance, this.transform);
        newDamageNumberInstance.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        newDamageNumberInstance.GetComponent<TextMeshProUGUI>().text = damageAmount.ToString();
        if (isCritical)
        {
         newDamageNumberInstance.GetComponent<TextMeshProUGUI>().color = critColor;
        }

        if (damageAmount <= 10) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.4f; }
        if (damageAmount <= 20 && damageAmount > 10) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.5f; }
        if (damageAmount <= 30 && damageAmount > 20) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.6f; }
        if (damageAmount <= 50 && damageAmount > 30) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.7f; }
        if (damageAmount <= 80 && damageAmount > 50) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.8f; }
        if (damageAmount <= 100 && damageAmount > 80) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1f; }
        if (damageAmount <= 150 && damageAmount > 100) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.1f; }
        if (damageAmount <= 200 && damageAmount > 150) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.2f; }
        if (damageAmount <= 300 && damageAmount > 200) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.3f; }
        if (damageAmount <= 400 && damageAmount > 300) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.4f; }
        if (damageAmount <= 550 && damageAmount > 400) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.5f; }
        if (damageAmount <= 700 && damageAmount > 550) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.7f; }
        if (damageAmount <= 800 && damageAmount > 700) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.8f; }
        if (damageAmount <= 900 && damageAmount > 800) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.9f; }
        if (damageAmount <= 1000 && damageAmount > 900) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 2f; }
        if (damageAmount > 1000) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 2f; }

        Destroy(newDamageNumberInstance, 1f);
    }
}
