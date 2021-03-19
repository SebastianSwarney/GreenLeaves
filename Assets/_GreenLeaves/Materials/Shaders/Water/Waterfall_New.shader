Shader "Custom/WaterIntersection-Waterfall"
{
	Properties
	{
	   _Color("Main Bright Color", Color) = (1, 1, 1, .5)
	   _DarkColor("Main Dark Color", Color) = (1, 1, 1, .5)
	   _IntersectionColor("Intersection Color", Color) = (1, 1, 1, .5)
	   _IntersectionThresholdMax("Intersection Threshold Max", float) = 1
	   _DisplGuide("Displacement guide", 2D) = "white" {}
	   _DisplAmount("Displacement amount", float) = 0
		_borderColorCutoff("Border Color Cutoff", float) = 0.5
		   [Space]
	   [Header(Water Details)]
	   _LeveledWaterTex("Leveled Water Texture", 2D) = "white"
	   _leveledWaterSpeed("Leveled Water Speed", float) = 1
		   _leveledWaterTexScale("Leveled Water Texture Scale", float) = 1


		   [Space]
	   [Header(Sine Bounce)]
	   _WaterBounceFrequency("Water Bounce Frequency", float) = 1
		   _WaterBounceHeight("Water Bounce Height", float) = 1

		   _debug("DEbug", float) = 1
		[Toggle(VERTEX)] _VERTEX("Use Vertex Colors", Float) = 0

		   _lightingBrightness("LightingBrightness", float) = 1

	}
		SubShader
	   {
		   Tags {"Queue" = "Transparent" "RenderType" = "Transparent"  }

		   Pass
		   {
			   Tags{"LightMode" = "ForwardBase"}
			  Blend SrcAlpha OneMinusSrcAlpha
			  ZWrite Off

			  CGPROGRAM
			  #pragma vertex vert
			  #pragma fragment frag
			  #pragma multi_compile_fog
#pragma target 3.5
			  #include "UnityCG.cginc"
				#include "AutoLight.cginc"


			  struct appdata
			  {

				  float4 vertex : POSITION;
				  float2 uv : TEXCOORD0;
				  float4 color :COLOR;
				  float3 normal:NORMAL;
			  };

			  struct v2f
			  {

				  float3 worldPos:TEXCOORD0;
				  float2 uv : TEXCOORD4;
				  UNITY_FOG_COORDS(1)
				  float4 vertex : SV_POSITION;
				  float2 displUV : TEXCOORD2;
				  float4 scrPos : TEXCOORD3;
				  fixed4 color : COLOR;
				  float3 wNormal: TEXCOORD5;
				  float3 normal: NORMAL;

				  float3 lightDir:TEXCOORD6;

				  LIGHTING_COORDS(7,8)
			  };

			  sampler2D _CameraDepthTexture;
			  float4 _Color;
			  float4 _DarkColor;
			  float4 _IntersectionColor;
			  float _IntersectionThresholdMax;
			  sampler2D _DisplGuide;
			  float4 _DisplGuide_ST;
			  float _VERTEX;





			  //Water Details
			  sampler2D _LeveledWaterTex;
			  float _leveledWaterSpeed;
			  float _leveledWaterTexScale;
			  float _waterDetailBrightness;
			  float _borderColorCutoff;



			  //Water bounce variables
			  float _WaterBounceFrequency;
				  float _WaterBounceHeight;
				  float _debug;

				  float _lightingBrightness;

			  v2f vert(appdata v)
			  {
				  v2f o;
				  o.vertex = UnityObjectToClipPos(v.vertex);
				  o.scrPos = ComputeScreenPos(o.vertex);
				  o.displUV = TRANSFORM_TEX(v.uv, _DisplGuide);
				  o.uv = v.uv;
				  o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				  UNITY_TRANSFER_FOG(o,o.vertex);
				  o.wNormal = UnityObjectToWorldNormal(v.normal);
				  o.normal = normalize(v.normal);

				  o.color = v.color;

				  o.lightDir = normalize(ObjSpaceLightDir(v.vertex));

				  float offset = (o.worldPos.x + (o.worldPos.z * 0.2)) * 0.5;
				  o.vertex.y += sin(_Time.y * _WaterBounceFrequency * offset * .2) * _WaterBounceHeight;

				  TRANSFER_VERTEX_TO_FRAGMENT(o);
				  return o;
			  }

			  half _DisplAmount;

			  half4 frag(v2f i) : SV_TARGET
			  {




					   float timing = frac(_Time.y * 0.5 + 0.5);
					   float timing2 = frac(_Time.y * 0.5);
					   float timingLerp = abs((0.5 - timing) / 0.5);

					   float3 flowDir = (i.color * 2.0f) - 1.0f;

					   flowDir *= _leveledWaterSpeed;

					   

					   float2 displ = tex2D(_DisplGuide, i.displUV + flowDir.xz * _Time.y).xy;
					   displ = ((displ * 2) - 1) * _DisplAmount;

					   float diff = (saturate(_IntersectionThresholdMax * (100 - i.scrPos.w) + displ));










					   ///Normal Flat water
					   fixed4 topTex = tex2D(_LeveledWaterTex, (i.worldPos.xy* _leveledWaterTexScale) + flowDir.xz * timing * _leveledWaterSpeed);
					   fixed4 topTex2 = tex2D(_LeveledWaterTex, (i.worldPos.xy * _leveledWaterTexScale) + flowDir.xz * timing2 * _leveledWaterSpeed);

					   fixed4 col = lerp(topTex, topTex2, timingLerp);


					   /*float4 paintColor = _Color;
					   if (1 - normAngle > _waterfallAngle) {
						   paintColor = (0, 255, 255, 0);
					   }*/

					   if (diff < _borderColorCutoff) {
						   col = lerp(_IntersectionColor, _Color, step(0.5, diff));
					   }
					   else {
						   col = lerp(_Color, _DarkColor, col.r);
					   }


						   if (_VERTEX > 0) {

							   col = i.color;
						   }


					float3 lightDir = normalize(i.lightDir);
					float3 lightNorm = normalize(i.normal);
					float attenuation = LIGHT_ATTENUATION(i) / _lightingBrightness;
					float4 ambient = UNITY_LIGHTMODEL_AMBIENT;
					float NormDotDir = saturate(dot(lightNorm, lightDir));

					float4 diffuseTerm = NormDotDir * (attenuation);
					float4 finalColor = (ambient + diffuseTerm) * col;

						UNITY_APPLY_FOG(i.fogCoord, col);
						return finalColor;
			   }

			   ENDCG
		   }
	   }
		   FallBack "Diffuse"
}