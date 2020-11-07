Shader "GreenLeaves_Custom/DirectionalDissolve" {
    Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DissolveTex ("Dissolve Texture", 2D) = "white" {}
		_DissolveAmount("Dissolve amount", Range(-3,3)) = 0
		_Direction("Direction", vector) = (0,1,0,0)
		[HDR]_Emission("Emission", Color) = (1,1,1,1)
		_EmissionThreshold("Emission threshold", float) = 0.1
		_NoiseSize("Noise size", float) = 1
		
		_BurnSize("Burn size", Range(0.0, 1.0)) = 0.15
		_BurnRamp("Burn Ramp(RGB)", 2D) = "white" {}
		_BurnColor("BurnColor", color) = (1,1,1,1)
	}
	SubShader {
		//Batching is disabled because batches cause a bunch of meshes to gether and the object 
		//space coordinates all get unified and they stop referring to each instance's 
		//actual object coordinates
		
		Tags { "RenderType" = "Opaque" "DisableBatching" = "True"}
		LOD 200
		Cull off

		CGPROGRAM
		#pragma surface surf Lambert addshadow vertex:vert

		// make fog work
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			//passes the adjusted vertex coordinates of the object from 
			//the vertex shader onto the surface shader
			float3 worldPosAdj;
		};

		fixed4 _Color;
		sampler2D _DissolveTex;
		float _DissolveAmount;
		half4 _Direction;
		fixed4 _Emission;
		float _EmissionThreshold;
		float _NoiseSize;

        void vert (inout appdata_full v, out Input o)
        {
			//initializes the input object
			UNITY_INITIALIZE_OUTPUT(Input, o);
			//converts the vertex coordinates of the object in world space 
			//and saves it in the worldPosAdj field
			o.worldPosAdj = mul(unity_ObjectToWorld, v.vertex.xyz);
        }

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		//random function is used for creating a pixelated emissive noise.
		float random(float2 input) {
			return frac(sin(dot(input, float2(12.9898, 78.233))) * 43758.5453123);
		}


        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            // Clipping
			half test = (dot(IN.worldPosAdj, normalize(_Direction)) + 1) / 2;
			//*---- Test with textures----------- *//
			half test2 = test * tex2D(_DissolveTex, IN.uv_MainTex).rgb - _DissolveAmount;
			// ------------

			clip(test2 - _DissolveAmount);
			//Emission noise
			float squares = step(0.5, random(floor(IN.uv_MainTex * _NoiseSize) * _DissolveAmount));
			
			
			
			//half emissionRing = step(test - _EmissionThreshold, _DissolveAmount) * squares;
			half emissionRing = step(test - _EmissionThreshold, _DissolveAmount) * test2;

			o.Albedo = c.rgb;
			o.Emission = _Emission * emissionRing;// * tex2D(_BurnRamp, float2(test2 * 1/_BurnSize), 0));
			o.Alpha = c.a;
        }
        ENDCG
    }
	FallBack "Diffuse"
}
