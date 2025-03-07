using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBody : MonoBehaviour, IBodyModifiable
{

    public PlayerLegs myMovement;
    public PlayerWeaponControl weaponHolder;
    private CharacterVFXManager vfxManager;
    public Legs legStats;
    public PlayerUltyControl ultController;
    public List<Chip> chipsInserted;
    private PlayerInput playerInputs;
    public Camera myCamera;
    public Transform playerCentre;
    public BodyStats baseStats;
    public List<BodyChip> myMods = new List<BodyChip>();

    [SerializeField] List<Transform> weaponPoints = new();

    //input actions
    private InputAction move, look, dash,
        ultUse, leftFire, rightFire,
        leftRe, rightRe, interact;

    private bool isGamepad = true;

    public PlayerUI myUI;
    private Health myHealth;
    private float lastHealth;
    private float lastMax;

    private Interactable curInteract;

    public bool canMove = true;
    public bool canShoot = true;

    private static PlayerBody instance;
    public static PlayerBody Instance()
    {
        return instance;
    }

    private void Awake()
    {
        playerInputs = GetComponent<PlayerInput>();
        vfxManager = GetComponentInChildren<CharacterVFXManager>();
        myMovement = GetComponent<PlayerLegs>();
        weaponHolder = GetComponent<PlayerWeaponControl>();
        ultController = GetComponent<PlayerUltyControl>();
        myUI = FindObjectOfType<PlayerUI>();
        myHealth = GetComponent<Health>();
        lastHealth = myHealth.health;
        lastMax = myHealth.maxHealth;
        instance = this;
        myHealth.onDeath.AddListener(OnDeath);
    }

    private void Start()
    {
        
        isGamepad = playerInputs.currentControlScheme.Equals("Controller");
        myCamera = Camera.main;
        SetControls();
        SetHooks();
        LoadStats();
    }

    private void FixedUpdate()
    {
        myMovement.Movement( canMove ? move.ReadValue<Vector2>() : new Vector2());
        
        if (canShoot)
            weaponHolder.LookDirection(isGamepad ?
                look.ReadValue<Vector2>():
                Input.mousePosition - myCamera.WorldToScreenPoint(playerCentre.position),
                isGamepad);

    }

    private void Update()
    {
        if (lastHealth != myHealth.health || lastMax != myHealth.maxHealth)
        {
            lastHealth = myHealth.health;
            lastMax = myHealth.maxHealth;
            myUI.HealthChanged(lastHealth, lastMax);
        }

        if(weaponHolder.leftWeapon != null)
            myUI.WeaponAmmoLeft(weaponHolder.leftWeapon.maxAmmo * weaponHolder.leftWeapon.modifiers.ammoCount, weaponHolder.leftWeapon.curAmmo);
        if(weaponHolder.rightWeapon != null)
            myUI.WeaponAmmoRight(weaponHolder.rightWeapon.maxAmmo * weaponHolder.rightWeapon.modifiers.ammoCount, weaponHolder.rightWeapon.curAmmo);
    }

    private void TriggerEndOfRoom()
    {
        foreach(BodyChip chip in myMods)
            if(chip.bodyType == BodyChip.BodyType.EndRoom)
                chip.TriggerAbility();
    }

    public void TriggerOnKill()
    {

        foreach (BodyChip chip in myMods)
            if (chip.bodyType == BodyChip.BodyType.OnKill)
                chip.TriggerAbility();
    }

    public void TriggerOnDamage()
    {
        foreach (BodyChip chip in myMods)
            if (chip.bodyType == BodyChip.BodyType.OnDamage)
                chip.TriggerAbility();

        PlayerUI.instance.OnHealthChange(false);
    }

    public void TriggerOnHeal(int amount)
    {
        vfxManager.SpawnHealParticles(-amount);
        foreach (BodyChip chip in myMods)
            if (chip.bodyType == BodyChip.BodyType.OnHeal)
                chip.TriggerAbility();
        PlayerUI.instance.OnHealthChange(true);

    }

    private void Dash(InputAction.CallbackContext context)
    {
        StartCoroutine(myMovement.Dash(move.ReadValue<Vector2>()));
    }

    private void SetControls()
    {
        move = playerInputs.actions["Move"];
        look = playerInputs.actions["Look"];
        dash = playerInputs.actions["Dash"];
        dash.performed += Dash;

        ultUse = playerInputs.actions["Ultimate"];
        leftFire = playerInputs.actions["Left Fire"];
        leftRe = playerInputs.actions["Left Reload"];

        leftFire.performed += weaponHolder.PressLeft;
        leftFire.canceled += weaponHolder.LiftLeft;
        leftRe.performed += weaponHolder.ReloadLeft;

        rightFire = playerInputs.actions["Right Fire"];
        rightRe = playerInputs.actions["Right Reload"];

        rightFire.performed += weaponHolder.FireRight;
        rightFire.canceled += weaponHolder.LiftRight;
        rightRe.performed += weaponHolder.ReloadRight;

        interact = playerInputs.actions["Interact"];
        interact.performed += Interact;

        ultUse = playerInputs.actions["Ultimate"];
        ultUse.performed += ultController.UseUltimate;
        ultUse.canceled += ultController.EndUltimate;

        playerInputs.onControlsChanged += SetControlScheme;
    }

    private void UnsetControls()
    {

        dash.performed -= Dash;
        leftFire.performed -= weaponHolder.PressLeft;
        leftFire.canceled -= weaponHolder.LiftLeft;
        leftRe.performed -= weaponHolder.ReloadLeft;
        rightFire.performed -= weaponHolder.FireRight;
        rightFire.canceled -= weaponHolder.LiftRight;
        rightRe.performed -= weaponHolder.ReloadRight;
        interact.performed -= Interact;
        playerInputs.onControlsChanged -= SetControlScheme;

    }

    private void SetHooks()
    {

        if(LevelGenerator.instance != null) LevelGenerator.instance.onSpawnRoom.AddListener(TriggerEndOfRoom);

    }

    private void Interact(InputAction.CallbackContext context)
    {
        if(curInteract!=null)
        curInteract.TriggerInteraction();

    }

    private void SetControlScheme(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Controller");

        Debug.Log("Control scheme is now " + input.currentControlScheme);
    }

    private void LoadStats()
    {
        myMovement.curLegs = legStats.LoadLegs();

        myUI.LockAndLoad(myHealth.maxHealth, myHealth.health,
            weaponHolder.leftWeapon != null ? weaponHolder.leftWeapon.curAmmo : 0,
            weaponHolder.rightWeapon != null ? weaponHolder.rightWeapon.curAmmo : 0,
            legStats.dashRecharge, legStats.dashCharges, ultController.currentUltimate.rechargeTime,
            weaponHolder.leftWInfo != null ? weaponHolder.leftWInfo.mySprite : null,
            weaponHolder.rightWInfo != null ? weaponHolder.rightWInfo.mySprite : null);
    }

    public struct LegInfo
    {
        public float speed;
        public float accelleration;
        public float turnSpeed;
        public float dashTime;
        public float dashDistance;
        public float dashRecharge;
        public int dashCharges;
    }

    [Tooltip("Add Prefab, if using left")]
    public void SetWeapon(WeaponPickup setWeapon, bool left)
    {

        myUI.WeaponChange(setWeapon.mySprite, left);

        if (left && weaponHolder.leftWeapon != null)
            Destroy(weaponHolder.leftWeapon.gameObject);
        else if (weaponHolder.rightWeapon != null)
            Destroy(weaponHolder.rightWeapon.gameObject);

        GameObject genWeapon = GameObject.Instantiate(setWeapon.weaponPrefab, weaponPoints[left ? 0 : 1]);

        genWeapon.transform.localScale = new Vector3(left ? 1 : -1, 1, 1);

        genWeapon.GetComponent<Weapon>().myController = weaponHolder;

        if (left)
        {
            weaponHolder.leftWInfo = setWeapon;
            if (PlayerManager.instance != null)
                PlayerManager.instance.leftWeapon = setWeapon;
            weaponHolder.leftWeapon = genWeapon.GetComponent<Weapon>();
        }
        else
        {
            if (PlayerManager.instance != null)
                PlayerManager.instance.rightWeapon = setWeapon;
            weaponHolder.rightWInfo = setWeapon;
            weaponHolder.rightWeapon = genWeapon.GetComponent<Weapon>();
        }

        weaponHolder.ReApplyChips(left);

    }

    public void SetInteract(Interactable interact, bool adding = true)
    {
        
        if(adding)
            curInteract = interact;
        else if (curInteract == interact)
            curInteract = null;

    }

    public void OnDeath()
    {

        myCamera.transform.parent = null;
        StopParts(false, false);

        StartCoroutine(myUI.StartDeath());

        weaponHolder.leftWeapon.FireRelease();
        weaponHolder.rightWeapon.FireRelease();

        TriggerDebrisExplosion[] explosions = GetComponentsInChildren<TriggerDebrisExplosion>();

        foreach (TriggerDebrisExplosion explosion in explosions)
            explosion.explosionTrigger = true;

        UnsetControls();

        LevelGenerator.instance.onSpawnRoom.RemoveListener(TriggerEndOfRoom);

    }

    [Tooltip("For setting the parts to on or off")]
    public void StopParts(bool weapons, bool legs)
    {

        canShoot = weapons;
        canMove = legs;
    }

    public BodyStats myStats;

    public void ApplyChip(BodyChip chip)
    {

        myMods.Add(chip);

        switch (chip.bodyType)
        {

            case (BodyChip.BodyType.Stat):
                ApplyStatusChange();
                break;
            case (BodyChip.BodyType):

                break;
        }

    }

    private void ApplyStatusChange()
    {

        foreach(BStatChip bStat in myMods)
        {



        }

    }

    public void Preform(IEnumerator corout)
        => StartCoroutine(corout);

    private void OnDestroy()
    {

        UnsetControls();

    }
}
