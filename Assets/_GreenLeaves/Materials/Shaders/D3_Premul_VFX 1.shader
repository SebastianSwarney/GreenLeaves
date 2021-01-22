// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "D3_PreMulBlendAdd_VFX"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (0,0,0,0)
		_NoiseTexture("Noise Texture", 2D) = "white" {}
		_MultiplyAlpha("Multiply Alpha", Range( 0 , 4)) = 4
		[Toggle(_DUALALPHA_ON)] _DualAlpha("DualAlpha", Float) = 0
		[Toggle(_TRIPLEALPHA_ON)] _TripleAlpha("TripleAlpha", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Back
		HLSLINCLUDE
		#pragma target 3.0
		ENDHLSL

		
		Pass
		{
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One OneMinusSrcAlpha , One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#define _ALPHAPREMULTIPLY_ON 1
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#pragma shader_feature_local _TRIPLEALPHA_ON
			#pragma shader_feature_local _DUALALPHA_ON


			sampler2D _Albedo;
			sampler2D _NoiseTexture;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color0;
			float4 _Albedo_ST;
			float _MultiplyAlpha;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_color = v.ase_color;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
				o.clipPos = vertexInput.positionCS;
				#ifdef ASE_FOG
				o.fogFactor = ComputeFogFactor( vertexInput.positionCS.z );
				#endif
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_Albedo = IN.ase_texcoord1 * _Albedo_ST.xy + _Albedo_ST.zw;
				float4 tex2DNode4 = tex2D( _Albedo, uv_Albedo );
				float4 _baseColor60 = ( _Color0 * ( tex2DNode4 * tex2DNode4.a ) );
				float _albedoAlpha50 = tex2DNode4.a;
				float4 uv046 = IN.ase_texcoord1;
				uv046.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float _time71 = uv046.w;
				float _w72 = uv046.z;
				float2 _direction77 = ( _w72 * float2( 0.2,1 ) );
				float2 _uvx170 = (uv046).xy;
				float2 panner83 = ( _time71 * _direction77 + _uvx170);
				float4 tex2DNode81 = tex2D( _NoiseTexture, panner83 );
				float2 _uvx0592 = ( (uv046).xy * 0.5 );
				float2 panner67 = ( _time71 * _direction77 + _uvx0592);
				float temp_output_99_0 = ( tex2D( _NoiseTexture, panner67 ).a * tex2DNode81.a );
				#ifdef _DUALALPHA_ON
				float staticSwitch100 = temp_output_99_0;
				#else
				float staticSwitch100 = tex2DNode81.a;
				#endif
				float2 _uvx296 = ( (uv046).xy * 2.0 );
				float2 panner86 = ( _time71 * _direction77 + _uvx296);
				#ifdef _TRIPLEALPHA_ON
				float staticSwitch102 = ( temp_output_99_0 * tex2D( _NoiseTexture, panner86 ).a );
				#else
				float staticSwitch102 = staticSwitch100;
				#endif
				float _noise49 = ( staticSwitch102 * 0.0 );
				float _Alpha57 = saturate( ( ( _albedoAlpha50 * _noise49 ) * _MultiplyAlpha ) );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( ( _baseColor60 * ( _Alpha57 * 4.0 ) ) * IN.ase_color ).rgb;
				float Alpha = ( IN.ase_color.a * _Alpha57 );
				float AlphaClipThreshold = 0.5;

				#if _AlphaClip
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				return half4( Color, Alpha );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#define _ALPHAPREMULTIPLY_ON 1
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#pragma shader_feature_local _TRIPLEALPHA_ON
			#pragma shader_feature_local _DUALALPHA_ON


			sampler2D _Albedo;
			sampler2D _NoiseTexture;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color0;
			float4 _Albedo_ST;
			float _MultiplyAlpha;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				o.clipPos = TransformObjectToHClip(v.vertex.xyz);
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_Albedo = IN.ase_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
				float4 tex2DNode4 = tex2D( _Albedo, uv_Albedo );
				float _albedoAlpha50 = tex2DNode4.a;
				float4 uv046 = IN.ase_texcoord;
				uv046.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float _time71 = uv046.w;
				float _w72 = uv046.z;
				float2 _direction77 = ( _w72 * float2( 0.2,1 ) );
				float2 _uvx170 = (uv046).xy;
				float2 panner83 = ( _time71 * _direction77 + _uvx170);
				float4 tex2DNode81 = tex2D( _NoiseTexture, panner83 );
				float2 _uvx0592 = ( (uv046).xy * 0.5 );
				float2 panner67 = ( _time71 * _direction77 + _uvx0592);
				float temp_output_99_0 = ( tex2D( _NoiseTexture, panner67 ).a * tex2DNode81.a );
				#ifdef _DUALALPHA_ON
				float staticSwitch100 = temp_output_99_0;
				#else
				float staticSwitch100 = tex2DNode81.a;
				#endif
				float2 _uvx296 = ( (uv046).xy * 2.0 );
				float2 panner86 = ( _time71 * _direction77 + _uvx296);
				#ifdef _TRIPLEALPHA_ON
				float staticSwitch102 = ( temp_output_99_0 * tex2D( _NoiseTexture, panner86 ).a );
				#else
				float staticSwitch102 = staticSwitch100;
				#endif
				float _noise49 = ( staticSwitch102 * 0.0 );
				float _Alpha57 = saturate( ( ( _albedoAlpha50 * _noise49 ) * _MultiplyAlpha ) );
				
				float Alpha = ( IN.ase_color.a * _Alpha57 );
				float AlphaClipThreshold = 0.5;

				#if _AlphaClip
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=17500
0;56;1920;958;1515.702;617.228;1.731534;True;True
Node;AmplifyShaderEditor.CommentaryNode;69;-3716.568,53.37419;Inherit;False;1034.89;1422.553;Comment;17;92;91;90;89;77;75;76;74;70;68;72;71;46;93;94;95;96;UVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-3678.268,351.6739;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;-3369.97,679.0558;Inherit;False;_w;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-3321.363,217.4797;Inherit;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;89;-3382.843,126.9303;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;74;-3655.41,990.6621;Inherit;False;Constant;_Direction;Direction;4;0;Create;True;0;0;False;0;0.2,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;76;-3666.41,895.662;Inherit;False;72;_w;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-3117.363,133.4797;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-3315.619,547.4366;Inherit;False;Constant;_Float2;Float 1;5;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;93;-3377.099,456.8873;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;68;-3360.932,339.3211;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-3375.41,897.6621;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;71;-3373.97,789.0552;Inherit;False;_time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-3111.619,463.4367;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-3117.018,339.0448;Inherit;False;_uvx1;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-3176.41,897.6621;Inherit;False;_direction;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;105;-2473.348,543.5434;Inherit;False;2147.137;923.0493;Comment;22;81;82;40;67;83;86;78;79;80;98;84;85;97;87;88;99;101;103;104;49;100;102;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;-2954.363,132.4797;Inherit;False;_uvx05;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-2420.964,593.543;Inherit;False;92;_uvx05;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-2422.675,926.4867;Inherit;False;77;_direction;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;-2422.964,676.5424;Inherit;False;77;_direction;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;-2948.619,462.4367;Inherit;False;_uvx2;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;-2423.348,843.3334;Inherit;False;70;_uvx1;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-2422.675,1010.487;Inherit;False;71;_time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-2420.964,763.5424;Inherit;False;71;_time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-2419.675,1278.489;Inherit;False;71;_time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;87;-2421.675,1184.488;Inherit;False;77;_direction;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;97;-2423.348,1097.334;Inherit;False;96;_uvx2;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;83;-2145.542,905.6537;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;67;-2146.831,655.7094;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;40;-1887.14,656.6633;Inherit;True;Property;_NoiseTexture;Noise Texture;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;86;-2147.542,1159.655;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;81;-1887.966,911.5424;Inherit;True;Property;_NoiseTexture1;Noise Texture;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;40;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-1517.401,894.5917;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;82;-1888.966,1160.544;Inherit;True;Property;_NoiseTexture2;Noise Texture;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;40;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;100;-1312.401,1008.592;Inherit;False;Property;_DualAlpha;DualAlpha;5;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-1351.401,1241.593;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;102;-1040.399,1219.593;Inherit;False;Property;_TripleAlpha;TripleAlpha;6;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-977.3988,1350.593;Inherit;False;Constant;_Float3;Float 3;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;63;-906.9976,-913.7188;Inherit;False;1144.857;593.6125;Comment;6;8;60;7;5;50;4;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;4;-856.9976,-635.3559;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-749.3988,1224.593;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;64;-2273.225,-901.9446;Inherit;False;1179.799;352.5983;Comment;7;48;51;52;53;56;54;57;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-550.2127,1235.101;Inherit;False;_noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-473.9701,-436.1062;Inherit;False;_albedoAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-2223.225,-851.9445;Inherit;False;50;_albedoAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-2216.623,-736.7568;Inherit;False;49;_noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1966.426,-665.3462;Inherit;False;Property;_MultiplyAlpha;Multiply Alpha;4;0;Create;True;0;0;False;0;4;2;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1887.426,-795.3463;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-1642.426,-792.3463;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;56;-1483.426,-787.3463;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-1317.426,-787.3463;Inherit;False;_Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;11;171.1386,236.7321;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;58;167.759,431.1814;Inherit;False;57;_Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-173.0157,-795.3147;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;60;13.85965,-802.3189;Inherit;False;_baseColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;7;-474.4021,-863.7188;Inherit;False;Property;_Color0;Color 0;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-470.9977,-633.3559;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;295.9822,-105.4308;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;-77.18929,-106.5947;Inherit;False;60;_baseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-77.18953,123.8592;Inherit;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;106.4406,27.75139;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;-77.75639,22.57822;Inherit;False;57;_Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;469.2191,418.6153;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;473.9081,129.3181;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;3;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;857.0507,136.1083;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;D3_PreMulBlendAdd_VFX;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;0;Forward;7;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;True;1;1;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;10;Surface;1;  Blend;1;Two Sided;1;Cast Shadows;0;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;0;Meta Pass;0;Vertex Position,InvertActionOnDeselection;1;0;4;True;False;True;False;False;;0
WireConnection;72;0;46;3
WireConnection;89;0;46;0
WireConnection;90;0;89;0
WireConnection;90;1;91;0
WireConnection;93;0;46;0
WireConnection;68;0;46;0
WireConnection;75;0;76;0
WireConnection;75;1;74;0
WireConnection;71;0;46;4
WireConnection;94;0;93;0
WireConnection;94;1;95;0
WireConnection;70;0;68;0
WireConnection;77;0;75;0
WireConnection;92;0;90;0
WireConnection;96;0;94;0
WireConnection;83;0;98;0
WireConnection;83;2;84;0
WireConnection;83;1;85;0
WireConnection;67;0;78;0
WireConnection;67;2;79;0
WireConnection;67;1;80;0
WireConnection;40;1;67;0
WireConnection;86;0;97;0
WireConnection;86;2;87;0
WireConnection;86;1;88;0
WireConnection;81;1;83;0
WireConnection;99;0;40;4
WireConnection;99;1;81;4
WireConnection;82;1;86;0
WireConnection;100;1;81;4
WireConnection;100;0;99;0
WireConnection;101;0;99;0
WireConnection;101;1;82;4
WireConnection;102;1;100;0
WireConnection;102;0;101;0
WireConnection;103;0;102;0
WireConnection;103;1;104;0
WireConnection;49;0;103;0
WireConnection;50;0;4;4
WireConnection;52;0;48;0
WireConnection;52;1;51;0
WireConnection;53;0;52;0
WireConnection;53;1;54;0
WireConnection;56;0;53;0
WireConnection;57;0;56;0
WireConnection;8;0;7;0
WireConnection;8;1;5;0
WireConnection;60;0;8;0
WireConnection;5;0;4;0
WireConnection;5;1;4;4
WireConnection;62;0;65;0
WireConnection;62;1;61;0
WireConnection;61;0;59;0
WireConnection;61;1;66;0
WireConnection;13;0;11;4
WireConnection;13;1;58;0
WireConnection;12;0;62;0
WireConnection;12;1;11;0
WireConnection;0;2;12;0
WireConnection;0;3;13;0
ASEEND*/
//CHKSM=5E5C8D29F4ADB2924C49AA042F8157C38FAE4186