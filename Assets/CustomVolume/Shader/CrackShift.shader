Shader "Hidden/Shader/CrackShift"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

    float _Intensity;
    TEXTURE2D_X(_InputTexture);
    float4 _InputTexture_TexelSize;

    int _Seed;
    int _Div;

    float rnd(float2 uv){
        return frac(45636.631 * sin(dot(uv,float2(21.536,25.635))));
    }

    float2x2 rot(float a){
        float c= cos(a),s = sin(a);
        return float2x2(c,s,-s,c);
    }

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = input.texcoord - 0.5;
        uint2 seed = uint2(fmod(_Seed * 65536 , 289),floor(_Seed * 65536 / 289.0));
        float r = rnd(_Seed);
        uv = mul(rot(r * PI * 2.0),uv);

        uint block = abs(_Div) == 0 ? 1 : abs(_Div);
        uv.x *= (float)block;
        
        float2 iuv = floor(uv);
        uv.x /= (float)block;
        uv.y += (rnd(iuv.x) * 2.0 - 1.0) * _Intensity;
        uv = mul(rot(-r * PI * 2.0),uv);
        uv += 0.5;

        uint2 positionSS = frac(uv) * _InputTexture_TexelSize.zw;

        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;

        return float4(outColor, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "CrackShift"

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
