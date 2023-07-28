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
    KickTrigger kickTrigger;

    [SerializeField]
    float gridSize;
    [SerializeField]
    Vector3 areaSize;

    int count = 128;

    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    struct GridParticleData
    {
        public Vector3 position;
        public Vector3 prevPosition;
        public Vector3 direction;
    };

    GraphicsBuffer buffer;

    private readonly int BufferProp = Shader.PropertyToID("ParticleBuffer");

    private readonly int CountProp = Shader.PropertyToID("Count");
    private readonly int SeedProp = Shader.PropertyToID("Seed");
    private readonly int GridProp = Shader.PropertyToID("GridSize");
    private readonly int AreaProp = Shader.PropertyToID("AreaSize");
    private readonly int TimeProp = Shader.PropertyToID("DeltaTime");

    [SerializeField]
    ComputeShader cs;

    int initKernel;
    int directionUpdateKernel;
    int updateKernel;

    Random rand;

    private void Start()
    {
        rand = new Random(5423);

        initKernel = cs.FindKernel("Init");
        directionUpdateKernel = cs.FindKernel("DirectionUpdate");
        updateKernel = cs.FindKernel("Update");

        buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, sizeof(float) * 9);

        vfx.SetGraphicsBuffer(BufferProp, buffer);

        cs.SetInt(CountProp, count);

        ParticleInit(buffer);

        kickTrigger.onKickOn += OnKick;
    }

    private void OnKick(float audioLevel)
    {
        cs.SetFloat(GridProp, gridSize);
        cs.SetVector(AreaProp, areaSize);

        StopAllCoroutines();
        DirectionUpdate();
        StartCoroutine(nameof(ParticleUpdate));
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

    void DirectionUpdate()
    {
        cs.SetInt(SeedProp, math.abs(rand.NextInt()));
        cs.SetBuffer(directionUpdateKernel, BufferProp, buffer);
        cs.Dispatch(directionUpdateKernel, count / 8, 1, 1);
    }

    IEnumerator ParticleUpdate()
    {
        var time = 0.0f;
        while (time <= 1)
        {
            if (time > 1) time = 1;
            ParticleBufferUpdate(time);
            time += Time.deltaTime * 10;
            yield return null;
        }
    }

    void ParticleBufferUpdate(float time)
    {
        cs.SetFloat(TimeProp, Mathf.Exp(time) / Mathf.Exp(1) - 1);

        cs.SetBuffer(updateKernel, BufferProp, buffer);
        cs.Dispatch(updateKernel, count / 8, 1, 1);
    }
}
