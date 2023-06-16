Shader "Hidden/Shader/EdgeDitection"
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

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _InputTexture_TexelSize.zw;

        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;
        float3 baseColor = outColor;
        float3 outColor0 = LOAD_TEXTURE2D_X(_InputTexture, positionSS + uint2(2,0)).xyz;
        float3 outColor1 = LOAD_TEXTURE2D_X(_InputTexture, positionSS + uint2(-2,0)).xyz;
        float3 outColor2 = LOAD_TEXTURE2D_X(_InputTexture, positionSS + uint2(0,-2)).xyz;
        float3 outColor3 = LOAD_TEXTURE2D_X(_InputTexture, positionSS + uint2(0,-2)).xyz;

        outColor = 4 * outColor - outColor0 - outColor1 - outColor2 - outColor3;
        outColor = saturate(outColor);
        outColor = lerp(baseColor,outColor,_Intensity);

        return float4(outColor, 1);
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
