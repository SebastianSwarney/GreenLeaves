// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GreenLeaves_Custom/FoliageBaked"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_TintColor("Tint Color", Color) = (0.07801414,1,0,1)
		_BottomColor("Bottom Color", Color) = (1,0,0,1)
		_ColorTexture("Color Texture", 2D) = "white" {}
		_NormalTexture("Normal Texture", 2D) = "white" {}
		_PackedTexture("Packed Texture", 2D) = "white" {}
		_RoughnessIntensity("Roughness Intensity", Range( 0 , 5)) = 1
		_AOIntensity("AO Intensity", Range( 0 , 5)) = 1
		_Metallic("Metallic", Float) = 0
		_GradientPowerMul("Gradient Power Mul", Vector) = (1,1,0,0)
		_Amplitude("Amplitude", Float) = 1
		_Frequency("Frequency", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Grass"  "Queue" = "Transparent+0" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Frequency;
		uniform float _Amplitude;
		uniform sampler2D _NormalTexture;
		uniform float4 _TintColor;
		uniform float4 _BottomColor;
		uniform float2 _GradientPowerMul;
		uniform sampler2D _ColorTexture;
		uniform float _Metallic;
		uniform sampler2D _PackedTexture;
		uniform float4 _PackedTexture_ST;
		uniform float _RoughnessIntensity;
		uniform float _AOIntensity;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 appendResult37 = (float3(ase_vertex3Pos.x , ( ase_vertex3Pos.y + ( sin( ( ( ase_vertex3Pos.x * _Frequency ) + _Time.y ) ) * _Amplitude ) ) , ase_vertex3Pos.z));
			float3 lerpResult46 = lerp( ase_vertex3Pos , appendResult37 , v.texcoord.xy.y);
			v.vertex.xyz += lerpResult46;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = tex2D( _NormalTexture, i.uv_texcoord ).rgb;
			float4 lerpResult31 = lerp( _TintColor , _BottomColor , ( 1.0 - saturate( ( pow( i.uv_texcoord.y , _GradientPowerMul.x ) * _GradientPowerMul.y ) ) ));
			float4 tex2DNode1 = tex2D( _ColorTexture, i.uv_texcoord );
			o.Albedo = ( lerpResult31 * tex2DNode1 ).rgb;
			o.Metallic = _Metallic;
			float2 uv_PackedTexture = i.uv_texcoord * _PackedTexture_ST.xy + _PackedTexture_ST.zw;
			float4 tex2DNode60 = tex2D( _PackedTexture, uv_PackedTexture );
			o.Smoothness = ( 1.0 - ( tex2DNode60.r * _RoughnessIntensity ) );
			o.Occlusion = ( tex2DNode60.g * _AOIntensity );
			o.Alpha = 1;
			clip( tex2DNode60.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
-1929;78;1920;922;2629.268;311.78;1.740422;True;True
Node;AmplifyShaderEditor.PosVertexDataNode;36;-3287.674,1002.832;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;39;-2217.731,1469.372;Inherit;False;Property;_Frequency;Frequency;11;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;28;-3325.238,381.3626;Inherit;False;Property;_GradientPowerMul;Gradient Power Mul;9;0;Create;True;0;0;False;0;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-3433.169,70.13248;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;41;-1911.981,1660.569;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1951.731,1360.372;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-1696.981,1365.569;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;26;-2991.972,296.4919;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-2752.238,408.3625;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1396.981,1656.569;Inherit;False;Property;_Amplitude;Amplitude;10;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;42;-1432.981,1367.569;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1220.628,1371.955;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;30;-2579.638,423.3625;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;32;-2686.696,-249.6147;Inherit;False;Property;_BottomColor;Bottom Color;2;0;Create;True;0;0;False;0;1,0,0,1;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;33;-2425.625,417.4012;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;60;-1512.208,231.3869;Inherit;True;Property;_PackedTexture;Packed Texture;5;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-1072.156,295.9641;Inherit;False;Property;_RoughnessIntensity;Roughness Intensity;6;0;Create;True;0;0;False;0;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-981.1666,1265.721;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-2685.968,-510.25;Inherit;False;Property;_TintColor;Tint Color;1;0;Create;True;0;0;False;0;0.07801414,1,0,1;0.07801414,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-796.2393,194.4026;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;31;-2171.854,-249.87;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;37;-663.9612,1098.461;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;1;-1909.197,-123.8516;Inherit;True;Property;_ColorTexture;Color Texture;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-670.5992,1290.206;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;-1075.697,440.6333;Inherit;False;Property;_AOIntensity;AO Intensity;7;0;Create;True;0;0;False;0;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;48;-1919.108,770.6014;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;54;-3022.766,865.2661;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;50;-3301.234,751.066;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DistanceOpNode;51;-2782.97,750.8777;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-2751.995,924.7638;Inherit;False;Property;_DistanceExtra;Distance Extra;12;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-2491.995,810.7638;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-232.17,34.7898;Inherit;False;Property;_Metallic;Metallic;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;52;-2494.885,630.2742;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;25;-620.856,198.6636;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-790.9805,380.4724;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-1487.22,781.9015;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;46;-269.7467,1012.897;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-824.7093,-264.6702;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;11;-1482.139,-9.421439;Inherit;True;Property;_NormalTexture;Normal Texture;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;57;-2247.995,816.7638;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-25,-71.3;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;GreenLeaves_Custom/FoliageBaked;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Grass;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;38;0;36;1
WireConnection;38;1;39;0
WireConnection;40;0;38;0
WireConnection;40;1;41;0
WireConnection;26;0;16;2
WireConnection;26;1;28;1
WireConnection;29;0;26;0
WireConnection;29;1;28;2
WireConnection;42;0;40;0
WireConnection;43;0;42;0
WireConnection;43;1;44;0
WireConnection;30;0;29;0
WireConnection;33;0;30;0
WireConnection;45;0;36;2
WireConnection;45;1;43;0
WireConnection;61;0;60;1
WireConnection;61;1;24;0
WireConnection;31;0;3;0
WireConnection;31;1;32;0
WireConnection;31;2;33;0
WireConnection;37;0;36;1
WireConnection;37;1;45;0
WireConnection;37;2;36;3
WireConnection;1;1;16;0
WireConnection;48;1;52;0
WireConnection;48;0;57;0
WireConnection;54;0;36;0
WireConnection;51;0;50;0
WireConnection;51;1;54;0
WireConnection;56;0;51;0
WireConnection;56;1;55;0
WireConnection;25;0;61;0
WireConnection;62;0;60;2
WireConnection;62;1;63;0
WireConnection;49;0;1;4
WireConnection;49;1;48;0
WireConnection;46;0;36;0
WireConnection;46;1;37;0
WireConnection;46;2;47;2
WireConnection;4;0;31;0
WireConnection;4;1;1;0
WireConnection;11;1;16;0
WireConnection;57;0;56;0
WireConnection;0;0;4;0
WireConnection;0;1;11;0
WireConnection;0;3;23;0
WireConnection;0;4;25;0
WireConnection;0;5;62;0
WireConnection;0;10;60;4
WireConnection;0;11;46;0
ASEEND*/
//CHKSM=76C315F9B9FDBEDCAF9B151AF41025CD3F0A0F06