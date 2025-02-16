using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum effect
{
    Burn,
    Hack,
    ShortCircuit
}
public class CharacterVFXManager : MonoBehaviour
{
    [Header("Burn")]
    public List<GameObject> burnObjects;
    public bool testBurn;
    public Material burnMaterial;
    public List<MeshRenderer> meshRenderers;


    [Header("Hack")]
    public List<GameObject> hackObjects;
    public bool testHack;

    [Header("Short Circuit")]
    public List<GameObject> shortCircuitObjects;

    private void Start()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            meshRenderers.Add(GetComponent<MeshRenderer>());
        }

        // get mesh renderers childed to main object
        MeshRenderer[] childedMeshRenderers;
        childedMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < childedMeshRenderers.Length; i++)
        {
            meshRenderers.Add(childedMeshRenderers[i]);
        }
    }

    private void Update()
    {
        if (testBurn)
        {
            ToggleEffectVFX(effect.Burn, true);
            testBurn = !testBurn;
        }
        if (testHack)
        {
            ToggleEffectVFX(effect.Burn, false);
            testHack = !testHack;
        }
    }

    public void ToggleEffectVFX(effect effect, bool isOn)
    {
        switch (effect)
        {
            case effect.Burn:
                ToggleBurn(isOn);
                break;

            case effect.Hack:
                ToggleHack(isOn);
                break;

            case effect.ShortCircuit:
                ToggleShortCircuit(isOn);
                break;
        }
    }


    void ToggleBurn(bool isOn)
    {
        
        for (int i = 0; i < burnObjects.Count; i++)
        {
            if(burnObjects[i] != null) burnObjects[i].SetActive(isOn);
        }
        


        if (isOn)
        {
            if (GetComponent<MeshRenderer>() != null)
            {
                meshRenderers.Add(GetComponent<MeshRenderer>());
            }

            // get mesh renderers childed to main object
            MeshRenderer[] childedMeshRenderers;
            childedMeshRenderers = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < childedMeshRenderers.Length; i++)
            {
                meshRenderers.Add(childedMeshRenderers[i]);
            }

            SetAdditionalMaterial(burnMaterial);
        }
        else
        {
            ClearAdditionalMaterial(burnMaterial);
        }
    }




    void ToggleHack(bool isOn)
    {
        for (int i = 0; i < hackObjects.Count; i++)
        {
            if(hackObjects[i] != null) hackObjects[i].SetActive(isOn);
        }
    }

    void ToggleShortCircuit(bool isOn)
    {
        for (int i = 0; i < shortCircuitObjects.Count; i++)
        {
            if(shortCircuitObjects[i] != null) shortCircuitObjects[i].SetActive(isOn);
        }
    }  
    
    
    
    
    public void SetAdditionalMaterial(Material mat)
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            if (meshRenderers[i] == null) continue;
            
            List<Material> materials = meshRenderers[i].materials.ToList();
            materials.Add(mat);
            meshRenderers[i].materials = materials.ToArray();
        }
    }
    public void ClearAdditionalMaterial(Material mat)
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            if (meshRenderers[i] == null) continue;
            
            List<Material> materials = meshRenderers[i].materials.ToList();
            if (materials.Contains(mat)) materials.Remove(mat);
            meshRenderers[i].materials = materials.ToArray();
        }
    }
}
