Shader "GreenLeaves_Custom/CelShade"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_LightCutoff("Light cutoff", Range(0,1)) = 0.5
		_ShadowBands("Shadow bands", Range(1,4)) = 1

		[Header(Specular)]
		_SpecularMap("Specular map", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		[HDR]_SpecularColor("Specular color", Color) = (0,0,0,1)

		[Header(Rim)]
		_RimSize("Rim size", Range(0,1)) = 0
		[HDR]_RimColor("Rim color", Color) = (0,0,0,1)
		[Toggle(SHADOWED_RIM)]
		_ShadowedRim("Rim affected by shadow", float) = 0

		[Header(Emission)]
		[HDR]_Emission("Emission", Color) = (0,0,0,1)

			[Header(Cutout)]
		_CutoutMap("CutoutMap", 2D) = "white"{}
			_CutoutAlpha("CutoutAmount", Range(0,1)) = 1
	}
	SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf CelShaded fullforwardshadows addshadow
			#pragma shader_feature SHADOWED_RIM
			#pragma target 3.0

			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _Normal;
			float _LightCutoff;
			float _ShadowBands;

			sampler2D _SpecularMap;
			half _Glossiness;
			fixed4 _SpecularColor;

			float _RimSize;
			fixed4 _RimColor;

			fixed4 _Emission;


			///Cutout
			sampler2D _CutoutMap;
			float _CutoutAlpha;
			struct Input {
				float2 uv_MainTex;
				float2 uv_Normal;
				float2 uv_SpecularMap;
			};

			struct SurfaceOutputCelShaded {
				fixed3 Albedo;
				fixed3 Normal;
				float Smoothness;
				half3 Emission;
				fixed Alpha;
			};

			//custom lighting model
			half4 LightingCelShaded(SurfaceOutputCelShaded s, half3 lightDir, half3 viewDir, half atten) {
				//calculate lighting based on surface normal and ligh direction
				half nDotL = saturate(dot(s.Normal, normalize(lightDir)));
				//adjust area of the lit surface with _LightCutoff by dividing the light intensity
				// then multiply the result by the number of shadow bands we want, round the result and divide it by the number of shadow bands
				// * multiplying NdotL with # of shadow bands gives a spectrum between 0-4. after rounding and dividing, diff is 0, .25, .5, .75, or 1
				half diff = round(saturate(nDotL / _LightCutoff) * _ShadowBands) / _ShadowBands;

				//calculate the specular highlights of the material
				float3 refl = reflect(normalize(lightDir), s.Normal); // find reflected ray based on light dir and surface normal
				float vDotRefl = dot(viewDir, -refl); //dot of viewDir and inverted reflection vector
				float3 specular = _SpecularColor.rgb * step(1 - s.Smoothness, vDotRefl); //????

				//calculate rim color: get inverted fresnel value, and get inverted value of _TimSize as a threshold to multiply into _RimColor to use for later
				float3 rim = _RimColor * step(1 - _RimSize, 1 - saturate(dot(normalize(viewDir), s.Normal)));

				//round attenuation value so its either 0 or 1, 
				// then make a shadow field which is made up by multiplying the rounded attenuation 
				// with the shading value calculated previously (line 71)
				half stepAtten = round(atten);
				half shadow = diff * stepAtten;

				//calculate main color by adding the specular color to the albedo and multiplying the whole thing by _LightColor0
				//multiplyies the color of the object with the color of the light affecting the object; 
				//there are no limites on the lights affecting the object on the shader, just the pixel lights limit of the project
				half3 col = (s.Albedo + specular) * _LightColor0;

				//assign final color to the RGB channels of c, the rim is calculated differently depending on if Shadowed_Rim is 1 or 0
				half4 c;
				#ifdef SHADOWED_RIM
				c.rgb = (col + rim) * shadow; //no rim light on shadowed areas
				#else
				c.rgb = col * shadow + rim; //rim light on top of shadowed areas
				#endif
				c.a = s.Alpha;
				return c;
			}

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			//In surface shaders, declaring coordinates as uv_ followed 
			//by the texture name automatically applies the tiling and offset 
			//on those coordinates as set by the material inspector. No need to declare _MainTex_ST

			void surf (Input IN, inout SurfaceOutputCelShaded o) //the struct type should match the one used in the lighting model - i.e. SurfaceOutputCelShaded instead of SurfaceOutputStandard in this case
			{
				clip(tex2D(_CutoutMap, IN.uv_MainTex).r - _CutoutAlpha);

				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color; // get texture colors and multiply by main color
				o.Albedo = c.rgb; // assign color from above to albedo/diffuse
				o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Normal)); // sample normal map and unpack its values to assign them to the normal field
				o.Smoothness = tex2D(_SpecularMap, IN.uv_SpecularMap).x * _Glossiness; // get red channel of specular map of specular map, and multiply by _Glossiness to get overall amount of specularity
				o.Emission = o.Albedo * _Emission; //multiply albedo color with _Emission and assign it to the corresponding field
				o.Alpha = c.a; // assign alpha value of the calculated color to the alpha field
			}
			ENDCG
    }
	FallBack "Diffuse"
}
