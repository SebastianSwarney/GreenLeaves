Shader "Custom/DissolveTest" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0

		_BurnSize("Burn Size", Range(0.0, 1.0)) = 0.15
		_BurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
		_BurnColor("Burn Color", Color) = (1,1,1,1)

		_EmissionAmount("Emission amount", float) = 2.0

		[Header(EmissionRange)]
		///Need to change this to match the object's y position
		_YPosition("YPosition", Float) = 0.0
		
		//Needs to match the current height of the object. Will require bounding box
		_ItemHeight("Height", float) = 0.0

		//Changes how much is currently effected
		[PerRendererData]_EffectAmount("Effect amount", Range(0.0, 1.0)) = 1

		
		_ScrollXSpeed ("ScrollXSpeed", float) = 0.0
		_ScrollYSpeed("ScrollYSpeed", float) = 0.0

	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200
			Cull Off
			CGPROGRAM
			#pragma surface surf Lambert addshadow
			#pragma target 3.0

			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _SliceGuide;
			sampler2D _BumpMap;
			sampler2D _BurnRamp;
			fixed4 _BurnColor;
			float _BurnSize;
			float _SliceAmount;
			float _EmissionAmount;

			float _YPosition;
			float _EffectAmount;
			float _ItemHeight;
			float _ScrollXSpeed;
			float _ScrollYSpeed;
			fixed4 _RimColor;

			
			

			struct Input {
				float2 uv_MainTex;
				float3 worldPos;
			};


			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				
				float amount = 1 - ((IN.worldPos.y - (_YPosition - _ItemHeight/2)) / (_ItemHeight)) + ((_EffectAmount-.5f)*2);

					//1 - ((IN.worldPos.y - _YPosition)/_ItemHeight - _MinRange) / (_MaxRange - _MinRange);
				//IN.worldPos.y - _YPosition;
				//_EffectAmount +tex2D(_DisolveMap, IN.uv_MainTex).r;
				// 1 - (IN.worldPos.y - _YPosition - _MinRange) / (_MaxRange - _MinRange);
				//amount = 1 - (IN.worldPos.y);
				//float newAmount = _EffectAmount;//1 - (IN.worldPos.y - _YPosition);

				//half test = tex2D(_SliceGuide, IN.uv_MainTex).rgb -amount;
				fixed2 scrolledBurnUV = IN.uv_MainTex;
				
				fixed xScroll = _ScrollXSpeed * _Time;
				fixed yScroll = _ScrollYSpeed * _Time;
				scrolledBurnUV += fixed2(xScroll, yScroll);

				

				//half test = tex2D(_SliceGuide, IN.uv_MainTex).rgb - amount;
				half test = tex2D(_SliceGuide, scrolledBurnUV).rgb - amount;
				//test = amount;
				clip(test);

				if (test < _BurnSize && _SliceAmount > 0) {
					o.Emission = tex2D(_BurnRamp, float2(test * (1 / _BurnSize), 0)) * _BurnColor * _EmissionAmount;
				}
				
				o.Albedo = c.rgb;
				
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}