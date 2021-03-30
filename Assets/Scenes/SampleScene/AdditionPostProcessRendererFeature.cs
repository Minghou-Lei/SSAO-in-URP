using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AdditionPostProcessRendererFeature : ScriptableRendererFeature{
		
    //public Shader shader = Shader.Find("Custom/MySSAO");
    public Shader shader;
    public Material _Material;
    public MySSAOVolume volume;
		
		
    public override void Create(){
        volume = new MySSAOVolume();
        volume.renderPassEvent = RenderPassEvent.AfterRendering;
        volume.m_TemporaryColorTexture01.Init("_ScreenSpaceOcclusionTexture");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData){
        // 检测Shader是否存在
        if (shader == null)
            return;

        // 创建材质
        if (_Material==null)
            _Material = CoreUtils.CreateEngineMaterial(shader);

        // 获取当前渲染相机的目标颜色，也就是主纹理
        var cameraColorTarget = renderer.cameraColorTarget;

        // 设置调用后处理Pass
        volume.Setup(cameraColorTarget, _Material);
            
        // 添加该Pass到渲染管线中
        renderer.EnqueuePass(volume);
    }
    
    
}
