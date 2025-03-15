using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEditor;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] List<Image> healthBars;
    private float newHealth;
    private float currentHealth;
    private float maxHealth;
    private bool healthChanging;
    [SerializeField] List<TextMeshProUGUI> healthTexts;
    [SerializeField] float healthChangeSpeed;
    float currentPoint;
    [SerializeField] Image[] weaponImages;
    [SerializeField] TextMeshProUGUI leftAmmo;
    [SerializeField] TextMeshProUGUI rightAmmo;
    [SerializeField] List<Image> dashCharges = new();
    [SerializeField] TextMeshProUGUI dashCountTxt;
    private bool dashCharging;
    private float dashChargeTime;
    private float curCharge;
    private int dashCount;
    private int dashTotal;
    [SerializeField] Color32 dashColour;
    [SerializeField] Image ultimate;
    private bool ultCharging = false;
    private float ultCharge;
    private float ultChargeTime;

    [SerializeField] GameObject deathImage;
    [SerializeField] float deathTime;

    public static PlayerUI instance;

    [SerializeField] Animator healthChanges;

    private void Awake()
    {

        instance = this;

    }

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

            foreach(Image health in healthBars)
                health.fillAmount = currentHealth / maxHealth;
            healthTexts[0].text = currentHealth.ToString();

        }

        //dash charge
        if (dashCharging)
        {
            curCharge += Time.deltaTime;
            dashCharges[0].fillAmount = curCharge / dashChargeTime;

            if(curCharge >= dashChargeTime * 0.97)
            {
                dashCount++;

                dashCharging = dashCount != dashTotal;

                curCharge = 0;

                dashCountTxt.text = dashCount.ToString();
            }

            if (dashCount == 0)
                dashCharges[1].color = Color.gray;
            else
                dashCharges[1].color = dashColour;

        }

        if (ultCharging)
        {

            ultCharge += Time.deltaTime;
            ultimate.fillAmount = ultCharge / ultChargeTime;

            if(ultCharge >= ultChargeTime * 99)
            {

                ultCharge = 0;
                ultimate.fillAmount = 1;
                ultCharging = false;

            }

        }
        
    }

    public void HealthChanged(float nextHealth, float newMaxHealth = 0)
    {

        healthChanging = true;
        newHealth = nextHealth;

        if (newMaxHealth != 0)
        {
            maxHealth = newMaxHealth;
            healthTexts[1].text = newMaxHealth.ToString();
        }

    }

    public void WeaponChange(Sprite weaponImage, bool left)
    {

        if (left)
        {
            weaponImages[0].gameObject.SetActive(true);
            weaponImages[1].gameObject.SetActive(true);
            weaponImages[0].sprite = weaponImage;
            weaponImages[1].sprite = weaponImage;
        }
        else
        {
            weaponImages[2].gameObject.SetActive(true);
            weaponImages[3].gameObject.SetActive(true);
            weaponImages[2].sprite = weaponImage;
            weaponImages[3].sprite = weaponImage;
        }
    }

    public void WeaponAmmoLeft(float max, float cur)
    {

        weaponImages[0].fillAmount = cur / max;
        leftAmmo.text = cur.ToString();

    }

    public void WeaponAmmoRight(float max, float cur)
    {

        weaponImages[2].fillAmount = cur / max;
        rightAmmo.text = cur.ToString();

    }

    public void Dashed()
    {

        dashCount--;

        dashCountTxt.text = dashCount.ToString();

        dashCharging = true;

    }

    public void UltUsed()
    {

        ultCharging = true;
        ultCharge = 0;

    }

    public void LockAndLoad(float mHealth, float curHealth, int leftAm, int rightAm, float dashCha,
        int dashMax, float ultCha, Sprite left = null, Sprite right = null)
    {

        healthTexts[0].text = curHealth.ToString();
        currentHealth = curHealth;
        healthTexts[1].text = mHealth.ToString();
        maxHealth = mHealth;

        leftAmmo.text = leftAm.ToString();
        rightAmmo.text = rightAm.ToString();

        if (left != null)
        {
            weaponImages[0].sprite = left;
            weaponImages[1].sprite = left;
        }
        else
        {
            weaponImages[0].gameObject.SetActive(false);
            weaponImages[1].gameObject.SetActive(false);
        }
        if (right != null)
        {
            weaponImages[2].sprite = right;
            weaponImages[3].sprite = right;
        }
        else
        {

            weaponImages[1].gameObject.SetActive(false);
            weaponImages[1].gameObject.SetActive(false);
        }
        

        dashCount = dashMax;
        dashTotal = dashMax;

        dashCountTxt.text = dashMax.ToString();

        dashChargeTime = dashCha;

        ultChargeTime = ultCha;

    }

    public IEnumerator StartDeath()
    {

        deathImage.SetActive(true);

        yield return new WaitForSeconds(deathTime);

        FadeCanvas.instance.FadeToBlack();

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(1, LoadSceneMode.Single);

        yield return null;

    }

    public void OnHealthChange(bool healing)
    {

        healthChanges.SetTrigger(healing ? "Heal" : "Hit");

    }
}
