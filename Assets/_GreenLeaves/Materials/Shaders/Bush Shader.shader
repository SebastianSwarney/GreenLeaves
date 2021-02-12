// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GreenLeaves_Custom/Bush Shader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.7
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_Packed("Packed", 2D) = "white" {}
		_RoughnessIntensity("Roughness Intensity", Range( 0 , 3)) = 0
		_AOPower("AO Power", Range( 0 , 5)) = 0
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,1)
		_EmissionTint("Emission Tint", Color) = (1,1,1,1)
		_Offset("Offset", Vector) = (0,0,0,0)
		_Tiling("Tiling", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TreeTransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float2 _Tiling;
		uniform float2 _Offset;
		uniform float4 _AlbedoTint;
		uniform sampler2D _Albedo;
		uniform float4 _EmissionTint;
		uniform float _RoughnessIntensity;
		uniform sampler2D _Packed;
		uniform float _AOPower;
		uniform float _Cutoff = 0.7;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord14 = i.uv_texcoord * _Tiling + _Offset;
			o.Normal = tex2D( _Normal, uv_TexCoord14 ).rgb;
			o.Albedo = ( _AlbedoTint * tex2D( _Albedo, uv_TexCoord14 ) ).rgb;
			o.Emission = _EmissionTint.rgb;
			o.Metallic = 0.0;
			float4 tex2DNode2 = tex2D( _Packed, uv_TexCoord14 );
			o.Smoothness = ( 1.0 - ( _RoughnessIntensity * tex2DNode2.b ) );
			o.Occlusion = pow( tex2DNode2.r , _AOPower );
			o.Alpha = 1;
			clip( tex2DNode2.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
-1920;81;1920;922;1349.496;896.2126;1;True;True
Node;AmplifyShaderEditor.Vector2Node;16;-1707.524,180.9597;Inherit;False;Property;_Offset;Offset;8;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;15;-1718.524,23.95972;Inherit;False;Property;_Tiling;Tiling;9;0;Create;True;0;0;False;0;0,0;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-1395.157,38.4426;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-893.813,200.2461;Inherit;False;Property;_RoughnessIntensity;Roughness Intensity;4;0;Create;True;0;0;False;0;0;1.72;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-903.063,416.6508;Inherit;True;Property;_Packed;Packed;3;0;Create;True;0;0;False;0;-1;None;04d9bbe07aadc374da0d7e13b01ad6b4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-588.813,195.2461;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-885.813,336.2461;Inherit;False;Property;_AOPower;AO Power;5;0;Create;True;0;0;False;0;0;2.14;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-908.663,-226.8492;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;-1;None;425f6f5bffb985949b81676657bd8555;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-925.8604,-448.0756;Inherit;False;Property;_AlbedoTint;Albedo Tint;6;0;Create;True;0;0;False;0;1,1,1,1;0.7096785,0.8773585,0.6828498,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;4;-408.9633,197.9509;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;8;-505.813,324.2461;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-250.3792,77.22812;Inherit;False;Constant;_Metallic;Metallic;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-367.5846,-284.4678;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-907.0386,-6.259491;Inherit;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;-1;None;9c986da97cf97834d88bc50f09cb084e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-362.4963,-521.2126;Inherit;False;Property;_EmissionTint;Emission Tint;7;0;Create;True;0;0;False;0;1,1,1,1;0.7096785,0.8773585,0.6828498,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,-1;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;GreenLeaves_Custom/Bush Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.7;True;True;0;True;TreeTransparentCutout;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;15;0
WireConnection;14;1;16;0
WireConnection;2;1;14;0
WireConnection;7;0;6;0
WireConnection;7;1;2;3
WireConnection;1;1;14;0
WireConnection;4;0;7;0
WireConnection;8;0;2;1
WireConnection;8;1;9;0
WireConnection;12;0;13;0
WireConnection;12;1;1;0
WireConnection;5;1;14;0
WireConnection;0;0;12;0
WireConnection;0;1;5;0
WireConnection;0;2;17;0
WireConnection;0;3;11;0
WireConnection;0;4;4;0
WireConnection;0;5;8;0
WireConnection;0;10;2;4
ASEEND*/
//CHKSM=CC66A921E0D33123C858089CA792FA3D1728BECD