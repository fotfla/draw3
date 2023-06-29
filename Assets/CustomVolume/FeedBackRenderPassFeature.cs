using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

public class FeedBackRenderPassFeature : ScriptableRendererFeature
{
    class FeedBackRenderPass : ScriptableRenderPass
    {
        ProfilingSampler _profilingSampler = new ProfilingSampler("FeedBack");

        RTHandle source;
        RTHandle destination;

        FeedBack feedBack;

        Material material;

        (RTHandle last, RTHandle next) _buffer;
        RTHandle NewBuffer(string name) => RTHandles.Alloc(Vector2.one, useDynamicScale: true, name: name, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32);

        public void Setup(RTHandle source)
        {
            material = CoreUtils.CreateEngineMaterial("Hidden/Shader/FeedBack");
            this.source = source;
            destination = source;
            // destination = NewBuffer("Dest");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(source);
            // _buffer = (NewBuffer("FeedBack1"), NewBuffer("FeedBack2"));
            // CoreUtils.SetRenderTarget(cmd, _buffer.last, ClearFlag.Color);
            // CoreUtils.SetRenderTarget(cmd, _buffer.next, ClearFlag.Color);
            // destination = NewBuffer("Dest");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            feedBack = stack.GetComponent<FeedBack>();

            var cmd = CommandBufferPool.Get();
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                material.SetTexture("_Dest", destination);
                Blitter.BlitCameraTexture(cmd, source, source, material, 0);

                Blitter.BlitCameraTexture(cmd, source, destination);
                Blitter.BlitCameraTexture(cmd, destination, source);
            }
        }

        void Dispose()
        {
            CoreUtils.Destroy(material);
            destination?.Release();
        }
    }

    FeedBackRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new FeedBackRenderPass();
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_ScriptablePass.Setup(renderer.cameraColorTargetHandle);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


