using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleButton : MonoBehaviour
{
    public RectTransform RectTransform;
    public int code = 1;
    public bool isCenter;

    private void OnEnable()
    {
        GetComponent<Button>().Select();
    }

    public void RotateSymbol()
    {
        RectTransform.localEulerAngles += new Vector3(0, 0, 45);
        code++;
        if(code > 4)
        {
            code = 1;
        }
        
    }
    public void ResetSymbol()
    {
        RectTransform.localEulerAngles = new Vector3(0, 0, 0);
        code = 1;
    }
}
