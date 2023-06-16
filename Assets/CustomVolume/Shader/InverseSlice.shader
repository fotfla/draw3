Shader "Hidden/Shader/InverseSlice"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_InputTexture);
    float4 _InputTexture_TexelSize;

    float _Angle;
    float2 _Offset;
    float _Width;
    float _Radius;

    int _ShapeType;

    float2 rotate(float2 p, float a){
        float c = cos(a),s = sin(a);
        return mul(float2x2(c,s,-s,c),p);
    }

    float2 pmod(float2 p, int n){
        float a = atan2(p.x,p.y) + PI / (float)n;
        float r = PI * 2.0 / (float)n;
        a = floor(a/r) * r;
        return rotate(p,-a);
    }

    float3 InvertColor(float3 color){
        color = saturate(1 - color);
        color = pow(color,2.2);
        return color;
    }

    bool InvertColorCalc(float2 uv, int type){
        if(type == 0){
            return 1;
        } else if(type == 1){
            uv = rotate(uv, _Angle * PI * 2);
            uv -= _Offset;
            return uv.x < uv.y;
        } else if(type == 2){
            uv.y *= _InputTexture_TexelSize.x * _InputTexture_TexelSize.w;
            float r = length(uv);
            float or = step(_Radius - _Width * 0.5, r);
            float ir = step(r, _Radius + _Width * 0.5);
            return or * ir > 0.5;
        } else if(type == 3){
            uv.y *= _InputTexture_TexelSize.x * _InputTexture_TexelSize.w;
            uv = rotate(uv, _Angle * PI * 2);
            uv = pmod(uv,3);
            float or = step(_Radius - _Width * 0.5, uv.y);
            float ir = step(uv.y, _Radius + _Width * 0.5);
            return or * ir > 0.5;
        } else if(type == 4){
            uv.y *= _InputTexture_TexelSize.x * _InputTexture_TexelSize.w;
            uv = rotate(uv, _Angle * PI * 2);
            uv = pmod(uv,4);
            float or = step(_Radius - _Width * 0.5, uv.y);
            float ir = step(uv.y, _Radius + _Width * 0.5);
            return or * ir > 0.5;
        } else if(type == 5){
            uv.y *= _InputTexture_TexelSize.x * _InputTexture_TexelSize.w;
            uv = rotate(uv, _Angle * PI * 2);
            uv.x = abs(uv.x) / 0.75 + 0.1;
            float or = step(_Radius - _Width, uv.x);
            float ir = step(uv.x, _Radius + _Width);
            return or * ir > 0.5;
        }
        return false;
    }

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _InputTexture_TexelSize.zw;// * _ScreenSize.xy;
        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;

        float2 uv = input.texcoord - 0.5;
        bool bInvert = InvertColorCalc(uv, _ShapeType);
        outColor = lerp(outColor, InvertColor(outColor), bInvert);

        return float4(outColor, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "InverseSlit"

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
