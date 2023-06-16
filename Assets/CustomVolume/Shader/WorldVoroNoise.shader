Shader "Hidden/Shader/WorldVoroNoise"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    // #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Packages/fotfla/ShaderLibrary/Noise.cginc"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
        float3 ray : TEXCOORD3;
    };

    float4x4 _InverseProjectionMatrix;
    float _NoiseScale;
    float _NoiseIntensity;
    int _Seed;

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);

        float2 sp  = output.texcoord;
        sp.xy = sp.xy * 2.0 - 1.0;

        float far = _ProjectionParams.z;
        float3 clipVec = float3(sp.xy,1.0) * far;
        output.ray = mul(_InverseProjectionMatrix,clipVec.xyzz).xyz;
        return output;
    }

    float _Intensity;
    TEXTURE2D_X(_InputTexture);
    float4 _InputTexture_TexelSize;

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _InputTexture_TexelSize.zw;

        float depth = LOAD_TEXTURE2D_X(_CameraDepthTexture, positionSS).x;
        float linear01Depth = Linear01Depth(depth,_ZBufferParams);
        
        float3 viewPos = input.ray * linear01Depth;
        float3 pos = mul(UNITY_MATRIX_I_V, float4(viewPos,1)).xyz;

        float3 positionWS = GetAbsolutePositionWS(pos);

        float3 vn = abs(voronoise(positionWS * _NoiseScale));
        uint2 vnseed = floor(vn);
        float2 h = (hash32(vnseed + _Seed) - 0.5) * 2.0;

        if(linear01Depth >= 1) vn = 0;

        float2 uv = input.texcoord + h * _NoiseIntensity * _Intensity;

        positionSS = uv * _InputTexture_TexelSize.zw;

        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;

        // float3 hsvColor = RgbToHsv(outColor);
        // hsvColor.x += vn.y * 4.0;
        // hsvColor.yz = abs(vn.y) < 0.001 || abs(vn.z) < 0.001 ? hsvColor.yz : 0.9;
        // hsvColor = saturate(hsvColor);
        // outColor = HsvToRgb(hsvColor);
        // return float4(positionWS,1);
        return float4(outColor, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "WorldVoroNoise"

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
