Shader "outline"
{
    //法线外扩实现描边
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
        //描边阶段，法线外扩，渲染背面
        Pass
        {
            //只需要边缘外扩
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
                //把法线转换到视图空间
                float3 vnormal = mul((float3x3)UNITY_MATRIX_IT_MV,v.normal);
                //把法线转换到投影空间
                float2 pnormal_xy = mul((float2x2)UNITY_MATRIX_P,vnormal.xy);
                //朝法线方向外扩
                o.vertex.xy = o.vertex.xy + pnormal_xy * _Outline;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
        //正常阶段
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
                // _ShadowPlane.w = p0 * n  // 平面的w分量就是p0 * n
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
                // 下面两种阴影衰减公式都可以使用(当然也可以自己写衰减公式)
                // 王者荣耀的衰减公式
                //color.w = (pow(0.5, _ShadowFadeParams.y) * _ShadowFadeParams.z);
                color.w = _ShadowFalloff;
                // 另外的阴影衰减公式
                //color.a = 1.0 - saturate(distance(i.xlv_TEXCOORD0, i.xlv_TEXCOORD1) * _ShadowFalloff);
                return color;
            }
            ENDCG
        }
    }
}