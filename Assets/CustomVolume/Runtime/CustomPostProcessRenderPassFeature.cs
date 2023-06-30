using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessRenderPassFeature : ScriptableRendererFeature
{
    class CustomPostProcessRenderPass : ScriptableRenderPass
    {
        // Voluem
        private CrackShiftPostProcessVolume crackShift;
        private InverseSlicePostProcessVolume inverseSlice;
        private WorldVoroNoisePostProcessVolume worldVoroNoise;
        private WorldDistortionPostProcessVolume worldDistortion;
        private DistortionPostProcessVolume distortion;
        private EdgeDitectionPostProcessVolume edgeDetection;
        private GlitchPostProcessVolume glitch;
        private UVShiftPostProcessVolume uvShift;
        private ScanLinePostProcessVolume scanLine;
        private FluidPostProcessVolume fluid;
        private PixelSorterPostProcessVolume pixelsort;
        private TextureOverrayPostProcessVolume textureOverray;
        private ComputeShader computeShader;
        private int sortKernel;
        private int count;


        private RenderTargetIdentifier colorTargetId;
        private int tempTargetId = Shader.PropertyToID("_TempTarget");
        private readonly int inputTextureId = Shader.PropertyToID("_InputTexture");

        private MaterialLibrary materialLibrary;

        private int width;
        private int height;
        private RenderTargetIdentifier source;

        public CustomPostProcessRenderPass()
        {
            materialLibrary = new MaterialLibrary();
            computeShader = Resources.Load<ComputeShader>("PixelSort");
            sortKernel = computeShader.FindKernel("Sort");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            crackShift = stack.GetComponent<CrackShiftPostProcessVolume>();
            inverseSlice = stack.GetComponent<InverseSlicePostProcessVolume>();
            worldVoroNoise = stack.GetComponent<WorldVoroNoisePostProcessVolume>();
            worldDistortion = stack.GetComponent<WorldDistortionPostProcessVolume>();
            distortion = stack.GetComponent<DistortionPostProcessVolume>();
            edgeDetection = stack.GetComponent<EdgeDitectionPostProcessVolume>();
            glitch = stack.GetComponent<GlitchPostProcessVolume>();
            fluid = stack.GetComponent<FluidPostProcessVolume>();
            pixelsort = stack.GetComponent<PixelSorterPostProcessVolume>();
            uvShift = stack.GetComponent<UVShiftPostProcessVolume>();
            scanLine = stack.GetComponent<ScanLinePostProcessVolume>();
            textureOverray = stack.GetComponent<TextureOverrayPostProcessVolume>();

            var cmd = CommandBufferPool.Get("Custom Post Process");
            // using
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public void Setup(in RenderTargetIdentifier colerTarget, in RenderingData renderingData)
        {
            var cameradata = renderingData.cameraData;
            width = cameradata.camera.scaledPixelWidth;
            height = cameradata.camera.scaledPixelHeight;

            colorTargetId = colerTarget;
        }

        int GetDestination(CommandBuffer commandBuffer)
        {
            var destination = tempTargetId;
            commandBuffer.GetTemporaryRT(destination, width, height, 0, FilterMode.Point, RenderTextureFormat.Default);
            return destination;
        }

        void Render(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            source = colorTargetId;
            if (textureOverray.IsActive()) DoTextureOverray(commandBuffer, ref renderingData);
            if (uvShift.IsActive()) DoUVShift(commandBuffer, ref renderingData);
            if (scanLine.IsActive()) DoScanLine(commandBuffer, ref renderingData);
            if (worldVoroNoise.IsActive()) DoWorldVoroNoise(commandBuffer, ref renderingData);
            if (worldDistortion.IsActive()) DoWorldDistortion(commandBuffer, ref renderingData);
            if (distortion.IsActive()) DoDistortion(commandBuffer, ref renderingData);
            if (edgeDetection.IsActive()) DoEdgeDetection(commandBuffer, ref renderingData);
            if (inverseSlice.IsActive()) DoInverseSlice(commandBuffer, ref renderingData);
            if (crackShift.IsActive()) DoCrackShift(commandBuffer, ref renderingData);
            if (glitch.IsActive()) DoGlitch(commandBuffer, ref renderingData);
            if (fluid.IsActive()) DoFluid(renderingData.cameraData, commandBuffer, ref renderingData);
            if (pixelsort.IsActive()) DoPixelSort(commandBuffer, ref renderingData);
        }

        void DoCrackShift(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var destination = GetDestination(commandBuffer);

            var mat = materialLibrary.crackShift;

            mat.SetFloat(ShaderConstants.Intensity, crackShift.intensity.value);
            mat.SetInt(ShaderConstants.Seed, crackShift.seed.value);
            mat.SetInt(ShaderConstants.Div, crackShift.div.value);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);

            commandBuffer.Blit(source, destination);
            commandBuffer.Blit(destination, source, mat);
        }

        void DoInverseSlice(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var destination = GetDestination(commandBuffer);
            var mat = materialLibrary.inverseSlice;
            mat.SetFloat(ShaderConstants.Intensity, inverseSlice.intensity.value);
            mat.SetFloat(ShaderConstants.Angle, inverseSlice.angle.value);
            mat.SetVector(ShaderConstants.Offset, inverseSlice.offset.value);
            mat.SetInt("_ShapeType", (int)inverseSlice.shapeType.value);
            mat.SetFloat("_Width", inverseSlice.width.value);
            mat.SetFloat("_Radius", inverseSlice.radius.value);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);

            commandBuffer.Blit(source, destination);
            commandBuffer.Blit(destination, source, mat);
        }

        void DoWorldVoroNoise(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var cameradata = renderingData.cameraData;
            var destination = GetDestination(commandBuffer);

            materialLibrary.worldVoroNoise.SetFloat(ShaderConstants.Intensity, worldVoroNoise.intensity.value);
            materialLibrary.worldVoroNoise.SetFloat(ShaderConstants.NoiseSacle, worldVoroNoise.noiseScale.value);
            materialLibrary.worldVoroNoise.SetFloat(ShaderConstants.NoiseIntensity, worldVoroNoise.noiseIntensity.value);
            materialLibrary.worldVoroNoise.SetInt(ShaderConstants.Seed, worldVoroNoise.seed.value);
            materialLibrary.worldVoroNoise.SetMatrix(ShaderConstants.InverseProjectionMatrix, cameradata.camera.projectionMatrix.inverse);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);

            commandBuffer.Blit(source, destination);
            commandBuffer.Blit(destination, source, materialLibrary.worldVoroNoise);
        }

        void DoWorldDistortion(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var cameradata = renderingData.cameraData;
            var destination = GetDestination(commandBuffer);

            materialLibrary.worldDistortion.SetFloat(ShaderConstants.Intensity, worldDistortion.intensity.value);
            materialLibrary.worldDistortion.SetFloat(ShaderConstants.NoiseIntensity, worldDistortion.noiseIntensity.value);
            materialLibrary.worldDistortion.SetFloat(ShaderConstants.NoiseSacle, worldDistortion.noiseScale.value);
            materialLibrary.worldDistortion.SetInt(ShaderConstants.Seed, worldDistortion.seed.value);
            materialLibrary.worldVoroNoise.SetMatrix(ShaderConstants.InverseProjectionMatrix, cameradata.camera.projectionMatrix.inverse);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);

            commandBuffer.Blit(source, destination);
            commandBuffer.Blit(destination, source, materialLibrary.worldDistortion);
        }

        void DoDistortion(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var destination = GetDestination(commandBuffer);
            var mat = materialLibrary.distortion;
            mat.SetFloat(ShaderConstants.Intensity, distortion.intensity.value);
            mat.SetFloat("_Amount", distortion.amout.value);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);

            commandBuffer.Blit(source, destination);
            commandBuffer.Blit(destination, source, mat);
        }

        void DoEdgeDetection(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {

            var destination = GetDestination(commandBuffer);
            materialLibrary.edgeDetection.SetFloat(ShaderConstants.Intensity, edgeDetection.intensity.value);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);

            commandBuffer.Blit(source, destination);
            commandBuffer.Blit(destination, source, materialLibrary.edgeDetection);
        }

        void DoGlitch(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var mat = materialLibrary.glitch;
            mat.SetFloat(ShaderConstants.Intensity, glitch.intensity.value);
            mat.SetInt(ShaderConstants.Seed, glitch.seed.value);
            mat.SetFloat(ShaderConstants.Threshold, glitch.threshold.value);
            mat.SetVector(ShaderConstants.Resolution, new Vector2(glitch.width.value, glitch.height.value));

            var destination = GetDestination(commandBuffer);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);

            commandBuffer.Blit(source, destination);
            commandBuffer.Blit(destination, source, mat);
        }

        Matrix4x4 prevViewProj;

        void DoFluid(CameraData cameraData, CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var mat = materialLibrary.fluid;

            var proj = cameraData.GetProjectionMatrix();
            var view = cameraData.GetViewMatrix();
            var viewProj = proj * view;
            mat.SetMatrix("_ViewProjM", viewProj);
            mat.SetMatrix("_PrevViewProjM", prevViewProj);
            prevViewProj = viewProj;

            mat.SetFloat(ShaderConstants.Intensity, fluid.intensity.value);
            mat.SetFloat("_Clamp", fluid.clamp.value);

            var dest = GetDestination(commandBuffer);


            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);
            commandBuffer.Blit(source, dest);
            commandBuffer.Blit(dest, source, mat);
        }

        void DoPixelSort(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            if (computeShader == null) return;

            var dest = GetDestination(commandBuffer);

            // var count = 512;
            var srcId = Shader.PropertyToID("_TempSrc");
            var dstId = Shader.PropertyToID("_TempDst");
            commandBuffer.GetTemporaryRT(srcId, 512, 512, 0, FilterMode.Point, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, 1, true);
            commandBuffer.GetTemporaryRT(dstId, 512, 512, 0, FilterMode.Point, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, 1, true);
            commandBuffer.Blit(source, srcId);
            commandBuffer.Blit(source, dstId);

            commandBuffer.SetComputeIntParam(computeShader, "_PixelSize", 512);
            commandBuffer.SetComputeFloatParam(computeShader, "_Threshold", pixelsort.threshold.value);

            for (var i = 0; i < 512; i++)
            {
                commandBuffer.SetComputeIntParam(computeShader, "_Iteration", i);
                commandBuffer.SetComputeTextureParam(computeShader, sortKernel, "_Src", srcId);
                commandBuffer.SetComputeTextureParam(computeShader, sortKernel, "_Dst", dstId);

                commandBuffer.DispatchCompute(computeShader, sortKernel, Mathf.CeilToInt(512 / 8), Mathf.CeilToInt(512 / 8), 1);

                commandBuffer.Blit(dstId, srcId);
                // CoreUtils.Swap(ref dstId, ref srcId);
            }

            commandBuffer.Blit(dstId, source);
            commandBuffer.ReleaseTemporaryRT(srcId);
            commandBuffer.ReleaseTemporaryRT(dstId);
        }

        void DoUVShift(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var mat = materialLibrary.uvShift;

            mat.SetFloat(ShaderConstants.Intensity, uvShift.intensity.value);
            mat.SetVector("_Shift", new Vector2(uvShift.shiftX.value, uvShift.shiftY.value));

            var dest = GetDestination(commandBuffer);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);
            commandBuffer.Blit(source, dest);
            commandBuffer.Blit(dest, source, mat);
        }

        void DoScanLine(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var cameradata = renderingData.cameraData;
            var destination = GetDestination(commandBuffer);

            var mat = materialLibrary.scanLine;

            mat.SetFloat(ShaderConstants.Intensity, scanLine.intensity.value);
            mat.SetFloat("_Width", scanLine.width.value);
            mat.SetFloat("_Length", scanLine.Lnegth.value);
            mat.SetFloat("_Depth", scanLine.depth.value);
            mat.SetMatrix(ShaderConstants.InverseProjectionMatrix, cameradata.camera.projectionMatrix.inverse);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);

            commandBuffer.Blit(source, destination);
            commandBuffer.Blit(destination, source, mat);
        }

        void DoTextureOverray(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var mat = materialLibrary.textureOverray;

            mat.SetFloat(ShaderConstants.Intensity, textureOverray.intensity.value);
            mat.SetTexture("_OverRayTexture", textureOverray.texture.value);

            var dest = GetDestination(commandBuffer);

            commandBuffer.SetGlobalTexture(inputTextureId, colorTargetId);
            commandBuffer.Blit(source, dest);
            commandBuffer.Blit(dest, source, mat);
        }
    }

    CustomPostProcessRenderPass customPostProcessSlicePass;

    /// <inheritdoc/>
    public override void Create()
    {
        customPostProcessSlicePass = new CustomPostProcessRenderPass();
        customPostProcessSlicePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        customPostProcessSlicePass.Setup(renderer.cameraColorTargetHandle, renderingData);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // var cameradata = renderingData.cameraData;
        // if (cameradata.isSceneViewCamera || cameradata.isPreviewCamera || cameradata.cameraType == CameraType.Reflection) return;

        // customPostProcessSlicePass.Setup(finalBlit, ref renderingData);

        // customPostProcessSlicePass.Setup(cameradata.renderer.cameraColorTargetHandle, ref renderingData);

        renderer.EnqueuePass(customPostProcessSlicePass);
    }

    class MaterialLibrary
    {
        public readonly Material crackShift;
        public readonly Material inverseSlice;
        public readonly Material worldVoroNoise;
        public readonly Material worldDistortion;
        public readonly Material distortion;
        public readonly Material edgeDetection;
        public readonly Material glitch;
        public readonly Material uvShift;
        public readonly Material scanLine;
        public readonly Material textureOverray;

        public readonly Material fluid;

        public MaterialLibrary()
        {
            crackShift = Load(ShaderResources.CrackShift);
            inverseSlice = Load(ShaderResources.InverseSlice);
            worldVoroNoise = Load(ShaderResources.WorldvoroNoise);
            worldDistortion = Load(ShaderResources.WorldDistortion);
            distortion = Load(ShaderResources.Distortion);
            edgeDetection = Load(ShaderResources.EdgeDitection);
            glitch = Load(ShaderResources.Glitch);
            uvShift = Load(ShaderResources.UVShift);
            scanLine = Load(ShaderResources.ScanLine);
            fluid = Load(ShaderResources.Fluid);
            textureOverray = Load(ShaderResources.TextureOverray);
        }

        Material Load(string path)
        {
            var shader = Shader.Find(path);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return null;
            }
            else if (!shader.isSupported)
            {
                return null;
            }
            return CoreUtils.CreateEngineMaterial(shader);
        }

        internal void Cleanup()
        {
            CoreUtils.Destroy(crackShift);
            CoreUtils.Destroy(inverseSlice);
            CoreUtils.Destroy(worldVoroNoise);
            CoreUtils.Destroy(worldDistortion);
            CoreUtils.Destroy(distortion);
            CoreUtils.Destroy(edgeDetection);
            CoreUtils.Destroy(glitch);
            CoreUtils.Destroy(fluid);
            CoreUtils.Destroy(uvShift);
            CoreUtils.Destroy(scanLine);
            CoreUtils.Destroy(textureOverray);
        }
    }

    static class ShaderConstants
    {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int InverseProjectionMatrix = Shader.PropertyToID("_InverseProjectionMatrix");
        public static readonly int Threshold = Shader.PropertyToID("_Threshold");
        public static readonly int Seed = Shader.PropertyToID("_Seed");

        //CrackShift
        public static readonly int Div = Shader.PropertyToID("_Div");

        // InverseSlice
        public static readonly int Angle = Shader.PropertyToID("_Angle");
        public static readonly int Offset = Shader.PropertyToID("_Offset");

        // WorldVoroNoise
        public static readonly int NoiseSacle = Shader.PropertyToID("_NoiseScale");
        public static readonly int NoiseIntensity = Shader.PropertyToID("_NoiseIntensity");

        // Glitch
        public static readonly int Resolution = Shader.PropertyToID("_Resolution");

    }

    static class ShaderResources
    {
        public static readonly string CrackShift = "Hidden/Shader/CrackShift";
        public static readonly string InverseSlice = "Hidden/Shader/InverseSlice";
        public static readonly string WorldvoroNoise = "Hidden/Shader/WorldVoroNoise";
        public static readonly string Distortion = "Hidden/Shader/Distortion";
        public static readonly string EdgeDitection = "Hidden/Shader/EdgeDitection";
        public static readonly string Glitch = "Hidden/Shader/Glitch";
        public static readonly string WorldDistortion = "Hidden/Shader/WorldDistortion";
        public static readonly string UVShift = "Hidden/Shader/UVShift";
        public static readonly string ScanLine = "Hidden/Shader/ScanLine";
        public static readonly string Fluid = "Hidden/Shader/Fluid";
        public static readonly string TextureOverray = "Hidden/Shader/TextureOverray";
    }
}


