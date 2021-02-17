﻿Shader "GreenLeaves_Custom/TreeLeaf_PineShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _LightCutoff("Light cutoff", Range(0,1)) = 0.5
        _TextureCutoff("Texture Cutoff", Range(0,1)) = 0.5
 
        [Header(Rim)]
        _RimSize("Rim size", Range(0,1)) = 0
        [HDR]_RimColor("Rim color", Color) = (0,0,0,1)
        [Toggle(SHADOWED_RIM)]
        _ShadowedRim("Rim affected by shadow", float) = 0
         
        [Header(Emission)]
        [HDR]_Emission("Emission", Color) = (0,0,0,1)
 
        [Header(Displacement)]
        _DisplacementGuide("Displacement guide", 2D) = "white" {}
        _DisplacementAmount("Displacement amount", float) = 0
        _DisplacementSpeed("Displacement speed", float) = 0

 
        [Header(SSS)]
		[HDR]_SSSColor("SSS color", Color) = (1,1,1,1)
		_SSSConcentration("SSS Area Concentration", float) = 0

		[Header(Pine Adjustment)]
		_PineOffsetAmount("Pine Offset", float) = 0.67
		_PineOffsetSpeed("Pine Offset Speed", float) = 1
		_PineOffsetGuide("Pine Offset Guide", 2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off // cull off to disable backface culling
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Tree fullforwardshadows addshadow  vertex:vert//addshadow allows the leaves that are clippsed to cast a matching shadow
        #pragma shader_feature SHADOWED_RIM 
        #pragma target 3.0
 
 
        fixed4 _Color;
        sampler2D _MainTex;
        float _LightCutoff;
        float _TextureCutoff;
 
        float _RimSize;
        fixed4 _RimColor;
 
        fixed4 _Emission;
 
        float _SSSConcentration;
        fixed4 _SSSColor;       
 
        sampler2D _DisplacementGuide;
        float _DisplacementAmount;
        float _DisplacementSpeed;
 

		///Pine Offset
		float _PineOffsetAmount;
		float _PineOffsetSpeed;
		sampler2D _PineOffsetGuide;


        struct Input
        {
            float2 uv_MainTex; //main texture for the leaf cards
            float2 uv_DisplacementGuide; //displacement texture *A large scale noise is probably better for displacing leaves to simulate wind
        };
 
        struct SurfaceOutputTree
        {
            fixed3 Albedo;
            fixed3 Normal;
            float Smoothness;
            half3 Emission;
            float4 SSS;
            fixed Alpha;
        };
 



		//Appdata_full is a built-in struct and is our input
		
		void vert(inout appdata_full v) { 

			float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
			float4 vert = UnityObjectToClipPos(v.vertex);
			float offset = (v.vertex.y + (v.vertex.z * 0.2)) * 0.5;

			v.vertex.y += _PineOffsetAmount * tex2Dlod(_PineOffsetGuide, v.texcoord).r * sin((_Time.y + offset) * _PineOffsetSpeed); //* (sin(_Time.y * _PineOffsetSpeed + v.vertex.y) - worldPos.z);
		}

        half4 LightingTree (SurfaceOutputTree s, half3 lightDir, half3 viewDir, half atten) {
            half nDotL = saturate(dot(s.Normal, normalize(lightDir))); // get preliminary light intensity through dot product
            half diff = step(_LightCutoff, nDotL); // use _LightCutoff as a threshld for where the shadows are
 
            // emulate fake Subsurface Scattering 
            half sssAmount = pow(saturate(dot(normalize(viewDir), -normalize(lightDir))), _SSSConcentration);
            fixed4 sssColor = sssAmount * s.SSS;
 
            float rimArea = step(1 - _RimSize ,1 - saturate(dot(normalize(viewDir), s.Normal)));
            float3 rim = _RimColor * rimArea;
 
            half stepAtten = round(atten);
            half shadow = diff * stepAtten;
             
            half3 col = s.Albedo * _LightColor0;
 
            half4 c;
            #ifdef SHADOWED_RIM
            c.rgb = (col + rim) * shadow;
            #else
            c.rgb = col * shadow + rim;
            #endif        
            c.rgb += sssColor.rgb * stepAtten * diff; //limit SSS in areas where it is somewhat lit
            c.a = s.Alpha;
            return c;
        }
 
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
 
        void surf (Input IN, inout SurfaceOutputTree o)
        {
			//sample the displacement guide and apply the speed to pan the UVs, map the results from -1 to 1 and multiply it by the _DisplacementAmount to scale
            float2 displ = (tex2D(_DisplacementGuide, IN.uv_DisplacementGuide + _Time.y * _DisplacementSpeed).xy * 2.0 - 1.0) * _DisplacementAmount;
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex + displ);
            fixed4 c = _Color * tex;
 
            o.Albedo = c.rgb;
            clip(tex.a - _TextureCutoff); // clips or discards the pixels where the alpha value is less than 0 after subtracting by _TextureCutoff value
            o.Emission = o.Albedo * _Emission;
            o.SSS = _SSSColor;
            o.Alpha = c.a;
			
        }
        ENDCG
    }
    FallBack "Diffuse"
}