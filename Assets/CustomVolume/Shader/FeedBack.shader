Shader "Hidden/Shader/FeedBack"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "FeedBackPass"

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            TEXTURE2D_X(_Dest);
            SAMPLER(sampler_Dest);

            #pragma vertex Vert
            #pragma fragment frag

            half4 frag (Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.texcoord);
                // float4 dest = SAMPLE_TEXTURE2D_X(_Dest, sampler_Dest, (input.texcoord - 0.5) * 0.5 + 0.5);
                float4 dest = SAMPLE_TEXTURE2D_X(_Dest, sampler_Dest, input.texcoord);
                return color + dest;
                // return dest;
                // return float4(1,0,0,0);
            }
            ENDHLSL
        }
    }
}
