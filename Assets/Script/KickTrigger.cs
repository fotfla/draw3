using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lasp;
using Random = Unity.Mathematics.Random;

public class KickTrigger : MonoBehaviour
{
    [SerializeField]
    AudioLevelTracker lowTracker;

    [SerializeField]
    float threshold = 0.3f;
    bool isUpdate = false;

    public delegate void Kick(float audioLevel, uint seed);

    public Kick onKickOn;
    public Kick onKickOff;

    Random random;

    void OnEnable()
    {
        random = new Random(255);
    }

    void Update()
    {
        if (lowTracker.normalizedLevel > threshold && isUpdate)
        {
            isUpdate = false;
            onKickOn?.Invoke(lowTracker.normalizedLevel, random.NextUInt(0, 65535));
        }
        else if (lowTracker.normalizedLevel < threshold && !isUpdate)
        {
            isUpdate = true;
            onKickOff?.Invoke(0, 0);
        }
    }

    public void SetDynamicRange(byte value)
    {
        lowTracker.dynamicRange = 39 * value / 127.0f + 1;
    }

    public void SetThreshold(byte value)
    {
        threshold = value / 127.0f;
    }
}
