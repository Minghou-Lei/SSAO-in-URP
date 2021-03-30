using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MySSAOVolume : ScriptableRenderPass{
	const string CommandBufferTag = "AdditionalPostProcessing Pass";
	public Material m_Material;
	MySSAOVolumeComponent m_SSAOVolumeComponent;
	RenderTargetIdentifier m_ColorAttachment;
	RenderTargetHandle m_TemporaryColorTexture01;
	public List<Vector4> samplePoint = new List<Vector4>();
	public float samplePointCount = 32;


	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData){
		var stack = VolumeManager.instance.stack;
		m_SSAOVolumeComponent = stack.GetComponent<MySSAOVolumeComponent>();
		samplePointCount = m_SSAOVolumeComponent._SamplePointCount.value;
		GenSampleKernal();
		Camera.main.depthTextureMode |= DepthTextureMode.DepthNormals;
		var cmd = CommandBufferPool.Get(CommandBufferTag);

		Render(cmd, ref renderingData);

		context.ExecuteCommandBuffer(cmd);
		CommandBufferPool.Release(cmd);
		cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
	}

	public void Setup(RenderTargetIdentifier _ColorAttachment, Material Material){
		this.m_ColorAttachment = _ColorAttachment;
		m_Material = Material;
		samplePoint = new List<Vector4>();
	}

	void Render(CommandBuffer cmd, ref RenderingData renderingData){
		if (m_SSAOVolumeComponent.IsActive() && !renderingData.cameraData.isSceneViewCamera){
			RenderTexture AO = new RenderTexture(Screen.width,Screen.height,0);
			RenderTexture blur = new RenderTexture(Screen.width,Screen.height,0);
			GenSampleKernal();
			Camera.main.depthTextureMode |= DepthTextureMode.DepthNormals;


			// 获取目标相机的描述信息
			RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
			// 设置深度缓冲区
			opaqueDesc.depthBufferBits = 0;
			// 通过目标相机的渲染信息创建临时缓冲区
			cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc);
			cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier(), m_Material);
			
			m_Material.SetTexture("_NoiseTex",m_SSAOVolumeComponent._NoiseTex.value);
			m_Material.SetMatrix("_InverseProjectionMatrix", Camera.main.projectionMatrix.inverse);
			m_Material.SetVectorArray("_SamplePointArray",samplePoint.ToArray());
			m_Material.SetFloat("_SamplePointCount",m_SSAOVolumeComponent._SamplePointCount.value);
			m_Material.SetFloat("_SampleKernelRadius",m_SSAOVolumeComponent._SampleKernelRadius.value);
			m_Material.SetFloat("_Strength",m_SSAOVolumeComponent._Strength.value);
			m_Material.SetFloat("_Bias",m_SSAOVolumeComponent._Bias.value);
			cmd.Blit(m_ColorAttachment,AO,m_Material,0);
			
			m_Material.SetFloat("_BilaterFilterFactor",1f-m_SSAOVolumeComponent._BilaterFilterFactor.value);
			m_Material.SetVector("_BlurRadius",new Vector4(m_SSAOVolumeComponent._BlurRadius.value,0,0,0));
			cmd.Blit(AO,blur,m_Material,1);
			
			m_Material.SetVector("_BlurRadius",new Vector4(0,m_SSAOVolumeComponent._BlurRadius.value,0,0));
			cmd.Blit(blur,AO,m_Material,1);

			Shader.SetGlobalTexture("_ScreenSpaceOcclusionTexture",AO);
			
			cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment);
		}
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