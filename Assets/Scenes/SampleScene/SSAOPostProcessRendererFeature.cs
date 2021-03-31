using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SSAOPostProcessRendererFeature : ScriptableRendererFeature{
		
    //public Shader shader = Shader.Find("Custom/MySSAO");
    public Shader shader;
    public Material _Material;
    public MySSAOVolume volume;
    public int samplePointCount = 32;
    public List<Vector4> samplePoint;
		
		
    public override void Create(){
        volume = new MySSAOVolume();
        volume.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        volume.m_TemporaryColorTexture01.Init("_ScreenSpaceOcclusionTexture");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (shader == null)
            return;
        if (_Material == null)
            _Material = CoreUtils.CreateEngineMaterial(shader);
        var cameraColorTarget = renderer.cameraColorTarget;
        GenSampleKernal();
        volume.Setup(cameraColorTarget, _Material,samplePoint);
        renderer.EnqueuePass(volume);
    }
    
    void GenSampleKernal()
    {
        if (samplePointCount == samplePoint.Count)
            return;
        samplePoint.Clear();
        for (int i = 0; i < samplePointCount; ++i)
        {
            var vector = new Vector4(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(0f, 1f), 1f).normalized;
            //拟合二次方程
            var scale = (float) i / samplePointCount;
            scale = Mathf.Lerp(0.01f, 1f, scale * scale);
            vector *= scale;
            samplePoint.Add(vector);
        }
    }
}
