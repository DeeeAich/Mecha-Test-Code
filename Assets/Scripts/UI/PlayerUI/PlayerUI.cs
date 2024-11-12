using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] Image health;
    private int newHealth;
    private int currentHealth;
    private int maxHealth;
    private bool healthChanging;
    [SerializeField] List<TextMeshProUGUI> healthTexts;
    [SerializeField] float healthChangeSpeed;
    float currentPoint;
    [SerializeField] Image leftWeapon;
    [SerializeField] TextMeshProUGUI leftAmmo;
    [SerializeField] Image rightWeapon;
    [SerializeField] TextMeshProUGUI rightAmmo;
    [SerializeField] List<Image> dashCharges = new();
    [SerializeField] Image ultimate;

    private void FixedUpdate()
    {

        //healthbar
        if (healthChanging)
        {
            currentPoint += Time.deltaTime;
            currentHealth = Mathf.RoundToInt(Mathf.Lerp(currentHealth, newHealth, currentPoint / healthChangeSpeed));

            if (currentPoint >= healthChangeSpeed)
            {
                currentHealth = newHealth;
                healthChanging = false;
                currentPoint = 0;
            }
            health.fillAmount = currentHealth / maxHealth;

        }
        
    }

    public void HealthChanged(int nextHealth, int newMaxHealth = 0)
    {

        healthChanging = true;
        newHealth = nextHealth;

        if (newMaxHealth != 0)
        {
            maxHealth = newMaxHealth;
            healthTexts[1].text = newMaxHealth.ToString();
        }

    }

    public void WeaponAmmoLeft(int max, int cur)
    {

        leftWeapon.fillAmount = cur / max;
        leftAmmo.text = cur.ToString();

    }

    public void WeaponAmmoRight(int max, int cur)
    {

        rightWeapon.fillAmount = cur / max;
        rightAmmo.text = cur.ToString();

    }

    public void LockAndLoad(int mHealth, int curHealth, int leftAm, int rightAm, Sprite left = null, Sprite right = null)
    {

        healthTexts[0].text = curHealth.ToString();
        currentHealth = curHealth;
        healthTexts[1].text = maxHealth.ToString();
        maxHealth = mHealth;

        leftAmmo.text = leftAm.ToString();
        rightAmmo.text = rightAm.ToString();

        if (left != null)
            leftWeapon.sprite = left;
        if (right != null)
            rightWeapon.sprite = right;

    }
}
