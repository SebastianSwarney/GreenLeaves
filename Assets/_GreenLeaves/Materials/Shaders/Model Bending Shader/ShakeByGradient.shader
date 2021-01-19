// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ShakeByGradient"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_LightCutoff("Light cutoff", Range(0,1)) = 0.5
		_ShadowBands("Shadow bands", Range(1,4)) = 1

		[Header(Specular)]
		_SpecularMap("Specular map", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		[HDR]_SpecularColor("Specular color", Color) = (0,0,0,1)

	
		[Header(Emission)]
		[HDR]_Emission("Emission", Color) = (0,0,0,1)

		[Header(Speed)]
		_SpeedX("SpeedX", Float) = .5
		_SpeedZ("SpeedZ", Float) = .5
		_Amplitude("Amplitude", Float) = .5
		_Distance("Distance", Range(0.0,50)) = .5
		_Amount("Amount", Float) = .5
		_MovementSample("Movement Sample", 2D) = "white" {}
		_XPos("X Pos", float) = 0.0
		_ZPos("Z Pos", float) = 0.0

		[Header(Wind)]
		_XWindDir("X Wind Dir" , Range(-0.5,0.5)) = 0.0
		_ZWindDir("Z Wind Dir", Range(-0.5,0.5)) = 0.0

		_XWindAmplitude("X Wind Amp", float) = 0.0
		_ZWindAmplitude("Z Wind Amp", float) = 0.0
	}
		SubShader
	{
		//Tags { "RenderType" = "Opaque" }
		Tags { "RenderType" = "Opaque"              "LightMode" = "ForwardBase"
			  "PassFlags" = "OnlyDirectional" }
		LOD 200


		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0


		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _Normal;
		sampler2D _MovementSample;
		float _LightCutoff;
		float _ShadowBands;


		sampler2D _SpecularMap;
		half _Glossiness;
		fixed4 _SpecularColor;


		fixed4 _Emission;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_Normal;
			float2 uv_SpecularMap;
		};

		struct SurfaceOutputCelShaded
		{
			fixed3 Albedo;
			fixed3 Normal;
			float Smoothness;
			half3 Emission;
			fixed Alpha;
		};




		struct v2f {
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
			float3 worldPos : TEXCOORD0;
		};

		float _SpeedX; 
		float _SpeedZ;
		
		float _XPos;
		float _ZPos;
		float _Amplitude;
		float _Distance;
		float _Amount;

		float _XWindDir;
		float _ZWindDir;
		
		float _XWindAmplitude;
		float _ZWindAmplitude;

		void vert(inout appdata_full v)
		{
			v2f o;

			float4 tex = tex2Dlod(_MovementSample, float4(v.texcoord.xy, 0, 0));

			o.worldPos = mul(unity_ObjectToWorld, v.vertex);
			o.vertex = UnityObjectToClipPos(v.vertex);


			v.vertex.x += tex.r * (sin((_Time.y * _SpeedX + v.vertex.y * _Amplitude) - _XPos) * _Distance * _Amount) +(pow(tex.r * _XWindAmplitude,2) ) * _XWindDir;
			v.vertex.z += tex.r * (cos((_Time.y * _SpeedZ + v.vertex.y * _Amplitude) - _ZPos) * _Distance * _Amount) + (pow(tex.r * _ZWindAmplitude,2)) * _ZWindDir;
		}


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		ENDCG

	}
		FallBack "Diffuse"
}