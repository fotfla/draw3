Shader "Hidden/Shader/FeedBack"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off Cull Off ZTest Always Blend Off
        Pass
        {
            Name "FeedBackPass"

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            SAMPLER(sampler_BlitTexture);

            TEXTURE2D_X(_Dest);
            SAMPLER(sampler_Dest);

            float _Intensity;

            #pragma vertex Vert
            #pragma fragment frag

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord);           
                float4 dest = SAMPLE_TEXTURE2D_X(_Dest, sampler_Dest, input.texcoord);
                return lerp(color, dest, _Intensity);
            }
            ENDHLSL
        }
    }
}
