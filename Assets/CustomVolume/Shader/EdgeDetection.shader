Shader "Hidden/Shader/EdgeDetection"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off Cull Off ZTest Always Blend Off
        Pass
        {
            Name "EdgeDetection"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Intensity;
            int _Depth;
            float4 _BlitTexture_TexelSize;
            SAMPLER(sampler_BlitTexture);

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                uint d =(uint) _Depth;
                float3 outColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord).xyz;
                float3 baseColor = outColor;
                float3 outColor0 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord + int2( d, d) * _BlitTexture_TexelSize.xy).xyz;
                float3 outColor1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord + int2(-d,-d) * _BlitTexture_TexelSize.xy).xyz;
                float3 outColor2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord + int2( d ,-d) * _BlitTexture_TexelSize.xy).xyz;
                float3 outColor3 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord + int2(-d, d) * _BlitTexture_TexelSize.xy).xyz;

                float3 dx = (outColor0 - outColor1) * 0.5;
                float3 dy = (outColor2 - outColor3) * 0.5;
                outColor = sqrt(dot(dx,dx) + dot(dy,dy));
                outColor = lerp(baseColor, outColor,_Intensity);

                return float4(outColor, 1);
            }

            ENDHLSL
        }
    }
    Fallback Off
}
