using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBody : MonoBehaviour, IBodyModifiable
{

    private PlayerLegs myMovement;
    private PlayerWeaponControl weaponHolder;
    public Legs legStats;
    public Ultimate ultimate;
    public List<Chip> chipsInserted;
    public LegInfo curLegs;
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

    private static PlayerBody playerBody;
    public static PlayerBody PlayBody()
    {
        return playerBody;
    }

    private void Awake()
    {
        playerInputs = GetComponent<PlayerInput>();
        myMovement = GetComponent<PlayerLegs>();
        weaponHolder = GetComponent<PlayerWeaponControl>();
        myUI = FindObjectOfType<PlayerUI>();
        myHealth = GetComponent<Health>();
        lastHealth = myHealth.health;
        lastMax = myHealth.maxHealth;
        playerBody = this;
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
        if (canMove)
            myMovement.Movement(move.ReadValue<Vector2>());

        weaponHolder.LookDirection(isGamepad ?
            look.ReadValue<Vector2>():
            Input.mousePosition - myCamera.WorldToScreenPoint(playerCentre.position), false);

    }

    private void Update()
    {
        if (lastHealth != myHealth.health || lastMax != myHealth.maxHealth)
        {
            lastHealth = myHealth.health;
            lastMax = myHealth.maxHealth;
            myUI.HealthChanged(lastHealth, lastMax);
        }

        myUI.WeaponAmmoLeft(weaponHolder.leftWeapon.maxAmmo, weaponHolder.leftWeapon.curAmmo);
        myUI.WeaponAmmoRight(weaponHolder.rightWeapon.maxAmmo, weaponHolder.rightWeapon.curAmmo);
    }

    private void TriggerEndOfRoom()
    {
        foreach(Chip chip in chipsInserted)
        {
            if((BEndChip)chip)
                chip.TriggerAbility();

        }
    }

    private void TriggerOnKill()
    {

    }

    private void TriggerOnDamage()
    {
        
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

        playerInputs.onControlsChanged += SetControlScheme;
    }

    private void UnsetControls()
    {

        dash.performed += Dash;
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

        LevelGenerator.instance.onSpawnRoom.AddListener(TriggerEndOfRoom);

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
        curLegs = legStats.LoadLegs();

        myUI.LockAndLoad(myHealth.maxHealth, myHealth.health,
            weaponHolder.leftWeapon.curAmmo, weaponHolder.rightWeapon.curAmmo,
            legStats.dashRecharge, legStats.dashCharges);
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
    public void SetWeapon(GameObject setWeapon, bool left)
    {

        if (left && weaponHolder.leftWeapon.gameObject != null)
            Destroy(weaponHolder.leftWeapon.gameObject);
        else if (weaponHolder.leftWeapon.gameObject != null)
            Destroy(weaponHolder.rightWeapon.gameObject);

        GameObject genWeapon = GameObject.Instantiate(setWeapon, weaponPoints[left ? 0 : 1]);

        genWeapon.transform.localScale = new Vector3(left ? 1 : -1, 1, 1);

        genWeapon.GetComponent<Weapon>().myController = weaponHolder;

        if (left)
            weaponHolder.leftWeapon = genWeapon.GetComponent<Weapon>();
        else
            weaponHolder.rightWeapon = genWeapon.GetComponent<Weapon>();

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

    private void OnDestroy()
    {

        UnsetControls();

    }
}
