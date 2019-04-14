//https://docs.unity3d.com/Manual/SL-SurfaceShaderExamples.html

Shader "Toon/ToonShader" {
	Properties{
		_Albedo("Color", Color) = (0.0, 0.0, 0.0, 0.0)
		_MainTex("Texture", 2D) = "white" {}
		_Ramp("Ramp", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_RimLight("Rim Light", Color) = (0.0, 0.0, 0.0, 0.0)
		_RimStrength("Rim Strength", Range(0.0, 20.0)) = 5.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			CGPROGRAM

			#pragma surface surf Ramp addshadow fullforwardshadows

			#pragma target 3.0

			sampler2D _Ramp;

			half4 LightingRamp(SurfaceOutput surface, half3 directionOfLight, half attenuation) {
				half NdotL = dot(surface.Normal, directionOfLight);
				half correctedCoord = NdotL * 0.5 + 0.5;
				half3 ToonRamp = tex2D(_Ramp, float2(correctedCoord, correctedCoord)).rgb;
				half4 ToonShade;
				ToonShade.rgb = surface.Albedo * _LightColor0.rgb * ToonRamp * attenuation;
				ToonShade.a = surface.Alpha;
				return ToonShade;
			}

			struct Input {
				float2 uv_MainTex;
				float2 uv_NormalMap;
				float3 viewDir;
			};

			float4 _Albedo;
			sampler2D _MainTex;
			sampler2D _NormalMap;
			float4 _RimLight;
			float _RimStrength;

			void surf(Input IN, inout SurfaceOutput o) {
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Albedo;
				o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
				half rLight = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
				o.Emission = _RimLight.rgb * pow(rLight, _RimStrength);
			}
			ENDCG
		}
			Fallback "Vertex Lit"
}
