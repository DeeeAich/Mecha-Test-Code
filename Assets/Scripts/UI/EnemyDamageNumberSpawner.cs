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
        int damage = Mathf.FloorToInt(damageAmount);

        GameObject newDamageNumberInstance = Instantiate(damageNumberInstance, null);
        newDamageNumberInstance.hideFlags = HideFlags.HideInHierarchy;
        newDamageNumberInstance.GetComponent<RectTransform>().anchoredPosition3D = transform.position;
        newDamageNumberInstance.GetComponent<Animator>().SetFloat("Horizontal", Random.Range(0f, 1f));
        newDamageNumberInstance.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        if (isCritical)
        {
         newDamageNumberInstance.GetComponent<TextMeshProUGUI>().color = critColor;
        }

        if (damage <= 10) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.4f; }
        if (damage <= 20 && damage > 10) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.5f; }
        if (damage <= 30 && damage > 20) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.6f; }
        if (damage <= 50 && damage > 30) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.7f; }
        if (damage <= 80 && damage > 50) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 0.8f; }
        if (damage <= 100 && damage > 80) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1f; }
        if (damage <= 150 && damage > 100) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.1f; }
        if (damage <= 200 && damage > 150) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.2f; }
        if (damage <= 300 && damage > 200) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.3f; }
        if (damage <= 400 && damage > 300) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.4f; }
        if (damage <= 550 && damage > 400) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.5f; }
        if (damage <= 700 && damage > 550) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.7f; }
        if (damage <= 800 && damage > 700) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.8f; }
        if (damage <= 900 && damage > 800) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 1.9f; }
        if (damage <= 1000 && damage > 900) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 2f; }
        if (damage > 1000) { newDamageNumberInstance.GetComponent<TextMeshProUGUI>().fontSize = 2f; }

        Destroy(newDamageNumberInstance, 0.75f);
    }
}
