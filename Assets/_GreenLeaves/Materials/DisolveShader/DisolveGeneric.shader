Shader "Custom/DissolveGeneric" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_ScrollXSpeed("ScrollXSpeed", float) = 0.0
		_ScrollYSpeed("ScrollYSpeed", float) = 0.0
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0

		_SecondarySliceGuide("Secondary Slice Guide (RGB)", 2D) = "white" {}
		_SecondaryScrollXSpeed("SecondaryScrollXSpeed", float) = 0.0
		_SecondaryScrollYSpeed("SecondaryScrollYSpeed", float) = 0.0
		_SecondarySliceAmount("SecondarySliceAmount", Range(0.0,1.0)) = 0
			

		_BurnSize("Burn Size", Range(0.0, 1.0)) = 0.15
		_BurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
		_BurnColor("Burn Color", Color) = (1,1,1,1)

		_EmissionAmount("Emission amount", float) = 2.0

		//[PerRendererData]
		_EffectAmount("Effect Amount", Range(0.0,1.0)) = 0.0


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
			sampler2D _SecondarySliceGuide;
			sampler2D _BumpMap;
			sampler2D _BurnRamp;
			fixed4 _BurnColor;
			float _BurnSize;
			float _SliceAmount;
			float _EmissionAmount;

			float _EffectAmount;
			float _ScrollXSpeed;
			float _ScrollYSpeed;			
			float _SecondaryScrollXSpeed;
			float _SecondaryScrollYSpeed;
			float _SecondarySliceAmount;

			
			

			struct Input {
				float2 uv_MainTex;
				float3 worldPos;
			};


			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				
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

				fixed2 secondarySliceUV = IN.uv_MainTex;
				xScroll = _SecondaryScrollXSpeed * _Time;
				yScroll = _SecondaryScrollYSpeed * _Time;
				secondarySliceUV += fixed2(xScroll, yScroll);
				
				half secondarySlice = tex2D(_SecondarySliceGuide, secondarySliceUV).rgb - _SecondarySliceAmount *_EffectAmount;
				clip(secondarySlice);
				//half test = tex2D(_SliceGuide, IN.uv_MainTex).rgb - amount;
				half test = tex2D(_SliceGuide, scrolledBurnUV).rgb  - _EffectAmount;
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