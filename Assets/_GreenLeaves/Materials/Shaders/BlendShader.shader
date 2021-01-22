// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TerrainBlender"
{
	Properties
	{
		_blendThickness("blendThickness", Range( 0 , 10)) = 0
		_blendThickness1("blendThickness", Range( 0 , 10)) = 0
		_noiseScale("noiseScale", Range( 0 , 1)) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_TextureSample2("Texture Sample 2", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform sampler2D _TextureSample2;
		uniform float4 _TextureSample2_ST;
		uniform sampler2D TB_DEPTH;
		uniform float TB_OFFSET_X;
		uniform float TB_OFFSET_Z;
		uniform float TB_SCALE;
		uniform float TB_FARCLIP;
		uniform float TB_OFFSET_Y;
		uniform float _blendThickness;
		uniform sampler2D _TextureSample0;
		uniform float _noiseScale;
		uniform float _blendThickness1;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			float2 uv_TextureSample2 = i.uv_texcoord * _TextureSample2_ST.xy + _TextureSample2_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float worldY2 = ase_worldPos.y;
			float4 temp_cast_0 = (worldY2).xxxx;
			float2 appendResult3 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 appendResult11 = (float2(TB_OFFSET_X , TB_OFFSET_Z));
			float4 temp_cast_1 = (TB_OFFSET_Y).xxxx;
			float4 clampResult25 = clamp( ( ( ( temp_cast_0 - ( tex2D( TB_DEPTH, ( ( appendResult3 - appendResult11 ) / TB_SCALE ) ) * TB_FARCLIP ) ) - temp_cast_1 ) / ( _blendThickness * tex2D( _TextureSample0, ( (ase_worldPos).xz * _noiseScale ) ) ) ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			float4 temp_cast_2 = (_blendThickness1).xxxx;
			float4 clampResult27 = clamp( pow( clampResult25 , temp_cast_2 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			float4 lerpResult38 = lerp( tex2D( _TextureSample1, uv_TextureSample1 ) , tex2D( _TextureSample2, uv_TextureSample2 ) , clampResult27.r);
			o.Albedo = lerpResult38.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
167;30;1920;1006;533.7925;-274.771;1.029277;True;True
Node;AmplifyShaderEditor.CommentaryNode;15;-1654.4,-90.27203;Inherit;False;1086.032;492;Relative position;9;2;1;11;10;12;13;3;14;16;;0.4292453,0.6657077,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1602.368,179.728;Inherit;False;Global;TB_OFFSET_X;TB_OFFSET_X;0;0;Create;True;0;0;False;0;0;1025.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1602.368,285.7278;Inherit;False;Global;TB_OFFSET_Z;TB_OFFSET_Z;0;0;Create;True;0;0;False;0;0;119.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-1604.4,-26.60001;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;11;-1246.368,226.728;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;-1251.368,95.42797;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-986.3677,285.5086;Inherit;False;Global;TB_SCALE;TB_SCALE;0;0;Create;True;0;0;False;0;0;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;13;-964.368,109.728;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;31;-429.99,122.3008;Inherit;False;2280.767;497.9066;Comment;15;19;17;20;18;21;23;22;24;25;30;26;27;28;29;36;;1,0.2688679,0.2688679,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;14;-720.368,114.728;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2;-1277.368,-39.27206;Inherit;False;worldY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-402.8131,954.9389;Inherit;False;Property;_noiseScale;noiseScale;3;0;Create;True;0;0;False;0;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-213.2179,502.3008;Inherit;False;Global;TB_FARCLIP;TB_FARCLIP;1;0;Create;True;0;0;False;0;0;1000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;-379.99,193.8472;Inherit;True;Global;TB_DEPTH;TB_DEPTH;0;0;Create;True;0;0;False;0;-1;None;4ecc6bf4b3a9c844cb5f079555b16807;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwizzleNode;32;-355.3016,774.6256;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-68.97385,811.2922;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;59.78213,172.3009;Inherit;False;2;worldY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;95.78215,292.3008;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;21;322.1938,298.7013;Inherit;False;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;29;283.0591,530.1704;Float;False;Property;_blendThickness;blendThickness;1;0;Create;True;0;0;False;0;0;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;288.7423,427.9861;Inherit;False;Global;TB_OFFSET_Y;TB_OFFSET_Y;1;0;Create;True;0;0;False;0;0;-860.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;35;149.1869,800.9096;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;623.3637,508.3258;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;539.1941,296.7013;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;24;845.3943,296.486;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;30;867.7936,500.0716;Float;False;Property;_blendThickness1;blendThickness;2;0;Create;True;0;0;False;0;0;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;25;1031.118,297.6866;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;26;1232.705,301.6554;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;27;1420.873,303.6243;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;41;1300.333,-541.6459;Inherit;False;372.5989;560.7999;Textures;2;40;39;;1,0.4386792,0.9818343,1;0;0
Node;AmplifyShaderEditor.SamplerNode;39;1350.333,-491.6459;Inherit;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;40;1352.932,-210.846;Inherit;True;Property;_TextureSample2;Texture Sample 2;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;28;1597.777,304.2056;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;38;1993.833,-106.8458;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2265.883,-99.55976;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;TerrainBlender;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;3;0;1;1
WireConnection;3;1;1;3
WireConnection;13;0;3;0
WireConnection;13;1;11;0
WireConnection;14;0;13;0
WireConnection;14;1;16;0
WireConnection;2;0;1;2
WireConnection;17;1;14;0
WireConnection;32;0;1;0
WireConnection;33;0;32;0
WireConnection;33;1;34;0
WireConnection;18;0;17;0
WireConnection;18;1;19;0
WireConnection;21;0;20;0
WireConnection;21;1;18;0
WireConnection;35;1;33;0
WireConnection;36;0;29;0
WireConnection;36;1;35;0
WireConnection;22;0;21;0
WireConnection;22;1;23;0
WireConnection;24;0;22;0
WireConnection;24;1;36;0
WireConnection;25;0;24;0
WireConnection;26;0;25;0
WireConnection;26;1;30;0
WireConnection;27;0;26;0
WireConnection;28;0;27;0
WireConnection;38;0;39;0
WireConnection;38;1;40;0
WireConnection;38;2;28;0
WireConnection;0;0;38;0
ASEEND*/
//CHKSM=BD9E69C1016E196C4834D16381766F460831E0C1