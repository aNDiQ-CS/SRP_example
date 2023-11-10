Shader "CustomSRP/Unlit"
{
	Properties
	{
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}

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