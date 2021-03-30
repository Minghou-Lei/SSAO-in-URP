using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MySSAOVolumeComponent : VolumeComponent,IPostProcessComponent
{
    public ClampedFloatParameter _SamplePointCount = new ClampedFloatParameter(0f, 0, 32);
    public ClampedFloatParameter _SampleKernelRadius = new ClampedFloatParameter(0f, 0, 3);
    public ClampedFloatParameter _BilaterFilterFactor = new ClampedFloatParameter(0f, 0, 0.2f);
    public ClampedFloatParameter _BlurRadius = new ClampedFloatParameter(0f, 0, 4);
    public ClampedFloatParameter _Bias = new ClampedFloatParameter(0f, 0, 0.0001f);
    public ClampedFloatParameter _Strength = new ClampedFloatParameter(0f, 0, 500);
    public TextureParameter _NoiseTex = new TextureParameter(null,true);
    public bool IsActive(){
        return base.active;
    }

    public bool IsTileCompatible(){
        throw new System.NotImplementedException();
    }
}
