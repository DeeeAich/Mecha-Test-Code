using System.Collections;
using System.Collections.Generic;
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

    [Header("Hack")]
    public List<GameObject> hackObjects;
    public bool testHack;

    [Header("Short Circuit")]
    public List<GameObject> shortCircuitObjects;
    public bool testShortCircuit;

    private void Update()
    {
        if (testBurn)
        {
            ToggleEffectVFX(effect.Burn, true);
            testBurn = !testBurn;
        }
        if (testHack)
        {
            ToggleEffectVFX(effect.Hack, true);
            testHack = !testHack;
        }
        if (testShortCircuit)
        {
            ToggleEffectVFX(effect.ShortCircuit, true);
            testShortCircuit = !testShortCircuit;
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
            burnObjects[i].SetActive(isOn);
        }
    }

    void ToggleHack(bool isOn)
    {
        for (int i = 0; i < hackObjects.Count; i++)
        {
            hackObjects[i].SetActive(isOn);
        }
    }

    void ToggleShortCircuit(bool isOn)
    {
        for (int i = 0; i < shortCircuitObjects.Count; i++)
        {
            shortCircuitObjects[i].SetActive(isOn);
        }
    }
}
