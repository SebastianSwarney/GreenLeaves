Shader "GreenLeaves_Custom/EdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Threshold("Threshold", float) = 0.01
		_EdgeColor("Edge color", Color) = (0,0,0,1)
    }
    SubShader
    {
		// No Culling or Depth
		Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _CameraDepthNormalsTexture;

			v2f vert(appdata v) 
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _Threshold;
            fixed4 _EdgeColor;
			
			float4 GetPixelValue(in float2 uv) {
				half3 normal;
				float depth;
				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv), depth, normal);
				return fixed4(normal, depth);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//get color of object based on uv
				fixed4 col = tex2D(_MainTex, i.uv);
				//get normal depth value of the current pixel
				fixed4 orValue = GetPixelValue(i.uv);
				//values used to calculate the position of each of the 8 surrounding pixels
				float2 offset[8] = {
					float2(-1,-1),
					float2(-1,0),
					float2(-1,1),
					float2(0,-1),
					float2(0,1),
					float2(1,-1),
					float2(1,0),
					float2(1,1),
				};
				//get normal dpeth value for each surrounding pixel by offsetting the uv coordinates
                fixed4 sampledValue = fixed4(0,0,0,0);
				//get all normal depth values from the 8 surrounding pixels and add up
				for (int j = 0; j < 8; j++) {
					sampledValue += GetPixelValue(i.uv + offset[j] * _MainTex_TexelSize.xy);
				}
				//average the pixedl values to see the deviation between all 8 pixels
				sampledValue /= 8;

				//step compares the threshold value with the difference of the original and average value
				// and returns 1 if _Threshold is larger, while 0 if _Threshold is smaller
                return lerp(col, _EdgeColor, step(_Threshold, length(orValue - sampledValue)));
            }
            ENDCG
        }
    }
}
