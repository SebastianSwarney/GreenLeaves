// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shader_Triplanar_Standard"
{
	Properties
	{
		_Albedo("Top_Albedo", 2D) = "white" {}
		_Side_Albedo("Side_Albedo", 2D) = "white" {}
		_Bot_Albedo("Bot_Albedo", 2D) = "white" {}
		_Top_Normal("Top_Normal", 2D) = "bump" {}
		_Side_Normal("Side_Normal", 2D) = "bump" {}
		_Bot_Normal("Bot_Normal", 2D) = "bump" {}
		_Top_Packed("Top_Packed", 2D) = "white" {}
		_Mid_Packed("Mid_Packed", 2D) = "white" {}
		_Bot_Packed("Bot_Packed", 2D) = "white" {}
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_NormalIntensity("Normal Intensity", Range( 0 , 3)) = 0
		_Offset("Offset", Vector) = (0,0,0,0)
		_Falloff("Falloff", Float) = 0
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,1)
		_AOIntensity("AO Intensity", Range( 0 , 2)) = 1
		_RoughnessIntensity("Roughness Intensity", Range( 0 , 3)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#define ASE_TEXTURE_PARAMS(textureName) textureName

		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Top_Normal;
		uniform sampler2D _Side_Normal;
		uniform sampler2D _Bot_Normal;
		uniform float2 _Tiling;
		uniform float _Falloff;
		uniform float3 _Offset;
		uniform float _NormalIntensity;
		uniform float4 _AlbedoTint;
		uniform sampler2D _Albedo;
		uniform sampler2D _Side_Albedo;
		uniform sampler2D _Bot_Albedo;
		uniform sampler2D _Top_Packed;
		uniform sampler2D _Mid_Packed;
		uniform sampler2D _Bot_Packed;
		uniform float _RoughnessIntensity;
		uniform float _AOIntensity;


		inline float3 TriplanarSamplingCNF( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			xNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			yNormN = ( tex2D( ASE_TEXTURE_PARAMS( botTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			xNorm.xyz = half3( UnpackScaleNormal( xNorm, normalScale.y ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz = half3( UnpackScaleNormal( yNorm, normalScale.x ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz = half3( UnpackScaleNormal( zNorm, normalScale.y ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			yNormN.xyz = half3( UnpackScaleNormal( yNormN, normalScale.z ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + yNormN.xyz * negProjNormalY + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSamplingCF( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			xNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			yNormN = ( tex2D( ASE_TEXTURE_PARAMS( botTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + yNormN * negProjNormalY + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 temp_output_8_0 = ( ase_worldPos + _Offset );
			float3 triplanar2 = TriplanarSamplingCNF( _Top_Normal, _Side_Normal, _Bot_Normal, temp_output_8_0, ase_worldNormal, _Falloff, _Tiling, ( float3(1,1,1) * _NormalIntensity ), float3(0,0,0) );
			float3 tanTriplanarNormal2 = mul( ase_worldToTangent, triplanar2 );
			o.Normal = tanTriplanarNormal2;
			float4 triplanar1 = TriplanarSamplingCF( _Albedo, _Side_Albedo, _Bot_Albedo, temp_output_8_0, ase_worldNormal, _Falloff, _Tiling, float3( 1,1,1 ), float3(0,0,0) );
			o.Albedo = ( _AlbedoTint * triplanar1 ).rgb;
			float4 triplanar14 = TriplanarSamplingCF( _Top_Packed, _Mid_Packed, _Bot_Packed, temp_output_8_0, ase_worldNormal, _Falloff, _Tiling, float3( 1,1,1 ), float3(0,0,0) );
			o.Metallic = triplanar14.x;
			o.Smoothness = ( 1.0 - ( triplanar14.w * _RoughnessIntensity ) );
			o.Occlusion = ( 1.0 - ( triplanar14.y * _AOIntensity ) );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
-1920;91;1920;928;2408.835;497.7758;1.3;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;7;-1831.783,-171.9895;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;6;-1834.384,30.81038;Inherit;False;Property;_Offset;Offset;11;0;Create;True;0;0;False;0;0,0,0;-54.48,-42.7,-19.5;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;4;-1620.308,334.1818;Inherit;False;Property;_Tiling;Tiling;9;0;Create;True;0;0;False;0;1,1;0.13,0.01;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;13;-1546.923,101.8445;Inherit;False;Property;_Falloff;Falloff;12;0;Create;True;0;0;False;0;0;2.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-1553.582,-77.08928;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1902.191,763.3175;Inherit;False;Property;_NormalIntensity;Normal Intensity;10;0;Create;True;0;0;False;0;0;2.8;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;10;-1912.96,587.842;Inherit;False;Constant;_Vector0;Vector 0;7;0;Create;True;0;0;False;0;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;18;-971.5188,-510.4933;Inherit;False;802.8811;509.4391;Comment;3;1;17;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-714.7363,1068.894;Inherit;False;Property;_RoughnessIntensity;Roughness Intensity;15;0;Create;True;0;0;False;0;1;1.95;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-771.1497,899.9858;Inherit;False;Property;_AOIntensity;AO Intensity;14;0;Create;True;0;0;False;0;1;0.52;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;14;-891.0385,642.6169;Inherit;True;Cylindrical;World;False;Top_Packed;_Top_Packed;white;6;Assets/_GreenLeaves/Materials/Textures/Cliff_01/Cliff_01_basecolor.png;Mid_Packed;_Mid_Packed;white;7;None;Bot_Packed;_Bot_Packed;white;8;None;Packed Triplanar Sampler;False;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT3;1,1,1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1663.191,592.3175;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TriplanarNode;1;-921.5188,-232.054;Inherit;True;Cylindrical;World;False;Top_Albedo;_Albedo;white;0;Assets/_GreenLeaves/Materials/Textures/Cliff_01/Cliff_01_basecolor.png;Side_Albedo;_Side_Albedo;white;1;None;Bot_Albedo;_Bot_Albedo;white;2;None;Albedo Triplanar Sampler;False;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT3;1,1,1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-166.7124,620.6822;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-196.5453,826.7379;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-731.5283,-460.4933;Inherit;False;Property;_AlbedoTint;Albedo Tint;13;0;Create;True;0;0;False;0;1,1,1,1;0.9528302,0.9528302,0.9528302,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;2;-859,188;Inherit;True;Cylindrical;World;True;Top_Normal;_Top_Normal;bump;3;None;Side_Normal;_Side_Normal;bump;4;None;Bot_Normal;_Bot_Normal;bump;5;None;Normal Triplanar Sampler;False;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT3;1,1,1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-330.6377,-256.004;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;35;-39.8125,626.0956;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;28;-40.80672,807.9098;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;259,8;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Shader_Triplanar_Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;7;0
WireConnection;8;1;6;0
WireConnection;14;9;8;0
WireConnection;14;3;4;0
WireConnection;14;4;13;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;1;9;8;0
WireConnection;1;3;4;0
WireConnection;1;4;13;0
WireConnection;29;0;14;2
WireConnection;29;1;30;0
WireConnection;31;0;14;4
WireConnection;31;1;32;0
WireConnection;2;9;8;0
WireConnection;2;8;11;0
WireConnection;2;3;4;0
WireConnection;2;4;13;0
WireConnection;17;0;16;0
WireConnection;17;1;1;0
WireConnection;35;0;29;0
WireConnection;28;0;31;0
WireConnection;0;0;17;0
WireConnection;0;1;2;0
WireConnection;0;3;14;1
WireConnection;0;4;28;0
WireConnection;0;5;35;0
ASEEND*/
//CHKSM=9458AA92629416266AB3552CAD869755BF58FBF9