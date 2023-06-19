Shader "Unlit/Water"
{
    Properties
    {
        _ColorA("Shore Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _ColorB("Shallow Water Color", Color) = (0.0, 0.7, 1.0, 1.0)
        _ColorC("Deep Water Color", Color) = (0.0, 0.2, 1.0, 1.0)

        _ShoreOffset("Shore Distance", Range(0.0, 1.0)) = 0.05
        _WaterBlending("Water Blending", float) = 5.0

        _OffsetMagnitude("Offset Magnitude", float) = 0.02
        _OffsetCycle("Offset Cycle Duration", float) = 5.0
        _NoiseSize("Noise Size", float) = 0.0
        _NoiseIntensity("Noise Intensity", float) = 0.0

        _NoiseTex("Noise Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
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
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Random.hlsl"

            struct ExtendedVaryings
            {
                float4 positionCS : SV_POSITION;
                Varyings varyings : VAR_VARYINGS;
                float4 screenPos : VAR_SCREENPOS;
            };

            ExtendedVaryings vert(Attributes input)
            {
                ExtendedVaryings output;
                output.varyings = LitPassVertex(input);

                output.positionCS = output.varyings.positionCS;
                output.screenPos = ComputeScreenPos(output.varyings.positionCS);
                return output;
            }

            float4 _ColorA, _ColorB, _ColorC;
            float _ShoreOffset;
            float _WaterBlending;
            float _OffsetMagnitude, _OffsetCycle;
            float _NoiseSize, _NoiseIntensity;

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            float4 _NoiseTex_TexelSize;

            half4 frag(ExtendedVaryings input) : SV_Target
            {
                // Unity LitForwardPass.hlsl Fragment Setup -----
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                #if defined(_PARALLAXMAP)
                #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
                half3 viewDirTS = input.varyings.viewDirTS;
                #else
                half3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, viewDirWS);
                #endif
                ApplyPerPixelDisplacement(viewDirTS, input.varyings.uv);
                #endif

                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(input.varyings.uv, surfaceData);

                InputData inputData;
                InitializeInputData(input.varyings, surfaceData.normalTS, inputData);
                SETUP_DEBUG_TEXTURE_DATA(inputData, input.uv, _BaseMap);

                #ifdef _DBUFFER
                ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
                #endif

                // modifications of input and surface data to build up water effect.
                // --- [START WATER EFFECT] ---

                float2 depthUV = input.screenPos.xy / input.screenPos.w;
                float sceneDepth = LinearEyeDepth(SampleSceneDepth(depthUV), _ZBufferParams);
                float depth = LinearEyeDepth(input.varyings.positionCS.z / input.varyings.positionCS.w, _ZBufferParams);
                float diff = sceneDepth - depth;
 
                float noise = Noise(input.varyings.positionWS.xz / _NoiseSize);
                float offset = noise * _NoiseIntensity;

                diff += (sin(_Time[1] * TAU / _OffsetCycle + offset) - 1.0) * _OffsetMagnitude;
                clip(diff);

                float bands[] =
                {
                    saturate(diff - _ShoreOffset > 0.0),
                    pow(saturate((diff - _ShoreOffset) / _WaterBlending), 0.25),
                };
                surfaceData.albedo = lerp(_ColorA, lerp(_ColorB, _ColorC, bands[1]), bands[0]);
                surfaceData.smoothness = 1.0;

                //float3 normalTS = float3(noise(input.varyings.positionWS / _NoiseSize));
                
                // --- [END WATER EFFECT] ---

                // Unity LitForwardPass.hlsl mixing and output
                half4 color = UniversalFragmentPBR(inputData, surfaceData);

                color.rgb = MixFog(color.rgb, inputData.fogCoord);
                color.a = OutputAlpha(color.a, _Surface);

                return color;
            }
            ENDHLSL
        }
    }
}