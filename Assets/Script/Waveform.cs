using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using Lasp;

[RequireComponent(typeof(MeshRenderer))]
public class Waveform : MonoBehaviour
{
    [SerializeField]
    AudioLevelTracker audioLevelTracker;

    Mesh mesh;

    int vertexCount = 2048;

    [SerializeField]
    VisualEffect vfx;

    [SerializeField]
    ComputeShader cs;
    int updateKernel;
    GraphicsBuffer vertexData;
    GraphicsBuffer circleData;
    private readonly int VertexProp = Shader.PropertyToID("_VertexData");
    private readonly int CircleProp = Shader.PropertyToID("_CircleData");
    private readonly int CountProp = Shader.PropertyToID("_Count");

    void Start()
    {
        InitializeMesh();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        if (Time.frameCount % 30 == 0) UpdateMesh(audioLevelTracker.audioDataSlice);
    }

    void OnDestory()
    {
        if (mesh != null) Destroy(mesh);

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

        mesh = new Mesh();
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10);

        using (var vertices = CreateVertexArray(default(NativeSlice<float>)))
        {
            var desc = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

            mesh.SetVertexBufferParams(vertices.Length, desc);
            mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);

            vertexData.SetData(vertices);
            cs.SetBuffer(updateKernel, VertexProp, vertexData);
            cs.SetBuffer(updateKernel, CircleProp, circleData);
            cs.SetInt(CountProp, vertices.Length);
            cs.Dispatch(updateKernel, vertices.Length / 8, 1, 1);
        }

        using (var indices = CreateIndexArray())
        {
            var desc = new SubMeshDescriptor(0, indices.Length, MeshTopology.Lines);

            mesh.SetIndexBufferParams(indices.Length, IndexFormat.UInt32);
            mesh.SetIndexBufferData(indices, 0, 0, indices.Length);
            mesh.SetSubMesh(0, desc);
        }
    }

    void UpdateMesh(NativeSlice<float> source)
    {
        using (var vertices = CreateVertexArray(source))
        {
            mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);

            vertexData.SetData(vertices);
            cs.SetBuffer(updateKernel, VertexProp, vertexData);
            cs.SetBuffer(updateKernel, CircleProp, circleData);
            cs.SetInt(CountProp, vertices.Length);
            cs.Dispatch(updateKernel, vertices.Length / 8, 1, 1);
        }
    }

    NativeArray<float3> CreateVertexArray(NativeSlice<float> source)
    {
        var vertices = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        var vcount = math.min(source.Length, vertexCount);

        for (var i = 0; i < vcount; i++)
        {
            var x = (float)i / (vcount - 1);
            var xpos = math.remap(0, 1, -5, 5, x);
            vertices[i] = math.float3(xpos, source[i] * 5, 0);
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
