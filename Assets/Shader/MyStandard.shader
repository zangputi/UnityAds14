// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "带阴影着色器/标准"
{
    Properties
    {
        _Color("颜色", Color) = (1,1,1,1)
        _MainTex("反照率", 2D) = "white" {}

        _Cutoff("透明度 Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("平滑度", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("平滑度 比例", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("金属质感", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("金属质感", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("高光 聚焦", Float) = 1.0
        [ToggleOff] _GlossyReflections("光泽 Reflections", Float) = 1.0

        _BumpScale("比例", Float) = 1.0
        [Normal] _BumpMap("法线 Map", 2D) = "bump" {}

        _Parallax ("高 比例", Range (0.005, 0.08)) = 0.02
        _ParallaxMap ("高 映射", 2D) = "black" {}

        _OcclusionStrength("强度", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("阻塞", 2D) = "white" {}

        _EmissionColor("颜色", Color) = (0,0,0)
        _EmissionMap("放射", 2D) = "white" {}

        _DetailMask("层次蒙片", 2D) = "white" {}

        _DetailAlbedoMap("反照中子 x2", 2D) = "grey" {}
        _DetailNormalMapScale("比例", Float) = 1.0
        [Normal] _DetailNormalMap("法线贴图", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
		
		//新颜色
		_Main_Texture ("Main_Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(1, 4)) = 1
        [MaterialToggle] _Unlit_OnOff ("Unlit_On/Off", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }

    CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG

    SubShader
    {
        Tags { "渲染类型"="不透明" "PerformanceChecks"="False"    
			 } 
        LOD 300


        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature_local _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex vertBase
            #pragma fragment fragBase
            #include "UnityStandardCoreForward.cginc"
            ENDCG
        }
        Tags { "Queue"="Transparent+100" }        // 通知unity重写物体的渲染深度顺序
        Pass
		{		
			Blend SrcAlpha  OneMinusSrcAlpha
			ZWrite Off
            ZTest Less
			Cull Off
			ColorMask RGB
			
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
				float distance = (_ShadowPlane.w - dot(_ShadowPlane.xyz, worldpos)) / dot(_ShadowPlane.xyz, lightdir.xyz);
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
				//color.w = _ShadowFalloff;
				// 另外的阴影衰减公式
				color.a = 1.0 - saturate(distance(i.xlv_TEXCOORD0, i.xlv_TEXCOORD1) * _ShadowFalloff);
				return color;
			}
			ENDCG
		}
		
		
		//新颜色
	/*	  
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Main_Texture; uniform float4 _Main_Texture_ST;
            uniform fixed _Unlit_OnOff;
            uniform float _Brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float2 node_140 = i.uv0;
                float4 node_2 = tex2D(_Main_Texture,TRANSFORM_TEX(node_140.rg, _Main_Texture));
                clip(node_2.a - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
////// Emissive:
                float3 emissive = (node_2.rgb*_Unlit_OnOff);
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * (_Brightness*(node_2.rgb*(1.0 - _Unlit_OnOff)));
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Main_Texture; uniform float4 _Main_Texture_ST;
            uniform fixed _Unlit_OnOff;
            uniform float _Brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// 法线:
                float3 normalDirection =  i.normalDir;
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float2 node_141 = i.uv0;
                float4 node_2 = tex2D(_Main_Texture,TRANSFORM_TEX(node_141.rg, _Main_Texture));
                clip(node_2.a - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// 灯光:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// 漫反射:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * (_Brightness*(node_2.rgb*(1.0 - _Unlit_OnOff)));
/// 自定义的颜色:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            Cull Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers flash d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _Main_Texture; uniform float4 _Main_Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float2 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_142 = i.uv0;
                float4 node_2 = tex2D(_Main_Texture,TRANSFORM_TEX(node_142.rg, _Main_Texture));
                clip(node_2.a - 0.5);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers flash d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _Main_Texture; uniform float4 _Main_Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_143 = i.uv0;
                float4 node_2 = tex2D(_Main_Texture,TRANSFORM_TEX(node_143.rg, _Main_Texture));
                clip(node_2.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
		*/
    }
	
	 //FallBack "Diffuse"
    // CustomEditor "ShaderForgeMaterialInspector"
     FallBack "VertexLit"
   //  CustomEditor "StandardShaderGUI"
}
