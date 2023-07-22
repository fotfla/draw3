using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class CubeParticle : MonoBehaviour
{
    [SerializeField]
    KickTrigger trigger;

    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    struct CubeParticleData
    {
        public Vector3 position;
        public Vector2 scale;
    };

    [SerializeField]
    VisualEffect vfx;

    [SerializeField]
    int count = 128;

    GraphicsBuffer particleBuffer;
    GraphicsBuffer nextParticleBuffer;
    private readonly int BufferProp = Shader.PropertyToID("ParticleBuffer");
    private readonly int NextBufferProp = Shader.PropertyToID("NextParticleBuffer");
    private readonly int CountProp = Shader.PropertyToID("Count");
    private readonly int SeedProp = Shader.PropertyToID("Seed");
    private readonly int TimeProp = Shader.PropertyToID("Time");
    private readonly int DeltaTimeProp = Shader.PropertyToID("_DeltaTime");
    private readonly int AreaProp = Shader.PropertyToID("AreaSize");
    private readonly int ScaleProp = Shader.PropertyToID("ScaleSize");

    [SerializeField]
    ComputeShader cs;
    int initKernel;
    int updateKernel;

    [SerializeField]
    Vector3 areaSize = new Vector3(3, 3, 3);
    [SerializeField]
    Vector4 scaleSize = new Vector4(0.5f, 1, 0.5f, 1);

    [SerializeField]
    float attackSpeed = 10;

    Random random;

    void Start()
    {
        random = new Random(5314);
        particleBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, sizeof(float) * 5);
        nextParticleBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, sizeof(float) * 5);
        initKernel = cs.FindKernel("Init");
        updateKernel = cs.FindKernel("Update");
        trigger.onKickOn += OnKickOn;

        cs.SetInt(CountProp, count);
        cs.SetVector(AreaProp, areaSize);
        cs.SetVector(ScaleProp, scaleSize);

        ParticleInit(particleBuffer);

        vfx.SetInt(CountProp, count);
        vfx.SetGraphicsBuffer(BufferProp, particleBuffer);
    }

    public void OnKickOn(float audioLevel)
    {
        StopAllCoroutines();
        ParticleInit(nextParticleBuffer);
        StartCoroutine(nameof(ParticleUpdate));
    }

    IEnumerator ParticleUpdate()
    {
        var time = 0.0f;
        while (time < 1)
        {
            time += Time.deltaTime;
            ParitcleBufferUpdate();
            yield return null;
        }
    }

    void ParticleInit(GraphicsBuffer particleBuffer)
    {
        cs.SetInt(SeedProp, random.NextInt());
        cs.SetBuffer(0, BufferProp, particleBuffer);
        cs.Dispatch(0, count / 8, 1, 1);
    }

    void ParitcleBufferUpdate()
    {
        cs.SetFloat(DeltaTimeProp, Time.deltaTime * attackSpeed);
        cs.SetBuffer(updateKernel, BufferProp, particleBuffer);
        cs.SetBuffer(updateKernel, NextBufferProp, nextParticleBuffer);
        cs.Dispatch(updateKernel, count / 8, 1, 1);
    }
}



