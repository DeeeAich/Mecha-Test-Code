using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBody : MonoBehaviour
{

    private PlayerLegs myMovement;
    private PlayerWeaponControl weaponHolder;
    public Legs legStats;
    public Ultimate ultimate;
    public List<ChipBasic> chipsInserted;
    public LegInfo curLegs;
    private PlayerInput playerInputs;
    public Camera myCamera;
    public Transform playerCentre;

    [SerializeField] List<Transform> weaponPoints = new();

    //input actions
    private InputAction move, look, dash,
        ultUse, leftFire, rightFire,
        leftRe, rightRe, interact;

    private bool isGamepad = true;

    private void Awake()
    {
        playerInputs = GetComponent<PlayerInput>();
        myMovement = GetComponent<PlayerLegs>();
        weaponHolder = GetComponent<PlayerWeaponControl>();
    }

    private void Start()
    {
        
        isGamepad = playerInputs.currentControlScheme.Equals("Controller");
        SetControls();
        LoadStats();
    }

    private void FixedUpdate()
    {

        myMovement.Movement(move.ReadValue<Vector2>());

        weaponHolder.LookDirection(isGamepad ?
            look.ReadValue<Vector2>():
            Input.mousePosition - myCamera.WorldToScreenPoint(playerCentre.position));

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

        playerInputs.onControlsChanged += SetControlScheme;
    }

    private void SetControlScheme(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Controller");

        Debug.Log("Control scheme is now " + input.currentControlScheme);
    }

    private void LoadStats()
    {
        curLegs = legStats.LoadLegs();
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

        if (left)
            weaponHolder.leftWeapon = genWeapon.GetComponent<Weapon>();
        else
            weaponHolder.rightWeapon = genWeapon.GetComponent<Weapon>();

    }

}
