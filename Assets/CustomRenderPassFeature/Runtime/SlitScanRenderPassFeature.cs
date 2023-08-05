using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlitScanRenderPassFeature : ScriptableRendererFeature
{
    class SlitScanRenderPass : ScriptableRenderPass
    {
        ProfilingSampler _profilingSampler = new ProfilingSampler("SlitScan");

        RTHandle source;
        RTHandle destination;

        SlitScan slitScan;

        Material material;

        private readonly int DestProp = Shader.PropertyToID("_Dest");
        private readonly int IntensityProp = Shader.PropertyToID("_Intensity");
        private readonly int SplitProp = Shader.PropertyToID("_Split");
        private readonly int SpeedProp = Shader.PropertyToID("_Speed");

        public void Setup(RTHandle source, in RenderingData renderingData)
        {
            material = CoreUtils.CreateEngineMaterial("Hidden/Shader/SlitScan");
            this.source = source;

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = (int)DepthBits.None;
            RenderingUtils.ReAllocateIfNeeded(ref destination, desc, name: "_SlitScanDesc");

            slitScan = VolumeManager.instance.stack.GetComponent<SlitScan>();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (slitScan != null && slitScan.IsActive())
            {
                var cmd = CommandBufferPool.Get();
                using (new ProfilingScope(cmd, _profilingSampler))
                {
                    material.SetTexture(DestProp, destination);
                    material.SetFloat(SplitProp, slitScan.split.value);
                    material.SetFloat(SpeedProp, slitScan.speed.value);
                    var s = renderingData.cameraData.renderer.cameraColorTargetHandle;
                    Blitter.BlitCameraTexture(cmd, s, s, material, 0);
                    Blitter.BlitCameraTexture(cmd, s, destination);
                }
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
        }

        void Dispose()
        {
            CoreUtils.Destroy(material);
            destination?.Release();
        }
    }

    SlitScanRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new SlitScanRenderPass();
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_ScriptablePass.Setup(renderer.cameraColorTargetHandle, renderingData);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


