void Figure_float(in float2 uv, in float aspect, in int kind, out float4 c){
    c = 1;
}

void Invert_float(in int kind, in float3 incolor, out float3 outcolor)
{
    if (kind > 0)
    {
        outcolor = 1 - incolor;
    }
    else
    {
        outcolor = incolor;
    }
}