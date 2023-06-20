const static unsigned int randomConstants[] =
{
    0x68E31DA4,
    0xB5297A4D,
    0x1B56C4E9,
};

const static float TAU =  2.0 * PI;

uint Rand(int input, int seed = 0)
{
    uint output = input;
    
    output *= randomConstants[0];
    output += seed;
    output ^= output >> 8;
    output += randomConstants[1];
    output ^= output << 8;
    output *= randomConstants[2];
    output ^= output >> 8;

    return output;
}

uint Rand(int2 p, int seed = 0)
{
    return Rand(p.x + 198491317 * p.y, seed);
}

float interpolate(float a, float b, float t)
{
    return (b - a) * ((t * (t * 6.0 - 15.0) + 10.0) * t * t * t) + a;
}

float Noise(float uv)
{
    int uv0 = floor(uv);
    return lerp(Rand(uv0), Rand(uv0 + 1), uv - uv0) / (float)~0u;
}

float Noise(float2 uv)
{
    int2 corners[4] =
    {
        floor(uv) + int2(0, 0),
        floor(uv) + int2(1, 0),
        floor(uv) + int2(0, 1),
        floor(uv) + int2(1, 1),
    };

    float2 samples[4];
    for (int i = 0; i < 4; i++)
    {
        float a = Rand(corners[i]) / (float)~0u * TAU;
        samples[i] = float2(cos(a), sin(a));
    }

    float dots[4];
    for (int i = 0; i < 4; i++) dots[i] = dot(samples[i], uv - corners[i]);

    float2 p = uv - corners[0];
    
    return interpolate(interpolate(dots[0], dots[1], p.x), interpolate(dots[2], dots[3], p.x), p.y) * 0.5 + 0.5;
}
