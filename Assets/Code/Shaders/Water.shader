Shader "Unlit/Water"
{
    Properties
    {
        _ColorA("Shore Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _ColorB("Shallow Water Color", Color) = (0.0, 0.7, 1.0, 1.0)
        _ColorC("Deep Water Color", Color) = (0.0, 0.2, 1.0, 1.0)
        _WaterBlending("Water Blending", Vector) = (0.0, 0.0, 0.0, 0.0)

        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.8

        _Heightmap("Ocean Heightmap (BW)", 2D) = "gray" {}
        _NormalScale("Normal Intensity", float) = 0.6
        _NormalRelief("Normal Relief", float) = 8.0

        _Speed("UV Scroll Speed", Vector) = (0.0, 0.0, 0.0, 0.0)
        
        _Density("Density", float) = 1.0

        _UVSize("UV Size", float) = 10.0
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
            float _UVSize;

            Varyings vert(Attributes input)
            {
                Varyings output = LitPassVertex(input);
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.depthPosition = output.positionCS;
                output.uv = output.positionWS.xz / _UVSize;
                return output;
            }

            float4 _ColorA, _ColorB, _ColorC;
            float2 _WaterBlending;
            float _NormalScale, _NormalRelief;

            float2 _Speed;
            float _Density;

            TEXTURE2D(_Heightmap);
            SAMPLER(sampler_Heightmap);
            float4 _Heightmap_TexelSize;

            float4 getHeight(float2 uv)
            {
                float s1 = SAMPLE_TEXTURE2D(_Heightmap, sampler_Heightmap, uv + _Time[1] * _Speed);
                float s2 = SAMPLE_TEXTURE2D(_Heightmap, sampler_Heightmap, uv + _Time[1] * _Speed / 2.0);

                return (s1 + s2) / 2.0;
            }
            
            float3 normalFromBump(float2 uv, float scale)
            {
                float2 f = _Heightmap_TexelSize.xy * _NormalRelief;

                float4 samples =
                {
                    getHeight(uv - float2(f.x, 0.0)).r,
                    getHeight(uv + float2(f.x, 0.0)).r,
                    getHeight(uv - float2(0.0, f.y)).r,
                    getHeight(uv + float2(0.0, f.y)).r,
                };

                float3 val;
                val.xy = (samples.yw - samples.xz) / f.xy * scale;
                val.z = 1.0;
                return normalize(val) * 0.5 + 0.5;
            }

            half4 frag(Varyings input) : SV_Target
            {
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
                float rawDepth = (sceneDepth - waterDepth);
                float depth = rawDepth * -cameraDirection.y;

                float foamNoise = getHeight(input.uv * 10.0);
                
                float height = getHeight(input.uv);
                depth -= height * 0.1;
                depth += foamNoise * 0.02;
                clip(depth);


                float2 bands =
                {
                    depth - pow(foamNoise, 2) * 0.4 + 0.01 > _WaterBlending.x,
                    pow(saturate((depth - foamNoise * 0.1) / _WaterBlending.y), 0.5),
                };

                float4 finalColor = lerp(_ColorA, lerp(_ColorB, _ColorC, bands.y), bands.x);
                surfaceData.albedo = finalColor.rgb;
                surfaceData.normalTS = normalFromBump(input.uv, _NormalScale);
                surfaceData.normalTS = normalize(lerp(float3(0.0, 0.0, 1.0), surfaceData.normalTS, bands.y / 15.0));

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
                color.a = lerp(1.0, saturate(1.0 - saturate(exp(-pow(depth * _Density, 2.0))) + 0.2), bands.x);
                
                return color;
            }
            ENDHLSL
        }
    }
}