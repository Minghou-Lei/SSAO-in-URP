using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MySSAOVolume : ScriptableRenderPass
{
    const string CommandBufferTag = "SSAO PostProcessing Pass";
    public Material m_Material;
    MySSAOVolumeComponent m_SSAOVolumeComponent;
    RenderTargetIdentifier m_ColorAttachment;
    public RenderTargetHandle m_TemporaryColorTexture01;
    public List<Vector4> samplePoint = new List<Vector4>();
    public float samplePointCount = 32;
    private RenderTextureDescriptor opaqueDesc;
    private RenderTexture AO, blur,output;

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var stack = VolumeManager.instance.stack;
        m_SSAOVolumeComponent = stack.GetComponent<MySSAOVolumeComponent>();
        samplePointCount = m_SSAOVolumeComponent._SamplePointCount.value;
        opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;
        //GenSampleKernal();
        var cmd = CommandBufferPool.Get(CommandBufferTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
        cmd.Clear();
    }

    public void Setup(RenderTargetIdentifier _ColorAttachment, Material Material, List<Vector4> sp)
    {
        this.m_ColorAttachment = _ColorAttachment;
        m_Material = Material;
        samplePoint = new List<Vector4>();
        samplePoint = sp;
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (m_SSAOVolumeComponent.IsActive() && !renderingData.cameraData.isSceneViewCamera)
        {
            AO = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            blur = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            output = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc);
            m_Material.SetTexture("_NoiseTex", m_SSAOVolumeComponent._NoiseTex.value);
            m_Material.SetVectorArray("_SamplePointArray", samplePoint.ToArray());
            m_Material.SetFloat("_SamplePointCount", m_SSAOVolumeComponent._SamplePointCount.value);
            m_Material.SetFloat("_SampleKernelRadius", m_SSAOVolumeComponent._SampleKernelRadius.value);
            m_Material.SetFloat("_Strength", m_SSAOVolumeComponent._Strength.value);
            m_Material.SetFloat("_Bias", m_SSAOVolumeComponent._Bias.value);
            m_Material.SetTexture("_MainTex", RenderTexture.active);
            cmd.Blit(m_TemporaryColorTexture01.id, AO, m_Material, 0);

            m_Material.SetFloat("_BilaterFilterFactor", 1f - m_SSAOVolumeComponent._BilaterFilterFactor.value);
            m_Material.SetVector("_BlurRadius", new Vector4(m_SSAOVolumeComponent._BlurRadius.value, 0, 0, 0));
            m_Material.SetTexture("_MainTex", AO);
            cmd.Blit(AO, blur, m_Material, 1);
            m_Material.SetVector("_BlurRadius", new Vector4(0, m_SSAOVolumeComponent._BlurRadius.value, 0, 0));
            m_Material.SetTexture("_MainTex", blur);
            cmd.Blit(blur, output, m_Material, 1);
            cmd.SetGlobalTexture("_ScreenSpaceOcclusionTexture", output);
            RenderTexture.ReleaseTemporary(output);
            RenderTexture.ReleaseTemporary(blur);
            RenderTexture.ReleaseTemporary(AO);

        }

        if (renderingData.cameraData.isSceneViewCamera)
        {

            AO = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            blur = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            output = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc);
            m_Material.SetTexture("_NoiseTex", m_SSAOVolumeComponent._NoiseTex.value);
            m_Material.SetVectorArray("_SamplePointArray", samplePoint.ToArray());
            m_Material.SetFloat("_SamplePointCount", m_SSAOVolumeComponent._SamplePointCount.value);
            m_Material.SetFloat("_SampleKernelRadius", m_SSAOVolumeComponent._SampleKernelRadius.value);
            m_Material.SetFloat("_Strength", m_SSAOVolumeComponent._Strength.value);
            m_Material.SetFloat("_Bias", m_SSAOVolumeComponent._Bias.value);
            m_Material.SetTexture("_MainTex", RenderTexture.active);
            cmd.Blit(m_TemporaryColorTexture01.id, AO, m_Material, 0);

            m_Material.SetFloat("_BilaterFilterFactor", 1f - m_SSAOVolumeComponent._BilaterFilterFactor.value);
            m_Material.SetVector("_BlurRadius", new Vector4(m_SSAOVolumeComponent._BlurRadius.value, 0, 0, 0));
            m_Material.SetTexture("_MainTex", AO);
            cmd.Blit(AO, blur, m_Material, 1);
            m_Material.SetVector("_BlurRadius", new Vector4(0, m_SSAOVolumeComponent._BlurRadius.value, 0, 0));
            m_Material.SetTexture("_MainTex", blur);
            cmd.Blit(blur, output, m_Material, 1);
            cmd.SetGlobalTexture("_ScreenSpaceOcclusionTexture", output);
            RenderTexture.ReleaseTemporary(blur);
            RenderTexture.ReleaseTemporary(AO);
            RenderTexture.ReleaseTemporary(output);
        }

    }
    
    public override void FrameCleanup(CommandBuffer cmd)

    {
        if (m_TemporaryColorTexture01 != RenderTargetHandle.CameraTarget)
        {
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            m_TemporaryColorTexture01 = RenderTargetHandle.CameraTarget;
        }
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, cameraTextureDescriptor, FilterMode.Point);
        ConfigureTarget(m_TemporaryColorTexture01.Identifier());
        ConfigureClear(ClearFlag.All, Color.black);
    }
}