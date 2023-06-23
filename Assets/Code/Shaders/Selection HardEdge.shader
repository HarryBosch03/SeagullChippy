Shader "Unlit/Selection [Hard Edge]"
{
    Properties
    {
        _Color("Color [RGB]", Color) = (1.0, 1.0, 1.0, 1.0)
        _Brightness("Brightness", float) = 2.0
        _Size("Expansion", float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        Cull Front

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

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float _Size;
            
            Varyings vert(Attributes input)
            {
                Varyings output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.vertex = TransformObjectToHClip(input.vertex.xyz + input.normal * _Size / 500.0);

                return output;
            }

            float4 _Color;
            float _Brightness;

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float4 col;
                col.rgb = max(_Color.rgb * _Brightness * exp(_Brightness), 0.0);
                col.a = 1.0;

                return col;
            }
            ENDHLSL
        }
    }
}