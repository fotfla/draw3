Shader "Hidden/Shader/TextureOverray"
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

    TEXTURE2D_X(_OverRayTexture);
    float4 _OverRayTexture_TexelSize;

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _InputTexture_TexelSize.zw;

        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;

        uint2 positionOSS = input.texcoord * _OverRayTexture_TexelSize.zw;
        float4 overray = LOAD_TEXTURE2D_X(_OverRayTexture, positionOSS);
        if(overray.w < 0.1) overray = 0;

        return float4(outColor + overray * _Intensity, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "New Post Process Shader"

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
