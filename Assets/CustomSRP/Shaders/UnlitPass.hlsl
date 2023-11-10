#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"

float4 Vertex(float3 positionOS : POSITION) : SV_POSITION
{
	float3 positionWS = TransformObjectToWorld(positionOS.xyz);
	return TransformWorldToHClip(positionWS);
}

float4 Fragment() : SV_TARGET
{
	return float4(1.0, 1.0, 0.0, 1.0);
}

#endif