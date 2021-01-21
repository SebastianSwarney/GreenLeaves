// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shader_Terrain"
{
	Properties
	{
		_Tex_Albedo("Tex_Albedo", 2D) = "white" {}
		_Tex_Normal("Tex_Normal", 2D) = "bump" {}
		_Tex_Packed("Tex_Packed", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,0)
		_AOStrength("AO Strength", Range( 0 , 1)) = 0
		_RoughnessScale("Roughness Scale", Range( 0 , 3)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 4.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Tex_Normal;
		uniform float4 _Tex_Normal_ST;
		uniform float4 _AlbedoTint;
		uniform sampler2D _Tex_Albedo;
		uniform float4 _Tex_Albedo_ST;
		uniform sampler2D _Tex_Packed;
		uniform float4 _Tex_Packed_ST;
		uniform float _RoughnessScale;
		uniform float _AOStrength;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Tex_Normal = i.uv_texcoord * _Tex_Normal_ST.xy + _Tex_Normal_ST.zw;
			o.Normal = tex2D( _Tex_Normal, uv_Tex_Normal ).rgb;
			float2 uv_Tex_Albedo = i.uv_texcoord * _Tex_Albedo_ST.xy + _Tex_Albedo_ST.zw;
			o.Albedo = ( _AlbedoTint * tex2D( _Tex_Albedo, uv_Tex_Albedo ) ).rgb;
			float2 uv_Tex_Packed = i.uv_texcoord * _Tex_Packed_ST.xy + _Tex_Packed_ST.zw;
			float4 tex2DNode3 = tex2D( _Tex_Packed, uv_Tex_Packed );
			float4 clampResult13 = clamp( ( 1.0 - ( tex2DNode3 + _RoughnessScale ) ) , float4( 0,0,0,1 ) , float4( 1,1,1,1 ) );
			o.Smoothness = clampResult13.r;
			float lerpResult7 = lerp( tex2DNode3.b , ( tex2DNode3.b * tex2DNode3.b ) , _AOStrength);
			o.Occlusion = lerpResult7;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
-1993;84;1920;970;2312.628;460.9357;1.395822;True;True
Node;AmplifyShaderEditor.CommentaryNode;16;-1152.251,348.0725;Inherit;False;911.0361;276.0366;Roughness;4;15;14;13;11;;0.6367924,0.898025,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1056.251,513.1091;Inherit;False;Property;_RoughnessScale;Roughness Scale;5;0;Create;True;0;0;False;0;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-1653.708,389.198;Inherit;True;Property;_Tex_Packed;Tex_Packed;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;9;-1005.663,951.9246;Inherit;False;664.5453;340.9979;AO;3;6;7;8;;0.254717,0.2511125,0.2511125,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-753.759,397.0725;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-818.085,1032.742;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-955.6628,1176.922;Inherit;False;Property;_AOStrength;AO Strength;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;14;-575.1168,396.8211;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1600.234,-384.1102;Inherit;True;Property;_Tex_Albedo;Tex_Albedo;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-1571.12,-624.0353;Inherit;False;Property;_AlbedoTint;Albedo Tint;3;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;7;-523.1175,1001.925;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;13;-392.2145,397.2431;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,1;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1072.539,-377.4963;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-1639.945,-4.789714;Inherit;True;Property;_Tex_Normal;Tex_Normal;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;4;ASEMaterialInspector;0;0;Standard;Shader_Terrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;3;0
WireConnection;15;1;11;0
WireConnection;6;0;3;3
WireConnection;6;1;3;3
WireConnection;14;0;15;0
WireConnection;7;0;3;3
WireConnection;7;1;6;0
WireConnection;7;2;8;0
WireConnection;13;0;14;0
WireConnection;4;0;5;0
WireConnection;4;1;1;0
WireConnection;0;0;4;0
WireConnection;0;1;2;0
WireConnection;0;4;13;0
WireConnection;0;5;7;0
ASEEND*/
//CHKSM=15F7EE06C2C6A35A21FDC758F8684CFBEAABF509