using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IHackable, IBurnable
{
    public float health;
    public float maxHealth;
    public bool isAlive = true;
    public bool canTakeDamage = true;
    public bool canDie = true;

    //public string entityType;
    public EntityType entityType;
    [Tooltip("This marks the outer bounds of the entity, for the purposes of positioning and scaling anything attatched to them (ie: healthbars, shield circles etc)")]
    public Vector3 entityBounds = new Vector3(1, 1, 1);

    public bool editorTakeDamage = false;
    public float editorDamageAmount = 100f;

    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyTimer = 3f;

    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;

    public List<DamageMod> damageMods;

    public GameObject meshesRef;//The shield renderer thing needs thee target to be where the meshes are

    internal virtual void Update()
    {
        if (editorTakeDamage)
        {
            TakeDamage(editorDamageAmount, out bool discard);
            editorTakeDamage = false;
        }
    }

    internal virtual void Awake()
    {
        health = maxHealth;
        damageMods = new List<DamageMod>();
        burns = new List<Coroutine>();
    }

    internal virtual float TakeDamage(float amount, out bool isShield, bool isCrit = false)
    {
        isShield = false;
        if (!canTakeDamage || !isAlive)
        {
            return 0f;
        }
        for (int i = 0; i < damageMods.Count; i++)
        {
            if (damageMods[i].removeFlag)
            {
                damageMods.RemoveAt(i);
                i--;
            }
        }
        float damageTaken = 0f;
        float remainingDamage = amount;
        foreach (DamageMod mod in damageMods)
        {
            //Debug.Log("Current Damage: " + remainingDamage);
            damageTaken += mod.Modification(remainingDamage, out remainingDamage);
            //Debug.Log("New Damage: " + remainingDamage);
            if(mod is ShieldModifier)
            {
                isShield = true;
            }
        }



        health -= remainingDamage;

        onTakeDamage.Invoke();



        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (health <= 0 && canDie)
        {
            if (gameObject.tag != "Player")
                PlayerBody.PlayBody().TriggerOnKill();
            TriggerDeath();
        }

        return damageTaken + remainingDamage;
    }
    internal virtual float TakeDamage(float amount, bool isCrit = false)
    {
        if (!canTakeDamage || !isAlive)
        {
            return 0f;
        }
        for (int i = 0; i < damageMods.Count; i++)
        {
            if (damageMods[i].removeFlag)
            {
                damageMods.RemoveAt(i);
                i--;
            }
        }
        float damageTaken = 0f;
        float remainingDamage = amount;
        foreach (DamageMod mod in damageMods)
        {
            //Debug.Log("Current Damage: " + remainingDamage);
            damageTaken += mod.Modification(remainingDamage, out remainingDamage);
            //Debug.Log("New Damage: " + remainingDamage);
        }



        health -= remainingDamage;

        onTakeDamage.Invoke();



        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (health <= 0 && canDie)
        {
            if (gameObject.tag != "Player")
                PlayerBody.PlayBody().TriggerOnKill();
            TriggerDeath();
        }

        return damageTaken + remainingDamage;
    }
    internal virtual void TriggerDeath()
    {
        isAlive = false;
        canTakeDamage = false;
        onDeath.Invoke();
        if (destroyOnDeath) Destroy(gameObject, destroyTimer);
    }


    #region Hackable Interface Implementation
    Coroutine hackCoroutine;
    HackMod hack;
    float hackTimer;
    public virtual void Hack(float percentage, float chance, float duration)
    {
        int application = (int)(chance / 100f);

        if(chance % 100f >= UnityEngine.Random.Range(0f,100f))
        {
            application += 1;
        }
        if(application == 0)
        {
            return;
        }
        if (hackCoroutine == null)
        {
            hack = new HackMod(percentage*application);
            damageMods.Add(hack);
            hackCoroutine = StartCoroutine(HackDecay());
            hackTimer = duration;
        }
        else
        {
            hackTimer = Mathf.Max(hackTimer, duration);
            hack.percent = Mathf.Max(hack.percent, percentage*application);
        }
        //apply VFX for time
    }

    public virtual IEnumerator HackDecay()
    {
        CharacterVFXManager manager = GetComponentInChildren<CharacterVFXManager>();
        if (manager != null)
        {
            manager.ToggleEffectVFX(effect.Hack, true);
        }
        while (hackTimer > 0)
        {
            hackTimer -= Time.deltaTime;
            yield return null;
        }
        damageMods.Remove(hack); 
        if (manager != null)
        {
            manager.ToggleEffectVFX(effect.Hack, false);
        }
    }


    #endregion
    #region Burnable Interface Implementation
    List<Coroutine> burns;
    bool activeBurnEffect;
    IEnumerator BurnDamage(float damagePerTick, int count)
    {
        float timer = 0;
        float maxTime = 0.25f;
        int ticks = 0;
        while(ticks < count)
        {
            while(timer < maxTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            timer -= maxTime;
            ticks++;
            TakeDamage(damagePerTick, out bool discard);
        }
        yield return null;
    }
    public virtual void Burn(float chance, float damageTick, int tickCount)
    {
        //tick length is locked at 0.25s/t
        //Debug.Log(chance);
        int application = (int)(chance / 100f);
        //Debug.Log(application);
        if (chance % 100f >= UnityEngine.Random.Range(0f, 100f))
        {
            application += 1;
        }
        for(int i = 0; i < application; i++)
        {
            burns.Add(StartCoroutine(BurnDamage(damageTick,tickCount)));
        }
        if(!activeBurnEffect)
        StartCoroutine(BurnFX());
    }

    IEnumerator BurnFX()
    {
        CharacterVFXManager manager = GetComponentInChildren<CharacterVFXManager>();
        if(manager!=null)
        {
            manager.ToggleEffectVFX(effect.Burn, true);
        }
        activeBurnEffect = true;
        foreach(Coroutine c in burns)
        {
            yield return c;
        }
        activeBurnEffect = false;
        if (manager != null)
        {
            manager.ToggleEffectVFX(effect.Burn, false);
        }
    }

    #endregion
    /*SHIELD MECHANICS
    
    Shields can stack
    Smallest health sheild takes damage first //not yet implemented, just take damage in order applied (should be smallest to largest unless shields of varying sizes happen
    Shield provider gets stunned
    Healths have shieldable stat and shielded stat
    
    */
}

public abstract class DamageMod
{
    public bool removeFlag = false;
    public abstract float Modification(float damage, out float remainingDamage);
    //return damage dealt, input damage to process, modify remaining damage
}

public class HackMod : DamageMod
{
    internal float percent;
    public HackMod(float percent)
    {
        this.percent = percent;
    }

    public override float Modification(float damage, out float remaining)
    {
        //Debug.Log("hack");
        float multiplier = percent / 100f;
        multiplier += 1f;
        //Debug.Log(multiplier);
        remaining = damage * multiplier;
        return 0f;
    }
}

public class ShieldModifier : DamageMod
{
    public float shieldHealth;
    readonly Shielder source;
    public ShieldModifier(float shieldHealth, Shielder source)
    {
        this.shieldHealth = shieldHealth;
        this.source = source;
    }

    public override float Modification(float damage, out float remaining)
    {
        float returnDamage = damage;
        if (damage >= shieldHealth)
        {
            remaining = damage - shieldHealth;
            returnDamage = shieldHealth;
            shieldHealth = 0;
            source.Break();
            removeFlag = true;
        }
        else
        {
            remaining = 0;
            shieldHealth -= damage;
        }
        return returnDamage;
    }
}
