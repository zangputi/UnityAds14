Shader "Custom/Outline"
{
    Properties
    {
        _OutlineColor("描边颜色", Color) = (0,0,0,1)
        _OutlineWidth("描边粗细", Range(0, 1)) = 0.3
        _Offset("偏移", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent-100"}

        Pass
        {
            ZWrite Off
            Cull Front

            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex Vertex
            #pragma fragment Fragment

            struct VertexInput
            {
                half4 position : POSITION;
                half3 normal : NORMAL;
            };

            struct VertexOutput
            {
                half4 position : SV_POSITION;
            };

            half _OutlineWidth;
            half _Offset;

            VertexOutput Vertex(VertexInput input)
            {
                VertexOutput output;

                half3 WorldPosition = mul(unity_ObjectToWorld, input.position).xyz;
                half3 CameraDir = normalize(_WorldSpaceCameraPos.xyz - WorldPosition);
                WorldPosition += CameraDir * _Offset;
                half4 positionCS = mul(unity_MatrixVP, float4(WorldPosition, 1.0));

                // 将法线变换到NDC空间
                half3 viewNormal = mul((half3x3)UNITY_MATRIX_IT_MV, input.normal.xyz);
                half2 ndcNormal = normalize(mul((half2x2)UNITY_MATRIX_P, viewNormal.xy));

                // 将近裁剪面右上角位置的顶点变换到观察空间
                half4 nearUpperRight = mul(unity_CameraInvProjection, half4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));

                // 求得屏幕宽高比
                half aspect = abs(nearUpperRight.y / nearUpperRight.x);
                ndcNormal.x *= aspect;

                half4 posSV = positionCS;
                posSV.xy += _OutlineWidth * 0.01 * ndcNormal * positionCS.w;

                output.position = posSV;

                return output;
            }

            half4 _OutlineColor;

            half4 Fragment(VertexOutput input) : SV_TARGET
            {
                return _OutlineColor;
            }

            ENDCG
        }
    }
}