Shader "Hidden/Shader/Fluid"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_InputTexture);
    float4 _InputTexture_TexelSize;

    TEXTURE2D_X(_PrevCameraDepthTexture);

    float _Clamp;

    float4x4 _ViewProjM;
    float4x4 _PrevViewProjM;

    struct VaryingsF
    {
        float4 positionCS    : SV_POSITION;
        float4 uv            : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    VaryingsF VertF(Attributes input)
    {
        VaryingsF output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        #if _USE_DRAW_PROCEDURAL
            GetProceduralQuad(input.vertexID, output.positionCS, output.uv.xy);
        #else
            // output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
            // output.uv.xy = input.texcoord;
            output.positionCS = 0;
            output.uv = 0;
        #endif
        float4 projPos = output.positionCS * 0.5;
        projPos.xy = projPos.xy + projPos.w;
        output.uv.zw = projPos.xy;

        return output;
    }

    float2 ClampVelocity(float2 velocity, float maxVelocity)
    {
        float len = length(velocity);
        return (len > 0.0) ? min(len, maxVelocity) * (velocity * rcp(len)) : 0.0;
    }

    // Per-pixel camera velocity
    float2 GetCameraVelocity(float4 uv)
    {
        float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_PointClamp, uv.xy).r;

        #if UNITY_REVERSED_Z
            depth = 1.0 - depth;
        #endif

        depth = 2.0 * depth - 1.0;

        float3 viewPos = ComputeViewSpacePosition(uv.zw, depth, unity_CameraInvProjection);
        float4 worldPos = float4(mul(unity_CameraToWorld, float4(viewPos, 1.0)).xyz, 1.0);
        float4 prevPos = worldPos;

        float4 prevClipPos = mul(_PrevViewProjM, prevPos);
        float4 curClipPos = mul(_ViewProjM, worldPos);

        float2 prevPosCS = prevClipPos.xy / prevClipPos.w;
        float2 curPosCS = curClipPos.xy / curClipPos.w;

        return ClampVelocity(prevPosCS - curPosCS, _Clamp);
    }

    float4 CustomPostProcess(VaryingsF input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        uint2 positionSS = input.uv.xy * _InputTexture_TexelSize.zw;
        float2 velocity = GetCameraVelocity(input.uv);

        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;
        float depth = LOAD_TEXTURE2D_X(_CameraDepthTexture, positionSS).r;

        return float4(velocity,depth, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Fluid"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
            #pragma fragment CustomPostProcess
            #pragma vertex VertF
            ENDHLSL
        }
    }
    Fallback Off
}
