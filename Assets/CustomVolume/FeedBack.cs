using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenuForRenderPipeline("PostEffect/FeedBack", typeof(UniversalRenderPipeline))]
    public class FeedBack : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0, 0, 1);

        public bool IsActive() => intensity.value > 0;
        public bool IsTileCompatible() => false;
    }
}

