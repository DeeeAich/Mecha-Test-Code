using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeModifier : MonoBehaviour
{
    [SerializeField] internal static CinemachineVirtualCamera absoluteCinemachine;
    [SerializeField] float testVal;
    [SerializeField] bool test;
    [SerializeField] bool resetTest;
    [SerializeField] float speed;
    float rememberedLensSize;
    float currentSize;
    float targetSize;
    // Start is called before the first frame update
    void Start()
    {
        absoluteCinemachine = GetComponent<CinemachineVirtualCamera>();
        rememberedLensSize = absoluteCinemachine.m_Lens.OrthographicSize;
        currentSize = rememberedLensSize;
        targetSize = rememberedLensSize;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(test)
        {
            ChangeCameraSize(testVal);
            test = false;
        }
        if(resetTest)
        {
            ResetCameraSize();
            resetTest = false;
        }
        if(currentSize == targetSize)
        {
            return;
        }
        if(Mathf.Abs(currentSize - targetSize) < 0.01f)
        {
            currentSize = targetSize;
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * speed);
        }


        absoluteCinemachine.m_Lens.OrthographicSize = currentSize;
    }

    public void ChangeCameraSize(float newSize)
    {
        targetSize = newSize;
    }

    public void ResetCameraSize()
    {
        targetSize = rememberedLensSize;
    }
}
