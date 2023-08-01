using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FeedBackRenderPassFeature : ScriptableRendererFeature
{
    class FeedBackRenderPass : ScriptableRenderPass
    {
        ProfilingSampler _profilingSampler = new ProfilingSampler("FeedBack");

        RTHandle source;
        RTHandle destination;
        RTHandle copy;

        FeedBack feedBack;

        Material material;
        private readonly int DestProp = Shader.PropertyToID("_Dest");
        private readonly int IntensityProp = Shader.PropertyToID("_Intensity");

        public void Setup(RTHandle source, in RenderingData renderingData)
        {
            material = CoreUtils.CreateEngineMaterial("Hidden/Shader/FeedBack");
            this.source = source;

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = (int)DepthBits.None;
            RenderingUtils.ReAllocateIfNeeded(ref destination, desc, name: "_Dest");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            feedBack = stack.GetComponent<FeedBack>();

            if (feedBack.IsActive())
            {
                var cmd = CommandBufferPool.Get();
                using (new ProfilingScope(cmd, _profilingSampler))
                {
                    material.SetTexture(DestProp, destination);
                    material.SetFloat(IntensityProp, feedBack.intensity.value);
                    Blitter.BlitCameraTexture(cmd, source, source, material, 0);
                    Blitter.BlitCameraTexture(cmd, source, destination, material, 1);
                    //Blitter.BlitCameraTexture(cmd, source, source);
                }
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        void Dispose()
        {
            CoreUtils.Destroy(material);
            destination?.Release();
        }
    }

    FeedBackRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new FeedBackRenderPass();
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


