﻿Shader "GraphOutputTest"
{
	Properties
	{
		Vector1_Vector1_4B0AEC3F_Uniform("Smoothness", Float) = 0.5
		Vector1_Vector1_5370E8F3_Uniform("Metallic / Specular", Float) = 0.5
		Color_Color_8256B4E6_Uniform("Color", Color) = (0,0,0,0)

	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "LightweightPipeline" }

		LOD 200

		Pass
	{
		Tags
	{
		"LightMode" = "LightweightForward"
		"RenderType" = "Opaque"
		"Queue" = "Geometry"
	}

		Blend One Zero

		Cull Back

		ZTest LEqual

		ZWrite On


		CGPROGRAM
#pragma target 3.0
#include "UnityCG.cginc"

#pragma multi_compile _ _SINGLE_DIRECTIONAL_LIGHT _SINGLE_SPOT_LIGHT _SINGLE_POINT_LIGHT
#pragma multi_compile _ LIGHTWEIGHT_LINEAR
#pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
#pragma multi_compile _ LIGHTMAP_ON
#pragma multi_compile _ _LIGHT_PROBES_ON
#pragma multi_compile _ _HARD_SHADOWS _SOFT_SHADOWS _HARD_SHADOWS_CASCADES _SOFT_SHADOWS_CASCADES
#pragma multi_compile _ _VERTEX_LIGHTS
#pragma multi_compile_fog
#pragma multi_compile_instancing

#pragma vertex LightweightVertexCustom
#pragma fragment LightweightFragmentPBR
#pragma glsl
#pragma debug

#define _GLOSSYREFLECTIONS_ON


		float Vector1_Vector1_4B0AEC3F_Uniform;
	float Vector1_Vector1_5370E8F3_Uniform;
	float4 Color_Color_8256B4E6_Uniform;

	//sampler2D _MainTex;
	//float4 _MainTex_ST;

	struct vInput
	{
		float4 vertex : POSITION;
		float4 normal : NORMAL;
		float2 lightmapUV : TEXCOORD0;
		float4 color : COLOR;

		float2 texcoord : TEXCOORD1;

		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct vOutput
	{
		float4 posWS : TEXCOORD0;
		half4 viewDir : TEXCOORD1;
		half4 fogCoord : TEXCOORD2;
		half3 normal : TEXCOORD3;
		float4 hpos : SV_POSITION;

		float4 uv01 : TEXCOORD4;

		UNITY_VERTEX_OUTPUT_STEREO
	};

#include "UnityStandardInput.cginc"

	vOutput LightweightVertexCustom(vInput v)
	{
		vOutput o = (vOutput)0;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.uv01.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

		//o.uv01.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
#ifdef LIGHTMAP_ON
		o.uv01.zw = v.lightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
		o.hpos = UnityObjectToClipPos(v.vertex);

		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		o.posWS.xyz = worldPos;

		o.viewDir.xyz = normalize(_WorldSpaceCameraPos - worldPos);
		half3 normal = normalize(UnityObjectToWorldNormal(v.normal));

#if _NORMALMAP
		half sign = v.tangent.w * unity_WorldTransformParams.w;
		half3 tangent = normalize(UnityObjectToWorldDir(v.tangent));
		half3 binormal = cross(normal, tangent) * v.tangent.w;

		// Initialize tangetToWorld in column-major to benefit from better glsl matrix multiplication code
		o.tangentToWorld0 = half3(tangent.x, binormal.x, normal.x);
		o.tangentToWorld1 = half3(tangent.y, binormal.y, normal.y);
		o.tangentToWorld2 = half3(tangent.z, binormal.z, normal.z);
#else
		o.normal = normal;
#endif

		// TODO: change to only support point lights per vertex. This will greatly simplify shader ALU
#if defined(_VERTEX_LIGHTS) && defined(_MULTIPLE_LIGHTS)
		half3 diffuse = half3(1.0, 1.0, 1.0);
		// pixel lights shaded = min(pixelLights, perObjectLights)
		// vertex lights shaded = min(vertexLights, perObjectLights) - pixel lights shaded
		// Therefore vertexStartIndex = pixelLightCount;  vertexEndIndex = min(vertexLights, perObjectLights)
		int vertexLightStart = min(globalLightCount.x, unity_LightIndicesOffsetAndCount.y);
		int vertexLightEnd = min(globalLightCount.y, unity_LightIndicesOffsetAndCount.y);
		for (int lightIter = vertexLightStart; lightIter < vertexLightEnd; ++lightIter)
		{
			int lightIndex = unity_4LightIndices0[lightIter];
			LightInput lightInput;
			INITIALIZE_LIGHT(lightInput, lightIndex);

			half3 lightDirection;
			half atten = ComputeLightAttenuationVertex(lightInput, normal, worldPos, lightDirection);
			o.fogCoord.yzw += LightingLambert(diffuse, lightDirection, normal, atten);
		}
#endif

#if defined(_LIGHT_PROBES_ON) && !defined(LIGHTMAP_ON)
		o.fogCoord.yzw += max(half3(0, 0, 0), ShadeSH9(half4(normal, 1)));
#endif

		//UNITY_TRANSFER_FOG(o, o.hpos);
		return o;
	}

#include "CGIncludes/LightweightShadows.cginc"
#include "CGIncludes/LightweightBRDF.cginc"
#include "CGIncludes/LightweightCore.cginc"

	struct SurfacePBR
	{
		float3 Albedo;      // diffuse color
		float3 Specular;    // specular color
		float Metallic;		// metallic
		float3 Normal;      // tangent space normal, if written
		half3 Emission;
		half Smoothness;    // 0=rough, 1=smooth
		half Occlusion;     // occlusion (default 1)
		float Alpha;        // alpha for transparencies
	};

	SurfacePBR InitializeSurfacePBR()
	{
		SurfacePBR o;
		o.Albedo = float3(0.5, 0.5, 0.5);
		o.Specular = float3(0, 0, 0);
		o.Metallic = 0;
		o.Normal = float3(.5, .5, 1);
		o.Emission = 0;
		o.Smoothness = 0;
		o.Occlusion = 1;
		o.Alpha = 1;
		return o;
	}

	void DefineSurface(vOutput i, inout SurfacePBR o)
	{
		o.Albedo = Color_Color_8256B4E6_Uniform;
		o.Specular = Vector1_Vector1_5370E8F3_Uniform;
		o.Metallic = Vector1_Vector1_5370E8F3_Uniform;
		o.Smoothness = Vector1_Vector1_4B0AEC3F_Uniform;

	}

	half3 MetallicSetup(SurfacePBR o, out half3 specular, out half smoothness, out half oneMinusReflectivity)
	{
		smoothness = o.Smoothness;// metallicGloss.g;

								  // We'll need oneMinusReflectivity, so
								  //   1-reflectivity = 1-lerp(dielectricSpec, 1, metallic) = lerp(1-dielectricSpec, 0, metallic)
								  // store (1-dielectricSpec) in unity_ColorSpaceDielectricSpec.a, then
								  //   1-reflectivity = lerp(alpha, 0, metallic) = alpha + metallic*(0 - alpha) =
								  //                  = alpha - metallic * alpha
		half oneMinusDielectricSpec = _DieletricSpec.a;
		oneMinusReflectivity = oneMinusDielectricSpec - o.Metallic * oneMinusDielectricSpec;
		specular = lerp(_DieletricSpec.rgb, o.Albedo, o.Metallic);

		return o.Albedo * oneMinusReflectivity;
	}

	half3 SpecularSetup(SurfacePBR o, out half3 specular, out half smoothness, out half oneMinusReflectivity)
	{
		half4 specGloss = float4(o.Specular, o.Smoothness);

#if defined(UNITY_COLORSPACE_GAMMA) && defined(LIGHTWEIGHT_LINEAR)
		specGloss.rgb = LIGHTWEIGHT_GAMMA_TO_LINEAR(specGloss.rgb);
#endif

		specular = specGloss.rgb;
		smoothness = specGloss.a;
		oneMinusReflectivity = 1.0h - SpecularReflectivity(specular);
		return o.Albedo * (half3(1, 1, 1) - specular);
	}

	half4 LightweightFragmentPBR(vOutput i) : SV_Target
	{
		SurfacePBR o = InitializeSurfacePBR();
		DefineSurface(i, o);

		//float2 uv = i.uv01.xy;
		float2 lightmapUV = i.uv01.zw;

		half3 specColor;
		half smoothness;
		half oneMinusReflectivity;
		//#ifdef _METALLIC_SETUP
		half3 diffColor = MetallicSetup(o, specColor, smoothness, oneMinusReflectivity);
		//#else
		//half3 diffColor = SpecularSetup(o, specColor, smoothness, oneMinusReflectivity);
		//#endif

		diffColor = PreMultiplyAlpha(diffColor, o.Alpha, oneMinusReflectivity, /*out*/ o.Alpha);

		// Roughness is (1.0 - smoothness)²
		half perceptualRoughness = 1.0h - smoothness;

		// TODO - Actually handle normal
		half3 normal;
		CalculateNormal(i.normal, normal);

		// TODO: shader keyword for occlusion
		// TODO: Reflection Probe blend support.
		half3 reflectVec = reflect(-i.viewDir.xyz, normal);

		UnityIndirect indirectLight = LightweightGI(lightmapUV, i.fogCoord.yzw, reflectVec, o.Occlusion, perceptualRoughness);

		// PBS
		// grazingTerm = F90
		half grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));
		half fresnelTerm = Pow4(1.0 - saturate(dot(normal, i.viewDir.xyz)));
		half3 color = LightweightBRDFIndirect(diffColor, specColor, indirectLight, perceptualRoughness * perceptualRoughness, grazingTerm, fresnelTerm);
		half3 lightDirection;

	#ifndef _MULTIPLE_LIGHTS
		LightInput light;
		INITIALIZE_MAIN_LIGHT(light);
		half lightAtten = ComputeLightAttenuation(light, normal, i.posWS.xyz, lightDirection);

		//#ifdef _SHADOWS
		lightAtten *= ComputeShadowAttenuation(i.normal, i.posWS, _ShadowLightDirection.xyz);
		//#endif

		half NdotL = saturate(dot(normal, lightDirection));
		half3 radiance = light.color * (lightAtten * NdotL);
		color += LightweightBDRF(diffColor, specColor, oneMinusReflectivity, perceptualRoughness, normal, lightDirection, i.viewDir.xyz) * radiance;
	#else
		//#ifdef _SHADOWS
		half shadowAttenuation = ComputeShadowAttenuation(i.normal, i.posWS, _ShadowLightDirection.xyz);
		//#endif
		int pixelLightCount = min(globalLightCount.x, unity_LightIndicesOffsetAndCount.y);
		for (int lightIter = 0; lightIter < pixelLightCount; ++lightIter)
		{
			LightInput light;
			int lightIndex = unity_4LightIndices0[lightIter];
			INITIALIZE_LIGHT(light, lightIndex);
			half lightAtten = ComputeLightAttenuation(light, normal, i.posWS.xyz, lightDirection);
			//#ifdef _SHADOWS
			lightAtten *= max(shadowAttenuation, half(lightIndex != _ShadowData.x));
			//#endif
			half NdotL = saturate(dot(normal, lightDirection));
			half3 radiance = light.color * (lightAtten * NdotL);

			color += LightweightBDRF(diffColor, specColor, oneMinusReflectivity, perceptualRoughness, normal, lightDirection, i.viewDir.xyz) * radiance;
		}
	#endif

		color += o.Emission;
		UNITY_APPLY_FOG(i.fogCoord, color);
		return OutputColor(color, o.Alpha);
	}

		ENDCG
	}

		Pass
	{
		Tags{ "Lightmode" = "ShadowCaster" }
		ZWrite On ZTest LEqual

		CGPROGRAM
#pragma target 2.0
#include "UnityCG.cginc"
#include "CGIncludes/LightweightPass.cginc"
#pragma vertex shadowVert
#pragma fragment shadowFrag
		ENDCG
	}

		Pass
	{
		Tags{ "Lightmode" = "DepthOnly" }
		ZWrite On

		CGPROGRAM
#pragma target 2.0
#include "UnityCG.cginc"
#include "CGIncludes/LightweightPass.cginc"
#pragma vertex depthVert
#pragma fragment depthFrag
		ENDCG
	}
	}


		//FallBack "Diffuse"
		CustomEditor "LegacyIlluminShaderGUI"
}