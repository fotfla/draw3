void Screen_float(in float2 uv, in float width, in float aspect, out float4 c){
    float w = 1 -  step(abs(uv.x - 0.5), 0.5 - width);
    float h = 1 -  step(abs(uv.y - 0.5), 0.5 - width * aspect);
    c = w + h;
}