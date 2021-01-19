Shader "Custom/ShineShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Tex (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		[Space]
		[Header(Shine)]
		_ShineMap("Shine Map", 2D) = "white" {}
		_ShinePoint("Shine Point", Float) = 0
		_ShineRange("Shine Range", Float) = 0
		_ShineMinRange("Shine Min Range", Float) = 0
		_ShineIncrease("ShineIncreseAmount", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;


		sampler2D _ShineMap;
		float _ShineRange;
		float _ShinePoint;
		float _ShineMinRange;
		float _ShineIncrease;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            o.Alpha = c.a;
			float4 shine = tex2D(_ShineMap, IN.uv_MainTex) * _Color;




			if (shine.r <  _ShinePoint + _ShineRange && shine.r > _ShinePoint - _ShineRange) {
				
				float newPoint = abs(_ShinePoint - shine.r);
				if (newPoint < _ShineMinRange) {
					c.rgb += _ShineIncrease;
					//c.rgb = 0;
				}
				else {
					c.rgb += (1 - (newPoint - _ShineMinRange) / (_ShineRange - _ShineMinRange)) * _ShineIncrease;
				}

			}

			
			o.Albedo = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

