Shader "Ebitender/TestPostProcess"
{
	Properties
	{ }

	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

		LOD 100
		ZWrite Off Cull Off
		Pass
		{
			HLSLPROGRAM

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
			{
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
#if UNITY_REVERSED_Z
                float depth = SampleSceneDepth(input.texcoord);
#else
                float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(input.texcoord));
#endif
                half4 color = half4(depth, depth, depth, 1);

#if UNITY_REVERSED_Z
                if (depth < 0.0001)
                    return half4(0, 0, 0, 1);
#else
                if (depth > 0.9999)
                    return half4(0, 0, 0, 1);
#endif
                return color;
			}

			ENDHLSL
		}

		Pass
		{
            HLSLPROGRAM

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

			half4 frag(Varyings input) : SV_Target
			{
				float3 normal = SampleSceneNormals(input.texcoord);
	            half4 color = half4(normal.xyz, 1);
				return color;
			}
			ENDHLSL
		}

				Pass
		{
			HLSLPROGRAM

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

			#pragma vertex Vert
			#pragma fragment frag

			TEXTURE2D_X(_CameraOpaqueTexture);
			SAMPLER(sampler_CameraOpaqueTexture);

			half4 frag(Varyings input) : SV_Target
			{
				float4 color = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.texcoord);
				return color;
			}
			ENDHLSL
		}
	}
}