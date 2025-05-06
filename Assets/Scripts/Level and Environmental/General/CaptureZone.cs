using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class CaptureZone : MonoBehaviour
{
    public UnityEvent onCapture;

    public bool isCaptured;
    public float captureProgress;
    public float captureProgressPerSecond = 0.01f;
    public Image capturePorgressImage;

    public bool playerPresent;
    public int enemiesPresent;

    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask characterLayers;


    private void FixedUpdate()
    {
        animator.SetBool("Charging", playerPresent);
        animator.SetBool("EnemyOnPoint", enemiesPresent > 0);

        if (playerPresent && enemiesPresent == 0)
        {
            captureProgress += captureProgressPerSecond * Time.fixedDeltaTime;
            capturePorgressImage.fillAmount = captureProgress;
        }

        if (captureProgress >= 1 && !isCaptured)
        {
            isCaptured = true;
            animator.SetTrigger("Complete");
            onCapture.Invoke();
        }

        enemiesPresent = 0;
        playerPresent = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerBody>()) playerPresent = true;

        if (other.gameObject.layer == 9) enemiesPresent++;
    }
}
