using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using Lasp;

public class Waveform : MonoBehaviour
{
    [SerializeField]
    AudioLevelTracker audioLevelTracker;
    [SerializeField]
    float amp = 10;
    [SerializeField]
    uint updateFrame = 60;

    int vertexCount = 512;

    [SerializeField]
    VisualEffect vfx;

    [SerializeField]
    ComputeShader cs;

    [SerializeField, Range(0, 10)]
    float minThreshold;
    [SerializeField, Range(0, 10)]
    float maxThreshold;
    int updateKernel;
    GraphicsBuffer vertexData;
    GraphicsBuffer circleData;
    private readonly int VertexProp = Shader.PropertyToID("_VertexData");
    private readonly int CircleProp = Shader.PropertyToID("_CircleData");
    private readonly int CountProp = Shader.PropertyToID("_Count");
    private readonly int ThresholdProp = Shader.PropertyToID("_Threshold");
    private readonly int TimeProp = Shader.PropertyToID("Time");

    void Start()
    {
        InitializeMesh();
    }

    void Update()
    {
        var frameCount = Time.frameCount % updateFrame;
        if (frameCount == 0) UpdateMesh(audioLevelTracker.audioDataSlice);
        vfx.SetFloat(TimeProp, frameCount / (float)updateFrame);
    }

    void OnDestory()
    {
        vertexData?.Dispose();
        vertexData = null;
        circleData?.Dispose();
        circleData = null;
    }

    void InitializeMesh()
    {
        updateKernel = cs.FindKernel("Update");
        circleData = new GraphicsBuffer(GraphicsBuffer.Target.Structured, vertexCount, 4 * sizeof(float));
        vertexData = new GraphicsBuffer(GraphicsBuffer.Target.Structured, vertexCount, 3 * sizeof(float));

        vfx.SetGraphicsBuffer(CircleProp, circleData);
        vfx.SetGraphicsBuffer(VertexProp, vertexData);

        UpdateMesh(default(NativeSlice<float>));
    }

    void UpdateMesh(NativeSlice<float> source)
    {
        using (var vertices = CreateVertexArray(source))
        {
            vertexData.SetData(vertices);
            cs.SetBuffer(updateKernel, VertexProp, vertexData);
            cs.SetBuffer(updateKernel, CircleProp, circleData);
            cs.SetInt(CountProp, vertices.Length);
            cs.SetFloats(ThresholdProp, new float[2] { minThreshold, maxThreshold });
            cs.Dispatch(updateKernel, vertices.Length / 32, 1, 1);
        }
    }

    NativeArray<float3> CreateVertexArray(NativeSlice<float> source)
    {
        var vertices = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        var vcount = math.min(source.Length, vertexCount);

        for (var i = 0; i < vcount; i++)
        {
            var x = (float)i / (vcount - 1);
            var xpos = math.remap(0, 1, -3, 3, x);
            vertices[i] = math.float3(xpos, source[i] * amp, 0);
        }
        var last = (vcount == 0) ? float3.zero : vertices[vcount - 1];
        for (var i = vcount; i < vertexCount; i++)
        {
            vertices[i] = last;
        }
        return vertices;
    }

    NativeArray<int> CreateIndexArray()
    {
        return new NativeArray<int>(Enumerable.Range(0, vertexCount).ToArray(), Allocator.Temp);
    }
}
