Shader "Hidden/Shader/SlitScan"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off Cull Off ZTest Always Blend Off
        Pass
        {
            Name "SlitScanPass"

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            SAMPLER(sampler_BlitTexture);
            float4 _BlitTexture_TexelSize;

            TEXTURE2D_X(_Dest);
            SAMPLER(sampler_Dest);

            float _Intensity;

            float _Split;
            float _Speed;

            #pragma vertex Vert
            #pragma fragment frag

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = input.texcoord;
                float y = uv.y > 0.5 ? uv.y - _Speed * _BlitTexture_TexelSize.y : uv.y + _Speed * _BlitTexture_TexelSize.y;
                uv.y = y;
                float intensity = step( abs(uv.y - 0.5) , (1 - _Split) * 0.5);

                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord);
                float4 dest = SAMPLE_TEXTURE2D_X(_Dest, sampler_Dest, uv);

                return lerp(dest, color, intensity);
            }
            ENDHLSL
        }
    }
}
