Shader "myShader/anime_Test"
{
	Properties
	{
		_Color("Main Color",color) = (1,1,1,1)
		_Outline("Thick of Outline",range(0,0.1)) = 0.02
		_MainTex("Texture", 2D) = "white" {}
		_OutlineColor("OutlineColor",color) = (0,1,1,1)
		_ToonMap("Ramp Map",2D) = "white"{}
		_ToonEffect("Toon Effect",range(0,1)) = 0.5
		_Factor("Factor",range(0,1)) = 0.5
	}
		SubShader
		{
			pass {
				Tags{"LightMode" = "ForwardBase"}
				Cull Back
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include"UnityCG.cginc"

				sampler2D _ToonMap;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _LightColor0;
				float4 _Color;
				float _ToonEffect;

				struct v2f {
					float4 pos:SV_POSITION;
					float3 lightDir:TEXCOORD0;
					float3 normal:TEXCOORD1;
					float2 texcoord:TEXCOORD2;
				};

				v2f vert(appdata_full v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.normal = v.normal;
					o.lightDir = ObjSpaceLightDir(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
				float4 frag(v2f i) :COLOR
				{
					float4 c = 1;
					float3 N = normalize(i.normal);
					float3 lightDir = normalize(i.lightDir);
					float diff = max(0, dot(N, lightDir));
					diff = (diff + 1) / 2;
					diff = smoothstep(0, 1, diff);
					float toon = tex2D(_ToonMap, float2(diff, 0.5)).r;
					diff = lerp(diff, toon, _ToonEffect);
					c = _Color * _LightColor0*(diff);
					c *= tex2D(_MainTex, i.texcoord);
					return c;
				}
			ENDCG
			}
				Pass
			{
				Tags{"LightMode" = "Always"}
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
}
