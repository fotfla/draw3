Shader "Hidden/Shader/Glitch"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    #include "Noise.hlsl"

    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_InputTexture);
    float4 _InputTexture_TexelSize;
    
    float2 _Resolution;
    float _Threshold;
    float _Seed;

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _InputTexture_TexelSize.zw;
        float3 baseColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;

        float2 uv = input.texcoord * _Resolution;
        float2 seed = float2(fmod(_Seed * 65536,289),floor(_Seed * 65536/289));
        uint2 iuv = floor(uv + seed);
        float2 r = hash22(iuv);
        uv += r * _Resolution * 2.0;
        r = r.x > _Threshold || r.y > _Threshold ? 0.5 : r;
        r = r * 2.0 - 1.0;
        float2 s = abs(r) < 0.001 ? 1 : sign(r);
        float2 st = frac(abs(input.texcoord * s + r));

        positionSS = st * _InputTexture_TexelSize.zw;
        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;

        uv *= 2.0;
        iuv = floor(uv + seed);
        r = hash22(iuv);
        r = r.x > _Threshold || r.y > _Threshold ? 0.5 : r;
        r = r * 2.0 - 1.0;
        s = abs(r) < 0.001 ? 1 : sign(r);
        st = frac(abs(input.texcoord * s + r));
        positionSS = st * _InputTexture_TexelSize.zw;
        outColor += abs(r.x) < 0.001 || abs(r.y) < 0.001 ? 0 : LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;
        outColor = saturate(outColor);
        outColor = RgbToHsv(outColor);
        outColor.rg += r;
        outColor = HsvToRgb(outColor);
        
        outColor = lerp(baseColor,outColor, _Intensity);

        return float4(outColor, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Glitch"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
            #pragma fragment CustomPostProcess
            #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
