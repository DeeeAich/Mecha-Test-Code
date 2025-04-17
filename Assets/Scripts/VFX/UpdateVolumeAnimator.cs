using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UpdateVolumeAnimator : MonoBehaviour
{

    public Volume volume;
    LensDistortion lensDistortion;
    ChromaticAberration chromaticAberration;
    Bloom thisbloom;
    public float lens;
    public float lensScale;
    public float chrome;
    public float bloom;

    private void Start()
    {
        VolumeProfile proflile = volume.sharedProfile;
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out thisbloom);

    }

    private void FixedUpdate()
    {
        lensDistortion.intensity.value = lens;
        lensDistortion.scale.value = lensScale;
        chromaticAberration.intensity.value = chrome;
        thisbloom.intensity.value = bloom;
    }
}
