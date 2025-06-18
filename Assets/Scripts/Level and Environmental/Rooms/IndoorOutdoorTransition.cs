using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorOutdoorTransition : MonoBehaviour
{
    [Header("Meshes")]
    public MeshRenderer[] myMeshRenderer;
    public float myTransparency = 1f;

    [Header("Lights")]
    public Light[] myLights;
    public LightInfo[] myLightInfo;
    public struct LightInfo
    {
        public Light myLight;
        public float myInitialIntensity;
    }


    [Header("Room State")]
    public bool isTransparent;
    public bool fadingIn;
    public bool fadingOut;
    public float fadeSpeed = 5f;

    public float lerpTimer;

    private void Start()
    {
        myMeshRenderer = GetComponentsInChildren<MeshRenderer>();
        myLights = GetComponentsInChildren<Light>();

        myLightInfo = new LightInfo[myLights.Length];
        for (int i = 0; i < myLights.Length; i++)
        {
            myLightInfo[i].myLight = myLights[i];
            myLightInfo[i].myInitialIntensity = myLights[i].intensity;

            if (isTransparent) { myLightInfo[i].myLight.intensity = 0; }
        }

        if (isTransparent)
        {
            myTransparency = 0;
        }
        else
        {
            myTransparency = 1;
        }
        FadeMyMaterials();
    }

    public void FixedUpdate()
    {
        lerpTimer += Time.fixedDeltaTime * fadeSpeed;

        if (fadingIn)
        {
            myTransparency = Mathf.Lerp(0, 1, lerpTimer);
            FadeMyMaterials();

            for (int i = 0; i < myLightInfo.Length; i++)
            {
                myLightInfo[i].myLight.intensity = Mathf.Lerp(0, myLightInfo[i].myInitialIntensity, lerpTimer);       
            }

            if (myTransparency >= 1)
            {
                myTransparency = 1;
                isTransparent = false;
                fadingIn = false;
            }
        }
        
        if (fadingOut)
        {
            myTransparency = Mathf.Lerp(1, 0, lerpTimer);
            FadeMyMaterials();

            for (int i = 0; i < myLightInfo.Length; i++)
            {
                myLightInfo[i].myLight.intensity = Mathf.Lerp(myLightInfo[i].myInitialIntensity, 0, lerpTimer);
            }

            if (myTransparency <= 0)
            {
                myTransparency = 0;
                isTransparent = true;
                fadingOut = false;
            }
        }

    }

    public void TransitionRoom()
    {
        lerpTimer = 0;
        if (!isTransparent) { fadingOut = true; }
        else { fadingIn = true; }
    }
    public void FadeMyMaterials()
    {
        for (int i = 0; i < myMeshRenderer.Length; i++)
        {
            myMeshRenderer[i].material.SetFloat("_Transparency", myTransparency);
        }
    }
}
