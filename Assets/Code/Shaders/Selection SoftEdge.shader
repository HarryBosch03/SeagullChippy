Shader "Unlit/Selection [Soft Edge]"
{
    Properties
    {
        _Color("Color [RGB]", Color) = (1.0, 1.0, 1.0, 1.0)
        _Brightness("Brightness", float) = 2.0
        _Exponent("Fresnel Exponent", float) = 4.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        Blend One One

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : VIEW_DIR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 worldPos = TransformObjectToWorld(input.vertex.xyz);
                output.vertex = TransformWorldToHClip(worldPos.xyz);
                output.normal = TransformObjectToWorldNormal(input.normal.xyz);
                output.viewDir = _WorldSpaceCameraPos - worldPos;

                return output;
            }

            float4 _Color;
            float _Brightness, _Exponent;

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float4 col;
                col.rgb = _Color.rgb;
                col.a = 1.0;

                float3 v = normalize(input.viewDir);
                float3 n = normalize(input.normal);

                float d = 1.0 - dot(v, n);
                col.rgb *= _Brightness * exp(_Brightness) * pow(max(d, 0.0), _Exponent);

                return col;
            }
            ENDHLSL
        }
    }
}