using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class GridParticle : MonoBehaviour
{
    [SerializeField]
    VisualEffect vfx;

    [SerializeField]
    int gridSize;
    [SerializeField]
    Vector3 areaSize;

    int count = 128;

    GraphicsBuffer buffer;
    GraphicsBuffer prevBuffer;

    private readonly int BufferProp = Shader.PropertyToID("ParticleBuffer");
    private readonly int PrevBufferProp = Shader.PropertyToID("PrevBuffer");

    private readonly int CountProp = Shader.PropertyToID("Count");
    private readonly int SeedProp = Shader.PropertyToID("Seed");
    private readonly int GridProp = Shader.PropertyToID("GridSize");
    private readonly int AreaProp = Shader.PropertyToID("AreaSize");
    private readonly int TimeProp = Shader.PropertyToID("DeltaTime");

    [SerializeField]
    ComputeShader cs;

    int initKernel;
    int updateKernel;

    Random rand;

    private void Start()
    {
        rand = new Random();

        initKernel = cs.FindKernel("Init");
        updateKernel = cs.FindKernel("Update");

        buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, sizeof(float) * 3);
        prevBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, sizeof(float) * 3);

        vfx.SetGraphicsBuffer(BufferProp, buffer);

        cs.SetInt(CountProp, count);

        ParticleInit(buffer);
    }

    private void Update()
    {
        vfx.SetFloat(GridProp, gridSize);
    }

    void ParticleInit(GraphicsBuffer particleBuffer)
    {
        cs.SetInt(SeedProp, math.abs(rand.NextInt()));
        cs.SetBuffer(initKernel, BufferProp, particleBuffer);
        cs.Dispatch(initKernel, count / 8, 1, 1);
    }

    IEnumerator ParticleUpdate()
    {
        var time = 0.0f;
        while (time < 1)
        {
            time += Time.deltaTime;
            ParticleBufferUpdate();
            yield return null;
        }
    }

    void ParticleBufferUpdate()
    {
        cs.SetFloat(GridProp, gridSize);
        cs.SetVector(AreaProp, areaSize);

        cs.SetFloat(TimeProp, Time.deltaTime * 5);

        cs.SetBuffer(updateKernel, BufferProp, buffer);
        cs.SetBuffer(updateKernel, PrevBufferProp, prevBuffer);
        cs.Dispatch(updateKernel, count / 8, 1, 1);
    }
}
