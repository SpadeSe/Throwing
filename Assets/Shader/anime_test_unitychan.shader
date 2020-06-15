Shader "myShader/anime_test_unitychan"
{
	Properties
	{
		_Color("Main Color",color) = (1,1,1,1)
		_Outline("Thick of Outline",range(0,0.1)) = 0.02
		_MainTex("Diffuse", 2D) = "white" {}
		_FalloffSampler("Falloff Control", 2D) = "white" {}
		_RimLightSampler("RimLight Control", 2D) = "white" {}
		_Factor("Factor",range(0,1)) = 0.5
		_ShadowColor("Shadow Color", Color) = (0.8, 0.8, 1, 1)
	}
		SubShader
		{
			Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
			"LightMode" = "ForwardBase"
		}
			pass {
				Cull Back
				CGPROGRAM
				#pragma multi_compile_fwdbase
				#pragma vertex vert
				#pragma fragment frag
				#include"UnityCG.cginc"
				#include "AutoLight.cginc"

				sampler2D _MainTex;
				sampler2D _FalloffSampler;
				sampler2D _RimLightSampler;
				sampler2D _SpecularReflectionSampler;
				sampler2D _EnvMapSampler;
				sampler2D _NormalMapSampler;

				float4 _MainTex_ST;
				float4 _LightColor0;
				float4 _Color;
				float4 _ShadowColor;
				float _SpecularPower;

				#define FALLOFF_POWER 0.3

				struct v2f {
					float4 pos:SV_POSITION;
					LIGHTING_COORDS(0, 1)
					float2 uv : TEXCOORD2;
					float3 eyeDir : TEXCOORD3;
					float3 lightDir : TEXCOORD4;
					float3 normal:TEXCOORD5;
				};

				v2f vert(appdata_tan v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
					o.normal = normalize(mul(unity_ObjectToWorld,half4(v.normal,0)).xyz);
					half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.eyeDir.xyz = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz).xyz;
					o.lightDir = WorldSpaceLightDir(v.vertex);
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}
				half3 GetOverlayColor(half3 inUpper, half3 inLower)
				{
					half3 OneMinusLower = half3(1.0, 1.0, 1.0) - inLower;
					half3 valUnit = 2.0 * OneMinusLower;
					half3 minValue = 2.0 * inLower - half3(1.0, 1.0, 1.0);
					half3 greaterResult = inUpper * valUnit + minValue;
					half3 lowerResult = 2.0 * inLower * inUpper;
					half3 lerpVals = round(inLower);
					return lerp(lowerResult, greaterResult, lerpVals);
				}

				float4 frag(v2f i) :COLOR
				{
					//光照衰减
					half4 diffSamplerColor = tex2D(_MainTex,i.uv.xy);
					half3 normalVec = i.normal;
					half normalDotEye = dot(normalVec, i.eyeDir.xyz);
					half falloffU = clamp(1.0 - abs(normalDotEye), 0.02, 0.98);
					half4 falloffSamplerColor = FALLOFF_POWER * tex2D(_FalloffSampler, float2(falloffU, 0.25f));
					half3 shadowColor = diffSamplerColor.rgb * diffSamplerColor.rgb;
					half3 combinedColor = lerp(diffSamplerColor.rgb, shadowColor, falloffSamplerColor.r);
					combinedColor *= (1.0 + falloffSamplerColor.rgb * falloffSamplerColor.a);
					//高光反射
					half4 reflectionMaskColor = tex2D(_SpecularReflectionSampler, i.uv.xy);
					half specularDot = dot(normalVec,i.eyeDir.xyz);
					half4 lighting = lit(normalDotEye, specularDot, _SpecularPower);
					half3 specularColor = saturate(lighting.z) * reflectionMaskColor.rgb * diffSamplerColor.rgb;
					combinedColor += specularColor;

					//反射
					half3 reflectVector = reflect(-i.eyeDir.xyz, normalVec).xzy;
					half2 sphereMapCoords = 0.5 * (half2(1.0, 1.0) + reflectVector.xy);
					half3 reflectColor = tex2D(_EnvMapSampler, sphereMapCoords).rgb;
					reflectColor = GetOverlayColor(reflectColor, combinedColor);

					combinedColor = lerp(combinedColor, reflectColor, reflectionMaskColor.a);
					combinedColor *= _Color.rgb * _LightColor0.rgb;
					float opacity = diffSamplerColor.a * _Color.a * _LightColor0.a;

					//阴影
					shadowColor = _ShadowColor.rgb * combinedColor;
					half attenuation = saturate(2.0 * LIGHT_ATTENUATION(i) - 1.0);
					combinedColor = lerp(shadowColor, combinedColor, attenuation);

					//轮廓光
					half rimlightDot = saturate(0.5*(dot(normalVec, i.lightDir) + 1.0));
					falloffU = saturate(rimlightDot * falloffU);
					falloffU = tex2D(_RimLightSampler, float2(falloffU, 0.25f)).r;
					half3 lightColor = diffSamplerColor.rgb;
					combinedColor += falloffU * lightColor;
					return float4(combinedColor, opacity);
				}
			ENDCG
			}
				Pass
			{
				Cull Front
				ZWrite On
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include"UnityCG.cginc"

				float _Outline;
				float _Factor;
				float4 _OutlineColor;
				float4 _Color;
				float4 _LightColor0;
				sampler2D _MainTex;
				#define SATURATION_FACTOR 0.6
				#define BRIGHTNESS_FACTOR 0.8
				struct v2f {
					float4 pos:SV_POSITION;
					float2 uv:TEXCOORD0;
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					float3 dir = normalize(v.vertex.xyz);
					float3 dir2 = v.normal;
					float D = dot(dir, dir2);
					dir = dir * sign(D);
					dir = dir * _Factor + dir2 * (1 - _Factor);
					v.vertex.xyz += dir * _Outline;
					o.pos = UnityObjectToClipPos(v.vertex);
					return o;
				}
				/*float4 frag() :COLOR
				{
					float4 c = _OutlineColor;
					return c;
				}*/
				float4 frag(v2f i) :COLOR{
					float4 diffuseMapColor = tex2D(_MainTex, i.uv);

					float maxChan = max(max(diffuseMapColor.r, diffuseMapColor.g), diffuseMapColor.b);
					float4 newMapColor = diffuseMapColor;

					maxChan -= (1.0 / 255.0);
					float3 lerpVals = saturate((newMapColor.rgb - float3(maxChan, maxChan, maxChan)) * 255.0);
					newMapColor.rgb = lerp(SATURATION_FACTOR * newMapColor.rgb, newMapColor.rgb, lerpVals);

					return float4(BRIGHTNESS_FACTOR * newMapColor.rgb * diffuseMapColor.rgb, diffuseMapColor.a) * _Color * _LightColor0;
				}
				ENDCG
			}

		}
			FallBack"Diffuse"
}
