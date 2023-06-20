Shader "Unlit/Water"
{
    Properties
    {
        _ColorA("Shore Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _ColorB("Shallow Water Color", Color) = (0.0, 0.7, 1.0, 1.0)
        _ColorC("Deep Water Color", Color) = (0.0, 0.2, 1.0, 1.0)

        _FoamSize("Foam Size", Range(0.0, 1.0)) = 0.05
        _FoamFrequency("Foam Frequency", float) = 0.0
        _FoamDistance("Foam Distance", Range(0.0, 1.0)) = 0.0
        _FoamExponent("Foam Exponent", float) = 1.0
        _FoamSpeed("Foam Speed", float) = 0.1
        _FoamNoiseScale("Foam Noise Scale", float) = 1.0
        _FoamNoiseInfluence("Foam Noise Influence", float) = 1.0

        _WaterBlending("Water Blending", float) = 5.0

        _OffsetMagnitude("Offset Magnitude", float) = 0.02
        _OffsetCycle("Offset Cycle Duration", float) = 5.0
        _NoiseSize("Noise Size", float) = 0.0
        _NoiseIntensity("Noise Intensity", float) = 0.0

        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.8

        _BumpScale("Normal Scale", float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _UVSize("UV Size", float) = 10.0
        _UVScrolling("UV Scrolling", Vector) = (0.0, 0.0, 0.0, 0.0)
        _UVNoiseScale("UV Scrolling Noise Scale", float) = 0.0
        _UVNoiseSpeed("UV Scrolling Noise Speed", float) = 0.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #define _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _OCCLUSIONMAP
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local_fragment _SPECULAR_SETUP

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "WaterForwardPass.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Random.hlsl"

            float2 _UVScrolling;
            float _UVSize, _UVNoiseScale, _UVNoiseSpeed, _UVScrollPeriod;

            Varyings vert(Attributes input)
            {
                Varyings output = LitPassVertex(input);
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.depthPosition = output.positionCS;
                output.uv = output.positionWS.xz / _UVSize;
                return output;
            }

            float4 _ColorA, _ColorB, _ColorC;
            float _WaterBlending;
            float _OffsetMagnitude, _OffsetCycle;
            float _NoiseSize, _NoiseIntensity;

            float _FoamSize, _FoamFrequency, _FoamDistance, _FoamExponent, _FoamSpeed;
            float _FoamNoiseScale, _FoamNoiseInfluence;

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            float4 _NoiseTex_TexelSize;

            half4 frag(Varyings input) : SV_Target
            {
                float pos = dot(float2(_UVScrolling.y, -_UVScrolling.x), input.uv) * _UVNoiseScale;
                input.uv = input.uv + _UVScrolling * Noise(float2(pos, _Time[1] * _UVNoiseSpeed));

                // Unity LitForwardPass.hlsl Fragment Setup -----
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                #if defined(_PARALLAXMAP)
                #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
                half3 viewDirTS = input.viewDirTS;
                #else
                half3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, viewDirWS);
                #endif
                ApplyPerPixelDisplacement(viewDirTS, input.uv);
                #endif

                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(input.uv, surfaceData);

                // modifications of input and surface data to build up water effect.
                // --- [START WATER EFFECT] ---

                float2 depthUV = input.screenPos.xy / input.screenPos.w;
                float sceneDepth = LinearEyeDepth(SampleSceneDepth(depthUV), _ZBufferParams);
                float waterDepth = LinearEyeDepth(input.depthPosition.z / input.depthPosition.w, _ZBufferParams);
                float3 cameraDirection = normalize(input.positionWS - _WorldSpaceCameraPos);
                float depth = (sceneDepth - waterDepth) * -cameraDirection.y;

                float noise = Noise(input.positionWS.xz / _NoiseSize);
                float offset = noise * _NoiseIntensity;

                depth += (sin(_Time[1] * TAU / _OffsetCycle + offset) - 1.0) * _OffsetMagnitude;
                clip(depth);

                float foamNoise = Noise(input.uv * _FoamNoiseScale) * _FoamNoiseInfluence;

                float bands[] =
                {
                    (sin(TAU * (depth + _Time[1] * _FoamSpeed) * _FoamFrequency) * max(
                        (_FoamDistance - depth) / _FoamDistance, 0.0) - foamNoise) < _FoamSize,
                    pow(saturate((depth - _FoamSize) / _WaterBlending), 0.25),
                };

                surfaceData.albedo = lerp(_ColorA.rgb, lerp(_ColorB.rgb, _ColorC.rgb, bands[1]), bands[0]);

                // --- [END WATER EFFECT] ---

                // Unity LitForwardPass.hlsl mixing and output

                InputData inputData;
                InitializeInputData(input, surfaceData.normalTS, inputData);
                SETUP_DEBUG_TEXTURE_DATA(inputData, input.uv, _BaseMap);

                #ifdef _DBUFFER
                ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
                #endif

                half4 color = UniversalFragmentPBR(inputData, surfaceData);

                color.rgb = MixFog(color.rgb, inputData.fogCoord);
                color.a = OutputAlpha(color.a, _Surface) * max(pow(bands[1], 4.0), 1.0 - bands[0]);

                return color;
            }
            ENDHLSL
        }
    }
}