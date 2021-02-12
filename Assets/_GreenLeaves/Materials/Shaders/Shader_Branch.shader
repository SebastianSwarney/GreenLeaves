// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GreenLeaves_Custom/Shader_Branch"
{
	Properties
	{
		_AlbedoTint("Albedo Tint", Color) = (0.4150943,0.17711,0.03720184,1)
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Opacity("Opacity", 2D) = "white" {}
		_Packed("Packed", 2D) = "white" {}
		_RoughnessMulti("Roughness Multi", Range( 0 , 3)) = 1
		_AOMult("AO Mult", Range( 0 , 3)) = 1
		_Normal("Normal", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Range( 0 , 10)) = 1
		[HDR]_Emission("Emission", Color) = (3.031433,3.031433,3.031433,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 4.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NormalIntensity;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _AlbedoTint;
		uniform float4 _Emission;
		uniform sampler2D _Packed;
		uniform float4 _Packed_ST;
		uniform float _RoughnessMulti;
		uniform float _AOMult;
		uniform sampler2D _Opacity;
		uniform float4 _Opacity_ST;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalIntensity );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			float4 temp_output_16_0 = ( tex2DNode1 * _AlbedoTint );
			o.Albedo = temp_output_16_0.rgb;
			o.Emission = ( _Emission * temp_output_16_0 ).rgb;
			float2 uv_Packed = i.uv_texcoord * _Packed_ST.xy + _Packed_ST.zw;
			float4 tex2DNode3 = tex2D( _Packed, uv_Packed );
			o.Metallic = tex2DNode3.r;
			o.Smoothness = ( 1.0 - ( tex2DNode3.a * _RoughnessMulti ) );
			o.Occlusion = ( 1.0 - ( tex2DNode3.g * _AOMult ) );
			o.Alpha = 1;
			float2 uv_Opacity = i.uv_texcoord * _Opacity_ST.xy + _Opacity_ST.zw;
			clip( tex2D( _Opacity, uv_Opacity ).r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
-1764;86;1920;934;1199.14;659.275;1.124546;True;True
Node;AmplifyShaderEditor.CommentaryNode;19;-1854.928,920.2683;Inherit;False;370;280;Comment;1;3;Packed;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;11;-1265.712,969.7249;Inherit;False;523.3339;277.7545;Comment;3;6;7;5;AO;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;10;-1270.17,640.7898;Inherit;False;573;284.2544;Comment;3;9;8;4;Roughness;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;17;-1865.68,-517.165;Inherit;False;1182.65;601.7484;Comment;5;14;13;16;15;1;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;15;-1090.319,-431.1649;Inherit;False;Property;_AlbedoTint;Albedo Tint;0;0;Create;True;0;0;False;0;0.4150943,0.17711,0.03720184,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-1215.712,1131.48;Inherit;False;Property;_AOMult;AO Mult;6;0;Create;True;0;0;False;0;1;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-1804.928,970.2684;Inherit;True;Property;_Packed;Packed;4;0;Create;True;0;0;False;0;-1;79034da1a9306bd48afe0997d9d5c9fa;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1815.68,-145.4166;Inherit;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;False;0;-1;c88ee86373b02364fa904f511226c261;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-1256.17,832.0451;Inherit;False;Property;_RoughnessMulti;Roughness Multi;5;0;Create;True;0;0;False;0;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;18;-1132.009,1376.833;Inherit;False;370;280;Comment;1;2;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;27;-1895.822,147.3553;Inherit;False;676.0115;295.7897;Comment;2;24;20;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1064.937,690.791;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1064.38,1019.725;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1082.009,1426.833;Inherit;True;Property;_Opacity;Opacity;3;0;Create;True;0;0;False;0;-1;924ae6323660e4f41ba1c34fe53d7720;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-879.0309,-167.3166;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-1845.822,327.145;Inherit;False;Property;_NormalIntensity;Normal Intensity;8;0;Create;True;0;0;False;0;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;-514.2917,-516.4575;Inherit;False;Property;_Emission;Emission;9;1;[HDR];Create;True;0;0;False;0;3.031433,3.031433,3.031433,0;3.031433,3.031433,3.031433,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-165.6827,-296.0467;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;25;276.7032,1312.871;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;26;-1373.825,523.7151;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;13;-1394.554,-361.7648;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientNode;14;-1619.154,-482.4648;Inherit;False;0;2;2;1,1,1,0;1,1,1,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.OneMinusNode;4;-876.1703,696.0137;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-1539.811,197.3553;Inherit;True;Property;_Normal;Normal;7;0;Create;True;0;0;False;0;-1;a647c242ce73f6c4aaca12c063520ef3;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;5.73;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;5;-921.3802,1030.125;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;507.4178,16.36831;Float;False;True;-1;4;ASEMaterialInspector;0;0;Standard;GreenLeaves_Custom/Shader_Branch;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;False;TransparentCutout;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;3;4
WireConnection;8;1;9;0
WireConnection;6;0;3;2
WireConnection;6;1;7;0
WireConnection;16;0;1;0
WireConnection;16;1;15;0
WireConnection;28;0;29;0
WireConnection;28;1;16;0
WireConnection;25;0;2;0
WireConnection;26;0;3;1
WireConnection;13;0;14;0
WireConnection;13;1;1;0
WireConnection;4;0;8;0
WireConnection;20;5;24;0
WireConnection;5;0;6;0
WireConnection;0;0;16;0
WireConnection;0;1;20;0
WireConnection;0;2;28;0
WireConnection;0;3;26;0
WireConnection;0;4;4;0
WireConnection;0;5;5;0
WireConnection;0;10;25;0
ASEEND*/
//CHKSM=BB99BE7C2F36B142117B407946F393980D188A72