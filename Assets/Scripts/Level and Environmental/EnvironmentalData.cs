using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalData : MonoBehaviour
{
    static EnvironmentalData instance;
    [SerializeField] private Vector2 windXOffset, windZOffset;
    [SerializeField] private Vector2 windXOffsetDelta, windZOffsetDelta;
    [SerializeField] float windAreaScale;
    [SerializeField] float sunRotateDPS = 1f;

    public static EnvironmentalData Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            else
            {
                Debug.LogError("No Sunlight Installed, instantiating");
                Light[] lights = FindObjectsOfType<Light>();
                foreach (Light l in lights)
                {
                    if (l.type == LightType.Directional)
                    {
                        if (!l.gameObject.TryGetComponent<EnvironmentalData>(out instance))
                        {
                            instance = l.gameObject.AddComponent<EnvironmentalData>();
                        }
                        return instance;
                    }
                }
                Debug.LogError("No Directional Light in Scene for Sunlight to attach to");
                GameObject gO = new();
                Light light = gO.AddComponent<Light>();
                light.type = LightType.Directional;
                instance = gO.AddComponent<EnvironmentalData>();
                return instance;
            }
        }
        private set
        {
            if (instance != null)
            {
                instance = value;
            }
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        windXOffset += windXOffsetDelta * Time.deltaTime;
        windZOffset += windZOffsetDelta * Time.deltaTime;
        //Vector3 tempRot = transform.rotation.eulerAngles;
        //transform.rotation = Quaternion.AngleAxis(sunRotateDPS*Time.deltaTime, Vector3.up) * transform.rotation;
    }

    internal Vector3 GetWind(Vector3 pos)
    {
        float x, z;
        x = Mathf.PerlinNoise((windXOffset.x + pos.x) * windAreaScale, (windXOffset.y + pos.z) * windAreaScale);
        z = Mathf.PerlinNoise((windZOffset.x + pos.x) * windAreaScale, (windZOffset.y + pos.z) * windAreaScale);

        x -= 0.5f;
        z -= 0.5f;

        x *= 2;
        z *= 2;

        x = Mathf.Clamp(x, -1, 1);
        z = Mathf.Clamp(z, -1, 1);

        return new Vector3(x, 0, z);
    }
}
