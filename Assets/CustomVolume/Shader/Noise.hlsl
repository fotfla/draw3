uint baseHash(uint2 p){
    const uint u = 1103515245U;
    p = u * ((p.xy >> 1U) ^ p.yx);
    uint h32 = u * ((p.x) ^ (p.y >> 3U));
    return h32^(h32>>16);
}

float hash12(uint2 p){
    uint n = baseHash(p);
    return (float)n * (1.0/float(0xffffffffU)); 
}

float2 hash22(uint2 p){
    uint n = baseHash(p);
    uint2 rz = uint2(n, n * 48271U);
    return float2((rz >> 1)&0x7fffffffU)/float(0x7fffffffU);
}