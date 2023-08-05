using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenuForRenderPipeline("PostEffect/SlitScan", typeof(UniversalRenderPipeline))]
    public class SlitScan : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter split = new ClampedFloatParameter(0, 0, 1);
        public FloatParameter speed = new FloatParameter(0);
        public bool IsActive() => split.value > 0;
        public bool IsTileCompatible() => false;
    }
}
