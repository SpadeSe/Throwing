Shader "myShaders/water_shader_Transparent"
{
    Properties
    {
        _Diff_Set("漫反射调节",range(0,10)) = 1.71
		_SpecSet("高光调节",range(0,10)) = 3.33
		_SpecRange("高光范围",range(0,10)) = 1.55
		_NormalMap("法线贴图",2D) = "bump"{}
		[Space][Space][Space][Space][Space][Space][Space][Space]
		_uv_scale_r("uv大小调节R",range(0,3)) = 0.5
		_uv_scale_g("uv大小调节G",range(0,3)) = 0.5
		_uv_scale_b("uv大小调节B",range(0,3)) = 0.5
		_uv_Speed_r("uv速度调节R",range(0,1)) = 0.05
		_uv_Speed_g("uv速度调节G",range(0,1)) = 0.005
		_uv_Speed_b("uv速度调节B",range(0,1)) = 0.01
		_normal_r("法线强度R",range(0,3)) = 1
		_normal_g("法线强度G",range(0,3)) = 1
		_normal_b("法线强度B",range(0,3)) = 1
		[Space][Space][Space][Space][Space][Space][Space][Space]
		_water_color("水颜色",Color) = (0,0.5656692,0.8455882,1)
		[HideInInspector]_fre_int("_fre_int",range(0,3)) = 1.51
		_FresRange("折射范围",range(0,3)) = 1
		[Space][Space][Space][Space][Space][Space][Space][Space]
		_colorNear("颜色近",Color) = (0,0.835294,0.7894523,1)
		_colorFar("颜色远",Color) = (0.2205884,0.1502028,1,1)
		_Alpha("透明度",range(0,1)) = 0.441
		_refSet("折射调节",range(0,5)) = 0.5
		_SpecColor("高光颜色",Color) = (0,0,0,1)
		_diffStep("漫反射层级",range(0,5)) = 2
		[Space][Space][Space][Space][Space][Space][Space][Space]
		_RefTex("反射RT",2D) = "black"{}
		_flcSet("反射调节",range(0,1)) = 0.25
		_RefDistort("反射扰动",range(0,3.5)) = 1.52

		[Space][Space][Space][Space][Space][Space][Space][Space]
		_FoamTex("水沫",2D) = "black"{}
		_FoamSpeedU("水沫速度1",float) = 0.1
		_FoamSpeedV("水沫速度2",float) = 0.1
		_FoamDistort("水沫速度扰动",range(0,2)) = 0.05
		_FoamLight1("水沫亮度1",range(0,1)) = 0.067
		_FoamLight2("水沫亮度2",range(1,15)) = 6.08
		_FoamColor("水沫颜色",Color) = (1,1,1,1)

	}
	SubShader
		{
		Tags {
			"IngoreProjector" = "True"
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			}
		GrabPass{}

		Pass
		{
			Name"FORWARD"
			Tags{"LightMode" = "ForwardBase"}
			ZWrite off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_FORWARDBASE
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma only_renderers d3d9 d3d11 glcore gles 
			#pragma	target 3.0
			#include "UnityCG.cginc"

			sampler2D _GrabTexture;
			float _Diff_Set;
			float _SpecSet;
			float _SpecRange;
			sampler2D _NormalMap;
			uniform float4 _NormalMap_ST;
			float _uv_scale_r;
			float _uv_scale_g;
			float _uv_scale_b;
			float _uv_Speed_r;
			float _uv_Speed_g;
			float _uv_Speed_b;
			float _normal_r;
			float _normal_g;
			float _normal_b;
			float4 _water_color;
			float _fre_int;
			float _FresRange;
			float4 _colorNear;
			float4 _colorFar;
			float _Alpha;
			float _refSet;
			float4 _SpecColor;
			float _diffStep;
			float _specStep;
			sampler2D _RefTex;
			uniform float4 _RefTex_ST;
			float _flcSet;
			half _RefDistort;
			sampler2D _FoamTex;
			float4 _FoamTex_ST;
			half _FoamSpeedU;
			half _FoamSpeedV;
			half _FoamDistort;
			half _FoamLight1;
			half _FoamLight2;
			fixed4 _FoamColor;

			struct a2v {
				float4 vertex:POSITION;
				float3 normal :NORMAL;
				float4 tangent:TANGENT;
				float2 texcoord0:TEXCOORD0;
			};
			struct v2f {
				float4 pos:SV_POSITION;
				float2 uv0:TEXCOORD0;
				float4 posWorld:TEXCOORD1;
				float3 normalDir:TEXCOORD2;
				float3 tangentDir:TEXCOORD3;
				float3 bitangentDir : TEXCOORD4;
				float4 screenPos : TEXCOORD5;
				float4 refl: TEXCOORD6;
				float2 uvmain:TEXCOORD7;
				float2 uvFoam:TEXCOORD8;
				UNITY_FOG_COORDS(6)
			};
			v2f vert(a2v v) {
				v2f o = (v2f)0;
				o.uv0 = v.texcoord0;
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
				o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.refl = ComputeScreenPos(o.pos);
				o.uvmain = TRANSFORM_TEX(v.texcoord0, _RefTex);
				o.uvFoam = v.texcoord0 * _FoamTex_ST.xy + float2(_Time.y * _FoamSpeedU, _Time.y * _FoamSpeedV);
				UNITY_TRANSFER_FOG(o, o.pos);
				o.screenPos = o.pos;
				return o;
			}
			float4 frag(v2f i) : COLOR{
				#if UNITY_UV_STARTS_AT_TOP
					float grabSign = -_ProjectionParams.x;
				#else
					float grabSign = _ProjectionParams.x;
				#endif
				i.normalDir = normalize(i.normalDir);
				i.screenPos = float4(i.screenPos.xy / i.screenPos.w, 0, 0);
				i.screenPos.y *= _ProjectionParams.x;
				float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);
				float3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

				float2 uv1 = ((i.uv0*_uv_scale_r) + (_Time*_uv_Speed_r)*float2(1,1));
				float3 tex1RGB = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(uv1, _NormalMap)));
				float2 uv2 = ((_uv_scale_g*i.uv0) + (_Time*_uv_Speed_g)*float2(1,-1));
				float3 tex2RGB = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(uv2, _NormalMap)));
				float2 uv3 = ((_uv_scale_b*i.uv0) + (_Time*_uv_Speed_b)*float2(-1,1));
				float3 tex3RGB = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(uv3, _NormalMap)));
				float3 normalLocal = (float3((tex1RGB.r*_normal_r),(tex1RGB.g*_normal_r),tex1RGB.b) + float3((tex2RGB.r*_normal_g),(tex2RGB.g*_normal_g),tex2RGB.b) + float3((_normal_b*tex3RGB.r),(_normal_b*tex3RGB.g),tex3RGB.b));

				float3 normalDir = normalize(mul(normalLocal, tangentTransform));
				float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5 + 0.5;
				float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float Fresnel = saturate(pow((dot(ViewDir,normalDir)*_fre_int),_FresRange));
				i.refl.xy += normalLocal * _RefDistort;
				float4 _RefTex_var = tex2Dproj(_RefTex, UNITY_PROJ_COORD(i.refl));

				half4 foamC = tex2D(_FoamTex,normalLocal.xy * _FoamDistort + i.uvFoam);
				half foam = pow((foamC.r + foamC.g + foamC.b) * 0.333 + _FoamLight1, _FoamLight2);
				float4 foamRgb = float4(foam * _FoamColor.rgb,1);

				float3 finalColor = ((lerp(tex2D(_GrabTexture, (0.05*(((_refSet*(saturate(dot(normalLocal,float3(0.3,0.59,0.11))) - 0.5)) + 0.5) - 0.5)*mul(tangentTransform, ViewDir).xy + sceneUVs.rg).rg).rgb,((_water_color.rgb*((_Diff_Set*(0.2 + floor(max(0,dot(ViewDir,normalDir)) * _diffStep) / (_diffStep - 1))) + (pow((_SpecSet*floor((max(0,dot(ViewDir,normalDir))*max(0,dot(normalDir,lightDir))) * _specStep) / (_specStep - 1)),_SpecRange)*_SpecColor.rgb))) + lerp(_colorFar.rgb,_colorNear.rgb,Fresnel)),_Alpha) + (_RefTex_var.rgb * _flcSet)) + foamRgb.rgb);
				fixed4 finalRGBA = fixed4(finalColor,1);
				UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
				return finalRGBA;
			}
            ENDCG
        }
    }
	Fallback"Diffuse"
}
