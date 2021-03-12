// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GreenLeaves_Custom/Foliage_SimpleAlpha"
{
	Properties
	{
		_ColorTint("Color Tint", Color) = (1,1,1,0)
		[HDR]_EmissionColor("Emission Color", Color) = (0.509434,0.509434,0.509434,0)
		_Texture("Texture", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.75
		_PosterizePower("Posterize Power", Range( 0 , 100)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Grass"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _ColorTint;
		uniform float _PosterizePower;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform float4 _EmissionColor;
		uniform float _Cutoff = 0.75;


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1);
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1);
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			Gradient gradient3 = NewGradient( 0, 5, 2, float4( 0.7075472, 0.5916064, 0.3037113, 0 ), float4( 0.4433962, 0.1694108, 0.1694108, 0.1470665 ), float4( 0.3230257, 0.5092606, 0.4645802, 0.3499962 ), float4( 0.4628067, 0.745283, 0.4605287, 0.5411765 ), float4( 0.5999911, 0.9150943, 0.5999911, 0.9941252 ), 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float4 tex2DNode1 = tex2D( _Texture, uv_Texture );
			float div16=256.0/float((int)_PosterizePower);
			float4 posterize16 = ( floor( tex2DNode1 * div16 ) / div16 );
			float4 temp_output_11_0 = ( _ColorTint * SampleGradient( gradient3, posterize16.r ) );
			o.Albedo = temp_output_11_0.rgb;
			o.Emission = _EmissionColor.rgb;
			o.Metallic = 0.0;
			o.Smoothness = ( 1.0 - 1.0 );
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
1920;62;1920;898;2089.932;629.031;1.4691;False;True
Node;AmplifyShaderEditor.SamplerNode;1;-1837.731,78.29225;Inherit;True;Property;_Texture;Texture;2;0;Create;True;0;0;False;0;-1;a779e30184627fe47bd3cf063393c970;e3fa015ba350b284cb579ba104567052;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-1323.258,84.22815;Inherit;False;Property;_PosterizePower;Posterize Power;4;0;Create;True;0;0;False;0;1;33.9;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;3;-912.9005,-253.5392;Inherit;False;0;5;2;0.7075472,0.5916064,0.3037113,0;0.4433962,0.1694108,0.1694108,0.1470665;0.3230257,0.5092606,0.4645802,0.3499962;0.4628067,0.745283,0.4605287,0.5411765;0.5999911,0.9150943,0.5999911,0.9941252;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.PosterizeNode;16;-1030.258,-105.7719;Inherit;True;1;2;1;COLOR;0,0,0,0;False;0;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-436.1853,147.8071;Inherit;False;Constant;_Roughness;Roughness;3;0;Create;True;0;0;False;0;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;2;-675.057,-252.2032;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;12;-588.057,-467.0544;Inherit;False;Property;_ColorTint;Color Tint;0;0;Create;True;0;0;False;0;1,1,1,0;0.5167764,0.6920267,0.745283,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;-628.5072,-23.88046;Inherit;False;Property;_EmissionColor;Emission Color;1;1;[HDR];Create;True;0;0;False;0;0.509434,0.509434,0.509434,0;0.0360376,0.0360376,0.0360376,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-317.1853,60.80713;Inherit;False;Constant;_Metallic;Metallic;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;10;-145.1853,151.8071;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-228.0903,-385.5292;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RoundOpNode;13;201.7424,-499.7719;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CeilOpNode;14;-555.2576,-727.7719;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;176.6,-61.49999;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;GreenLeaves_Custom/Foliage_SimpleAlpha;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.75;True;True;0;True;Grass;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;3;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;1;1;0
WireConnection;16;0;17;0
WireConnection;2;0;3;0
WireConnection;2;1;16;0
WireConnection;10;0;9;0
WireConnection;11;0;12;0
WireConnection;11;1;2;0
WireConnection;13;0;11;0
WireConnection;0;0;11;0
WireConnection;0;2;4;0
WireConnection;0;3;8;0
WireConnection;0;4;10;0
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=C444E089D45774787857231F6B224C0ACF4E5C50