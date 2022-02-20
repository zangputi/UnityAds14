Shader "Custom/OldOutline"
{
    //��������ʵ�����
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Outline("Outline",float) = 0.1
        _OutlineColor("OutlineColor",Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        //��߽׶Σ�������������Ⱦ����
        Pass
        {
            //ֻ��Ҫ��Ե����
            Cull Front
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            float _Outline;
            float4 _OutlineColor;           
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //�ѷ���ת������ͼ�ռ�
                float3 vnormal = mul((float3x3)UNITY_MATRIX_IT_MV,v.normal);
                //�ѷ���ת����ͶӰ�ռ�
                float2 pnormal_xy = mul((float2x2)UNITY_MATRIX_P,vnormal.xy);
                //�����߷�������
                o.vertex.xy = o.vertex.xy + pnormal_xy * _Outline;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
        //�����׶�
        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv :TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv:TEXCOORD0;
            };
            sampler2D _MainTex;     
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }   
            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex,i.uv);
            }
            ENDCG
        }
         Pass
        {		
            Blend SrcAlpha  OneMinusSrcAlpha
            ZWrite Off
            // ZTest Less
            // Cull Off
            // ColorMask RGB
            
            Stencil
            {
                Ref 0			
                Comp Equal			
                WriteMask 255		
                ReadMask 255
                Pass Invert
                Fail Keep
                ZFail Keep
            }
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            float4 _ShadowPlane;
            float4 _ShadowProjDir;
            float4 _WorldPos;
            float _ShadowInvLen;
            float4 _ShadowFadeParams;
            float _ShadowFalloff;
            float4 _ShadowColor;
            
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 xlv_TEXCOORD0 : TEXCOORD0;
                float3 xlv_TEXCOORD1 : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3 lightdir = normalize(_ShadowProjDir);
                float3 worldpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // _ShadowPlane.w = p0 * n  // ƽ���w��������p0 * n
                float distance = (_ShadowPlane.w - dot(_ShadowPlane.xzy, worldpos)) / dot(_ShadowPlane.xzy, lightdir.xyz);
                worldpos = worldpos + distance * lightdir.xyz;
                o.vertex = mul(unity_MatrixVP, float4(worldpos, 1.0));
                o.xlv_TEXCOORD0 = _WorldPos.xyz;
                o.xlv_TEXCOORD1 = worldpos;
                return o;
            }
            
            float4 frag(v2f i) : COLOR
            {
                //float3 posToPlane_2 = (i.xlv_TEXCOORD0 - i.xlv_TEXCOORD1);
                float4 color;
                color.rgb = float3(0,0,0);
                // ����������Ӱ˥����ʽ������ʹ��(��ȻҲ�����Լ�д˥����ʽ)
                // ������ҫ��˥����ʽ
                //color.w = (pow(0.5, _ShadowFadeParams.y) * _ShadowFadeParams.z);
                color.w = _ShadowFalloff;
                // �������Ӱ˥����ʽ
                //color.a = 1.0 - saturate(distance(i.xlv_TEXCOORD0, i.xlv_TEXCOORD1) * _ShadowFalloff);
                return color;
            }
            ENDCG
        }
    }
}