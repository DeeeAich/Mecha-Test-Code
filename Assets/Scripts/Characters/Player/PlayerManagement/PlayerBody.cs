using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerBody : MonoBehaviour, IBodyModifiable
{

    public PlayerLegs myMovement;
    public PlayerWeaponControl weaponHolder;
    private CharacterVFXManager vfxManager;
    public Legs legStats;
    public PlayerUltyControl ultController;
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

    public TriggerEvents triggers;

    public bool isGamepad = true;

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
        //ultController = GetComponent<PlayerUltyControl>();
        myUI = FindObjectOfType<PlayerUI>();
        myHealth = GetComponent<Health>();
        lastHealth = myHealth.health;
        lastMax = myHealth.maxHealth;
        instance = this;
        myHealth.onDeath.AddListener(OnDeath);
        SetControlScheme(playerInputs);
        SceneManager.activeSceneChanged += UnsetChips;
    }

    private void Start()
    {
        
        isGamepad = playerInputs.currentControlScheme.Equals("Controller");
        myCamera = Camera.main;
        SetControls();
        SetHooks();
        LoadStats();
        LoadStartingChips();

    }

    private void FixedUpdate()
    {
        myMovement.Movement( canMove ? move.ReadValue<Vector2>() : new Vector2());
        
        if (canShoot)
            weaponHolder.LookDirection(isGamepad ?
                look.ReadValue<Vector2>():
                Input.mousePosition - myCamera.WorldToScreenPoint(playerCentre.position),
                isGamepad, myCamera.WorldToScreenPoint(playerCentre.position));

        if (canShoot)
        {
            triggers.constant?.Invoke();
        }
    }

    private void Update()
    {
        if (lastHealth != myHealth.health || lastMax != myHealth.maxHealth)
        {
            lastHealth = Mathf.RoundToInt(myHealth.health + myHealth.maxHealth - lastMax);
            lastMax = Mathf.RoundToInt(myHealth.maxHealth);
            myUI.HealthChanged(lastHealth, lastMax);
        }

        if (weaponHolder.leftWeapon != null)
            myUI.WeaponAmmoLeft(weaponHolder.leftWeapon.maxAmmo * weaponHolder.leftWeapon.modifiers.ammoCount, weaponHolder.leftWeapon.curAmmo);
        else
            myUI.WeaponAmmoLeft(100, 0);
        if(weaponHolder.rightWeapon != null)
            myUI.WeaponAmmoRight(weaponHolder.rightWeapon.maxAmmo * weaponHolder.rightWeapon.modifiers.ammoCount, weaponHolder.rightWeapon.curAmmo);
        else
            myUI.WeaponAmmoRight(100, 0);
    }

    public void TriggerOnKill(string killSource)
    {
        if (killSource == "left")
            triggers.killedLeft?.Invoke();
        else if (killSource == "right")
            triggers.killedRight?.Invoke();
    }

    public void TriggerOnDamage()
    {
        PlayerUI.instance.OnHealthChange(false);

        triggers.damaged?.Invoke();
    }

    public void TriggerOnHeal(int amount)
    {
        vfxManager.SpawnHealParticles(-amount);

        triggers.healed?.Invoke();

    }

    public void TriggerOnRoomEnd()
    {
        triggers.roomClear?.Invoke();
    }

    private void Dash(InputAction.CallbackContext context)
    {
        myMovement.StartDash(move.ReadValue<Vector2>());
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

        rightFire.performed += weaponHolder.PressRight;
        rightFire.canceled += weaponHolder.LiftRight;
        rightRe.performed += weaponHolder.ReloadRight;

        interact = playerInputs.actions["Interact"];
        interact.performed += Interact;

        //ultUse = playerInputs.actions["Ultimate"];
        //ultUse.performed += ultController.UseUltimate;
        //ultUse.canceled += ultController.EndUltimate;

        playerInputs.onControlsChanged += SetControlScheme;
    }

    private void UnsetControls()
    {

        dash.performed -= Dash;
        leftFire.performed -= weaponHolder.PressLeft;
        leftFire.canceled -= weaponHolder.LiftLeft;
        leftRe.performed -= weaponHolder.ReloadLeft;
        rightFire.performed -= weaponHolder.PressRight;
        rightFire.canceled -= weaponHolder.LiftRight;
        rightRe.performed -= weaponHolder.ReloadRight;
        interact.performed -= Interact;
        playerInputs.onControlsChanged -= SetControlScheme;
        myHealth.onDeath.RemoveListener(OnDeath);
    }

    private void SetHooks()
    {

        if (LevelGenerator.instance != null)
            LevelGenerator.instance.onSpawnRoom.AddListener(TriggerOnRoomEnd);

    }

    private void Interact(InputAction.CallbackContext context)
    {
        if(curInteract!=null)
            curInteract.TriggerInteraction();

    }

    private void SetControlScheme(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Controller");

        InputDeviceManager.instance.UpdateInputDevice(isGamepad ? InputDevice.PlaystationController : InputDevice.MouseAndKeyboard);

        myUI.SetCursorActive(isGamepad);

        foreach (Pickup pickup in FindObjectsOfType<Pickup>(true))
        {
            pickup.mouseControls = !isGamepad;
            pickup.TrySelectInitialButton();
        }

        //Debug.Log("Control scheme is now " + input.currentControlScheme);
    }

    private void LoadStats()
    {
        myMovement.curLegs = legStats.LoadLegs();

        myUI.LockAndLoad(myHealth.maxHealth, myHealth.health,
            weaponHolder.leftWeapon != null ? weaponHolder.leftWeapon.curAmmo : 0,
            weaponHolder.rightWeapon != null ? weaponHolder.rightWeapon.curAmmo : 0,
            legStats.dashRecharge, legStats.dashCharges,
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
        {
            foreach(WeaponChip chip in weaponHolder.leftMods)
                if(chip.supType == WeaponChip.WeaponSubType.Trigger)
                {
                    WeaponTriggerChip tChip = (WeaponTriggerChip)chip;
                    tChip.ChipTriggerUnsetter(weaponHolder.leftWeapon);
                }
            Destroy(weaponHolder.leftWeapon.gameObject);
        }
        else if (!left && weaponHolder.rightWeapon != null)
        {
            foreach (WeaponChip chip in weaponHolder.rightMods)
                if (chip.supType == WeaponChip.WeaponSubType.Trigger)
                {
                    WeaponTriggerChip tChip = (WeaponTriggerChip)chip;
                    tChip.ChipTriggerUnsetter(weaponHolder.rightWeapon);
                }
            Destroy(weaponHolder.rightWeapon.gameObject);
        }

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

        myUI.StartCoroutine(myUI.StartDeath());

        weaponHolder.leftWeapon.FireRelease();
        weaponHolder.rightWeapon.FireRelease();

        TriggerDebrisExplosion[] explosions = GetComponentsInChildren<TriggerDebrisExplosion>();

        foreach (TriggerDebrisExplosion explosion in explosions)
            explosion.explosionTrigger = true;

        UnsetControls();

        LevelGenerator.instance.onSpawnRoom.RemoveListener(delegate { triggers.roomClear?.Invoke(); });

        UnsetChips();

        triggers.ClearEvents();

    }

    [Tooltip("For setting the parts to on or off")]

    private struct PartStopper
    {
        public bool weapons,
            legs;
    }
    private List<PartStopper> stoppedParts = new();
    public void StopParts(bool weapons, bool legs)
    {

        if(!weapons || !legs)
        {

            print("Adding pause");

            PartStopper newStopper = new();
            newStopper.weapons = weapons;
            newStopper.legs = legs;
            stoppedParts.Add(newStopper);

            if (!weapons)
            {
                weaponHolder.leftWeapon.FireRelease();
                weaponHolder.rightWeapon.FireRelease();
            }

            canShoot = weapons ? canShoot : weapons;
            canMove = legs ? canMove : legs;

        }
        else
        {
            int removeIndex = 50;
            for (int i = 0; i < stoppedParts.Count; i++)
            {
                if (stoppedParts[i].weapons == !weapons && stoppedParts[i].legs == legs ||
                        stoppedParts[i].weapons == weapons && stoppedParts[i].legs == !legs ||
                            stoppedParts[i].weapons == !weapons && stoppedParts[i].legs == !legs)
                {
                    removeIndex = i;
                    break;
                }

            }

            if(removeIndex != 50)
                stoppedParts.RemoveAt(removeIndex);

            print(stoppedParts.Count);
            print(stoppedParts.Count > 0 ? stoppedParts[0].weapons : "");

            if(stoppedParts.Count == 0)
            {
                canMove = true;
                canShoot = true;
            }
            else
            {
                foreach (PartStopper partStopper in stoppedParts)
                {
                    canMove = partStopper.legs ? canMove : false;
                    canShoot = partStopper.weapons ? canShoot : false;
                }
            }

        }

        print(stoppedParts.Count);

    }

    public BodyStats myStats;

    public void ApplyChip(BodyChip chip)
    {

        myMods.Add(chip);
        if(PlayerManager.instance != null)
            PlayerManager.instance.bodyMods.Add(chip);

        switch (chip.bodyType)
        {

            case (BodyChip.BodyType.Stat):
                BStatChip bStatChip = (BStatChip)chip;
                ApplyStats(bStatChip.bodyStats);
                break;
            case (BodyChip.BodyType.Trigger):
                AddTrigger((BodyTriggerChip)chip);
                break;
        }

    }

    public void ApplyStats(BodyStats newStats)
    {

        newStats.AddStats(myStats);

        float healthDif = myHealth.maxHealth - myHealth.health;
        myHealth.maxHealth *= myStats.health;
        myHealth.health = myHealth.maxHealth - healthDif;

    }

    public void RemoveStats(BodyStats newStats)
    {

        

    }

    public void AddTrigger(BodyTriggerChip chip)
    {
        chip.ChipTriggerSetter();

    }

    public void Preform(IEnumerator corout)
        => StartCoroutine(corout);

    public void End(IEnumerator corout)
        => StopCoroutine(corout);

    private void OnDestroy()
    {

        UnsetControls();
        UnsetChips();
        triggers.ClearEvents();

    }

    private void LoadStartingChips()
    {

        foreach (BodyChip bodyChip in myMods)
        {
            BodyTriggerChip bodyTriggerChip = (BodyTriggerChip)bodyChip;
            bodyTriggerChip.ChipTriggerUnsetter();
        }

        List<BodyChip> setChips = myMods;

        myMods = new();

        foreach (BodyChip chip in setChips)
            ApplyChip(chip);

    }

    public void UnsetChips()
    {

        print("Unsetting Chips");

        foreach (BodyChip chip in myMods)
        {

            if(chip.bodyType == BodyChip.BodyType.Trigger)
            {
                BodyTriggerChip bodyTriggerChip = (BodyTriggerChip)chip;
                bodyTriggerChip.ChipTriggerUnsetter();
            }

        }

        foreach (WeaponChip chip in weaponHolder.leftMods)
        {
            if(chip.supType == WeaponChip.WeaponSubType.Trigger)
            {
                WeaponTriggerChip weaponTriggerChip = (WeaponTriggerChip)chip;
                weaponTriggerChip.ChipTriggerUnsetter(weaponHolder.leftWeapon);
            }
        }
        foreach (WeaponChip chip in weaponHolder.rightMods)
        {
            if (chip.supType == WeaponChip.WeaponSubType.Trigger)
            {
                WeaponTriggerChip weaponTriggerChip = (WeaponTriggerChip)chip;
                weaponTriggerChip.ChipTriggerUnsetter(weaponHolder.rightWeapon);
            }
        }

        foreach (MovementChip chip in myMovement.legChips)
        {

            if (chip.moveType == MovementChip.MovementType.Trigger)
            {
                MovementTriggerChip movementTriggerChip = (MovementTriggerChip)chip;
                movementTriggerChip.ChipTriggerUnsetter();
            }

        }
        triggers.ClearEvents();
        SceneManager.activeSceneChanged -= UnsetChips;
    }
    private void UnsetChips(Scene sceneUnloaded, Scene sceneLoaded)
    {

        print("Unsetting Chips");

        foreach (BodyChip chip in myMods)
        {

            if (chip.bodyType == BodyChip.BodyType.Trigger)
            {
                BodyTriggerChip bodyTriggerChip = (BodyTriggerChip)chip;
                bodyTriggerChip.ChipTriggerUnsetter();
            }

        }

        foreach (WeaponChip chip in weaponHolder.leftMods)
        {
            if (chip.supType == WeaponChip.WeaponSubType.Trigger)
            {
                WeaponTriggerChip weaponTriggerChip = (WeaponTriggerChip)chip;
                weaponTriggerChip.ChipTriggerUnsetter(weaponHolder.leftWeapon);
            }
        }
        foreach (WeaponChip chip in weaponHolder.rightMods)
        {
            if (chip.supType == WeaponChip.WeaponSubType.Trigger)
            {
                WeaponTriggerChip weaponTriggerChip = (WeaponTriggerChip)chip;
                weaponTriggerChip.ChipTriggerUnsetter(weaponHolder.rightWeapon);
            }
        }

        foreach (MovementChip chip in myMovement.legChips)
        {

            if (chip.moveType == MovementChip.MovementType.Trigger)
            {
                MovementTriggerChip movementTriggerChip = (MovementTriggerChip)chip;
                movementTriggerChip.ChipTriggerUnsetter();
            }

        }
        triggers.ClearEvents();
        SceneManager.activeSceneChanged -= UnsetChips;
    }

    private void OnApplicationQuit()
    {

        UnsetControls();

    }
}
