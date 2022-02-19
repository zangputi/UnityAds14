Shader "Custom/Shadow"
{
    Properties
    {
		_GroundHeight("Ground Height", Float) = 0
		_ShadowColor("Shadow Color", Color) = (0,0,0,1)
		_ShadowFalloff("Shadow Falloff", Range(0, 1)) = 1

    }
    SubShader
    {
		Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}

        Pass
        {
			// 用使用模板测试以保证alpha显示正确
			Stencil
			{
				Ref 0
				Comp equal
				Pass incrWrap
				Fail keep
				ZFail keep
			}

			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off

			// 深度稍微偏移防止阴影与地面穿插
			Offset -1, 0

			CGPROGRAM

			#pragma vertex Vertex
			#pragma fragment Fragment

			#include "UnityCG.cginc"

			struct VertexInput
			{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
			};

			struct VertexOutput
			{
				half4 vertex : SV_POSITION;
				half4 color : COLOR;
				half3 normalWS : TEXCOORD0;
			};

			half _GroundHeight;
			half4 _ShadowColor;
			half _ShadowFalloff;

			inline half3 ShadowProjectPos(half4 vertPos)
			{
				half3 shadowPos;

				// 得到顶点的世界空间坐标
				half3 worldPos = mul(unity_ObjectToWorld, vertPos).xyz;

				half3 worldLightDir = UnityWorldSpaceLightDir(worldPos);
	
				// 阴影的世界空间坐标（低于地面的部分不做改变）
				shadowPos.y = min(worldPos.y, _GroundHeight);
				shadowPos.xz = worldPos.xz - worldLightDir.xz * max(0, worldPos.y - _GroundHeight) / worldLightDir.y;

				return shadowPos;
			}

			VertexOutput Vertex(VertexInput input)
			{
				VertexOutput output = (VertexOutput)0;

				// 得到阴影的世界空间坐标
				half3 shadowPos = ShadowProjectPos(input.vertex);

				// 转换到裁切空间
				output.vertex = mul(unity_MatrixVP, float4(shadowPos, 1.0));

				// 阴影颜色
				output.color = _ShadowColor;
				output.color.a *= _ShadowFalloff;

				return output;
			}

			half4 Fragment(VertexOutput input) : SV_Target
			{
				return input.color;
			}

			ENDCG
        }
    }
}
