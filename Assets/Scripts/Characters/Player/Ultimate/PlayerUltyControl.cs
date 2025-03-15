using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUltyControl : MonoBehaviour
{

    PlayerBody myBody;
    public Ultimate currentUltimate;
    public GameObject ultimateCaster;
    private Animator ultAnimation;

    public Transform firePoint;

    public static PlayerUltyControl instance;

    public bool recharging = false;

    private void Start()
    {
        myBody = PlayerBody.Instance();

        instance = this;

        LoadUltimate(currentUltimate);

    }

    public void UseUltimate(InputAction.CallbackContext context)
    {
        if (!recharging)
            currentUltimate.ActivateUltimate();

    }

    public void EndUltimate(InputAction.CallbackContext context)
    {
        
        currentUltimate.EndUltimate();

    }

    private void FixedUpdate()
    {

        currentUltimate.UltUpdate();

    }

    private void LoadUltimate(Ultimate newUlt)
    {

        currentUltimate = newUlt;

        ultimateCaster = GameObject.Instantiate(currentUltimate.ultCaster, transform.GetChild(0));

        ultAnimation = ultimateCaster.GetComponentInChildren<Animator>();

    }

    public void RunAnimation(string animationTrigger)
    {

        ultAnimation.SetTrigger(animationTrigger);

    }
}
