using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUltyControl : MonoBehaviour
{

    PlayerBody myBody;
    public Ultimate currentUltimate;

    private void Start()
    {
        myBody = GetComponent<PlayerBody>();
        currentUltimate = myBody.ultimate;
    }

    public void UseUltimate()
    {

        currentUltimate.ActivateUltimate();

    }

    public void EndUltimate()
    {

        currentUltimate.EndUltimate();

    }

    private void FixedUpdate()
    {

        currentUltimate.UltUpdate();

    }
}
