Shader "CustomSRP/Unlit"
{
	Properties {}

	SubShader
	{
		Pass
		{
			HLSLPROGRAM
			#pragma vertex Vertex
			#pragma fragment Fragment
			#include "UnlitPass.hlsl"
			ENDHLSL
		}
	}
}