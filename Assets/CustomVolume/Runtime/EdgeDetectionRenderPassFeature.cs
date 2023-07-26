using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

public class EdgeDetectionRenderPassFeature : ScriptableRendererFeature
{
    class EdgeDetectionRenderPass : ScriptableRenderPass
    {
        ProfilingSampler _profilingSampler = new ProfilingSampler("EdgeDetection");

        RTHandle source;

        EdgeDetection edgeDetection;

        Material material;

        private readonly int IntensityProp = Shader.PropertyToID("_Intensity");

        public void Setup(RTHandle source, in RenderingData renderingData){
            material = CoreUtils.CreateEngineMaterial("Hidden/Shader/EdgeDetection");
            this.source = source;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            edgeDetection = stack.GetComponent<EdgeDetection>();

            if(edgeDetection.IsActive())
            {
                var cmd = CommandBufferPool.Get();
                Render(cmd, ref renderingData);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                material.SetFloat(IntensityProp, edgeDetection.intensity.value);
                Blitter.BlitCameraTexture(cmd, source, source, material, 0);
            }
        }

        void Dispose(){
            CoreUtils.Destroy(material);
        }
    }

    EdgeDetectionRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new EdgeDetectionRenderPass();

        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_ScriptablePass.Setup(renderer.cameraColorTargetHandle, renderingData);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}

