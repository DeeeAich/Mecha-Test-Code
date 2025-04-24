using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionSetter : MonoBehaviour
{
    public GameObject targetPosition;

    public void SetPlayerAtPosition()
    {
        PlayerBody.Instance().transform.position = targetPosition.transform.position;
    }
}
