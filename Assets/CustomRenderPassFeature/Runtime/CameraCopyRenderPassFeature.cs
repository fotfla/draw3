using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraCopyRenderPassFeature : ScriptableRendererFeature
{
    class CopyRenderPass : ScriptableRenderPass
    {
        ProfilingSampler _profilingSampler = new ProfilingSampler("Copy");
        RTHandle source;
        RTHandle texture;

        Material material;

        public void Setup(RTHandle source, RenderTexture texture, Material material, in RenderingData renderingData)
        {
            this.source = source;
            this.texture = RTHandles.Alloc(texture);
            this.material = material;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (texture == null) return;


            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                var s = renderingData.cameraData.renderer.cameraColorTargetHandle;
                if (material == null)
                {
                    Blitter.BlitCameraTexture(cmd, s, texture);
                }
                else
                {
                    Blitter.BlitCameraTexture(cmd, s, texture, material, 0);
                }
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    CopyRenderPass m_ScriptablePass;
    public RenderTexture texture;
    public Material material;

    public override void Create()
    {
        m_ScriptablePass = new CopyRenderPass();
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_ScriptablePass.Setup(renderer.cameraColorTargetHandle, texture, material, renderingData);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


