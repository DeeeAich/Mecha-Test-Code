using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BomberLean : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] float leanSpeed;
    // Start is called before the first frame update
    void Start()
    {
        if (agent == null)
            agent = GetComponentInParent<NavMeshAgent>();
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentLean = anim.GetFloat("Lean Amount");
        float targetLean;
        Vector3 targetDir = agent.steeringTarget.normalized;
        float turnVal = Vector3.Dot(targetDir, transform.parent.forward); //1 is no turn, -1 is max turn
        float rightlyness = Vector3.Dot(targetDir, transform.parent.right); //+ means turn right (have sign of Lean Amount match)
        targetLean = Mathf.Acos(turnVal)/Mathf.PI;
        if (Mathf.Abs(rightlyness) <= 0.01f)
        {
            targetLean = 0;
        }
        else
        {
            rightlyness /= Mathf.Abs(rightlyness);
            targetLean *= rightlyness;
        }
        anim.SetFloat("Lean Amount", Mathf.Lerp(currentLean, targetLean, leanSpeed));
    }
}
