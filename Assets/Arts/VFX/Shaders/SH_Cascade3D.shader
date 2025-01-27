// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Tartaros/Cascade3D"
{
	Properties
	{
		[HDR]_MainColor2("MainColor2", Color) = (0,0.7686272,1,1)
		[HDR]_MainColor("MainColor", Color) = (0,0.7686272,1,1)
		_VoroSpeed("VoroSpeed", Float) = 2
		_VoroScale("VoroScale", Float) = 10
		_RippleAmount2("RippleAmount2", Float) = 2
		_VertexOffset("VertexOffset", Float) = 2
		_BottomStrength("BottomStrength", Float) = 30
		_NoiseScale("NoiseScale", Float) = 20
		_BottomBrightness("BottomBrightness", Float) = 3
		_WhiteLinesTilingWidth("WhiteLinesTilingWidth", Vector) = (1.5,0,0,0)
		_WhiteLinesTilingOffset("WhiteLinesTilingOffset", Vector) = (0.2,0,0,0)
		_NoiseStrength("NoiseStrength", Float) = 0
		_RippleSpeed("RippleSpeed", Vector) = (0,1,0,0)
		_SmoothStep("SmoothStep", Vector) = (0.04,0.8,0,0)
		_VoroTiling("VoroTiling", Vector) = (0,0,0,0)
		_WhiteLinesStrength("WhiteLinesStrength", Float) = 5
		[HDR]_WhiteLines("WhiteLines", Color) = (1,1,1,0)
		_WhiteOuterStep("WhiteOuterStep", Range( -2 , 0.49)) = -0.17
		_WhiteInnerStep("WhiteInnerStep", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
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
			float2 vertexToFrag152;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _NoiseScale;
		uniform float2 _VoroTiling;
		uniform float3 _RippleSpeed;
		uniform float _NoiseStrength;
		uniform float _BottomStrength;
		uniform float _BottomBrightness;
		uniform float _VertexOffset;
		uniform float _RippleAmount2;
		uniform float2 _SmoothStep;
		uniform float _VoroScale;
		uniform float _VoroSpeed;
		uniform float4 _MainColor;
		uniform float4 _MainColor2;
		uniform float _WhiteInnerStep;
		uniform float _WhiteOuterStep;
		uniform float2 _WhiteLinesTilingWidth;
		uniform float2 _WhiteLinesTilingOffset;
		uniform float _WhiteLinesStrength;
		uniform float4 _WhiteLines;


		float2 voronoihash108( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi108( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -3; j <= 3; j++ )
			{
				for ( int i = -3; i <= 3; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash108( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return (F2 + F1) * 0.5;
		}


		float2 voronoihash9( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi9( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -3; j <= 3; j++ )
			{
				for ( int i = -3; i <= 3; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash9( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.707 * sqrt(dot( r, r ));
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return (F2 + F1) * 0.5;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float time108 = 0.0;
			float2 uv_TexCoord146 = v.texcoord.xy * _VoroTiling;
			float3 temp_output_6_0 = ( float3( ( uv_TexCoord146 + v.texcoord.xy ) ,  0.0 ) + ( _RippleSpeed * _Time.y ) );
			float2 coords108 = temp_output_6_0.xy * _NoiseScale;
			float2 id108 = 0;
			float2 uv108 = 0;
			float fade108 = 0.5;
			float voroi108 = 0;
			float rest108 = 0;
			for( int it108 = 0; it108 <8; it108++ ){
			voroi108 += fade108 * voronoi108( coords108, time108, id108, uv108, 0 );
			rest108 += fade108;
			coords108 *= 2;
			fade108 *= 0.5;
			}//Voronoi108
			voroi108 /= rest108;
			float4 ase_vertexTangent = v.tangent;
			float TexCoordTangentY168 = ( ( 1.0 - v.texcoord.xy.y ) * ( 1.0 - ase_vertexTangent.xyz.y ) );
			float BottomStr88 = (0.0 + (( ( voroi108 + _NoiseStrength ) * pow( TexCoordTangentY168 , _BottomStrength ) ) - 0.0) * (_BottomBrightness - 0.0) / (1.0 - 0.0));
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( BottomStr88 * _VertexOffset * ase_vertexNormal );
			v.vertex.w = 1;
			o.vertexToFrag152 = uv_TexCoord146;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float temp_output_4_0 = ( _Time.y * _VoroSpeed );
			float time9 = temp_output_4_0;
			float3 temp_output_6_0 = ( float3( ( i.vertexToFrag152 + i.uv_texcoord ) ,  0.0 ) + ( _RippleSpeed * _Time.y ) );
			float2 coords9 = temp_output_6_0.xy * _VoroScale;
			float2 id9 = 0;
			float2 uv9 = 0;
			float voroi9 = voronoi9( coords9, time9, id9, uv9, 0 );
			float smoothstepResult174 = smoothstep( _SmoothStep.x , _SmoothStep.y , saturate( voroi9 ));
			float Voronoi2171 = ( _RippleAmount2 + smoothstepResult174 );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float4 ase_vertexTangent = mul( unity_WorldToObject, float4( ase_worldTangent, 0 ) );
			float TexCoordTangentY168 = ( ( 1.0 - i.uv_texcoord.y ) * ( 1.0 - ase_vertexTangent.xyz.y ) );
			float4 lerpResult164 = lerp( _MainColor , _MainColor2 , TexCoordTangentY168);
			float4 temp_cast_2 = (_WhiteInnerStep).xxxx;
			float4 temp_cast_3 = (_WhiteOuterStep).xxxx;
			float2 uv_TexCoord189 = i.uv_texcoord * _WhiteLinesTilingWidth + _WhiteLinesTilingOffset;
			float clampResult206 = clamp( ( 1.0 - length( uv_TexCoord189.x ) ) , 0.0 , 1.0 );
			float4 WhiteLines198 = ( ( 1.0 - ( saturate( clampResult206 ) * _WhiteLinesStrength ) ) * _WhiteLines );
			float4 clampResult225 = clamp( ( 1.0 - ( WhiteLines198 * Voronoi2171 ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 smoothstepResult226 = smoothstep( temp_cast_2 , temp_cast_3 , clampResult225);
			o.Emission = ( float4( 0,0,0,0 ) + ( ( Voronoi2171 * lerpResult164 ) + smoothstepResult226 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
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
				float4 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.vertexToFrag152;
				o.customPack1.zw = customInputData.uv_texcoord;
				o.customPack1.zw = v.texcoord;
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
				surfIN.vertexToFrag152 = IN.customPack1.xy;
				surfIN.uv_texcoord = IN.customPack1.zw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
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
Version=18500
249;73;1146;575;-404.1096;587.5797;3.349838;True;False
Node;AmplifyShaderEditor.Vector2Node;148;-3235.877,-1414.042;Inherit;False;Property;_VoroTiling;VoroTiling;17;0;Create;True;0;0;False;0;False;0,0;2,-0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;187;235.0244,-1077.909;Inherit;False;Property;_WhiteLinesTilingWidth;WhiteLinesTilingWidth;11;0;Create;True;0;0;False;0;False;1.5,0;4,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;188;224.5494,-944.253;Inherit;False;Property;_WhiteLinesTilingOffset;WhiteLinesTilingOffset;12;0;Create;True;0;0;False;0;False;0.2,0;-0.8,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;146;-2972.977,-1423.81;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;189;537.1174,-1022.579;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,-0.14;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;153;-2656.563,-973.5715;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;128;-2926.376,-771.9775;Inherit;False;Property;_RippleSpeed;RippleSpeed;14;0;Create;True;0;0;False;0;False;0,1,0;0,0.3,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexToFragmentNode;152;-2591.371,-1225.137;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LengthOpNode;208;791.926,-973.3317;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;7;-2956.082,-596.2379;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-2696.238,-626.8259;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;159;-2396.477,-791.9057;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-2937.216,-437.9511;Inherit;False;Property;_VoroSpeed;VoroSpeed;3;0;Create;True;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;190;1008.209,-971.2466;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-2409.319,-441.1249;Inherit;False;Property;_VoroScale;VoroScale;4;0;Create;True;0;0;False;0;False;10;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-2699.79,-529.7104;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;206;1254.626,-978.0316;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-2279.764,-645.2219;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;195;1681.593,-754.5786;Inherit;False;Property;_WhiteLinesStrength;WhiteLinesStrength;22;0;Create;True;0;0;False;0;False;5;1.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;9;-1970.725,-552.8002;Inherit;True;2;1;1;3;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SaturateNode;207;1476.926,-978.0312;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;142;-1602.729,-761.9019;Inherit;False;Property;_SmoothStep;SmoothStep;16;0;Create;True;0;0;False;0;False;0.04,0.8;-0.1,1.23;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;1907.078,-1002.045;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;85;-2929.609,156.5435;Inherit;False;2885.165;676.2094;Comment;16;106;88;30;27;53;25;96;26;103;98;99;94;95;108;130;168;BottomStrength;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;175;-1532.003,-145.8843;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;193;2228.131,-1020.183;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;174;-1327.799,-167.5059;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.04;False;2;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-973.399,-202.2272;Inherit;False;Property;_RippleAmount2;RippleAmount2;5;0;Create;True;0;0;False;0;False;2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;197;2220.944,-756.9433;Inherit;False;Property;_WhiteLines;WhiteLines;23;1;[HDR];Create;True;0;0;False;0;False;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TangentVertexDataNode;94;-2869.103,594.8365;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;95;-2865.346,271.1183;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;196;2541.652,-892.8977;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;98;-2599.401,386.0131;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;182;-722.202,-126.9674;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;99;-2611.573,539.4992;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;198;2882.137,-898.596;Inherit;False;WhiteLines;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-2309.192,349.5825;Inherit;False;Property;_NoiseScale;NoiseScale;9;0;Create;True;0;0;False;0;False;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;171;-292.3643,-117.0555;Inherit;True;Voronoi2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-2430.538,455.5987;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-1833.701,367.6371;Inherit;False;Property;_NoiseStrength;NoiseStrength;13;0;Create;True;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-2223.887,559.187;Inherit;False;TexCoordTangentY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;201;581.2283,684.0725;Inherit;True;198;WhiteLines;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;589.6283,895.3685;Inherit;True;171;Voronoi2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-2216.537,704.4222;Inherit;False;Property;_BottomStrength;BottomStrength;8;0;Create;True;0;0;False;0;False;30;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;108;-2098.245,268.3793;Inherit;True;2;0;1;3;8;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.PowerNode;25;-1822,572.7663;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;130;-1612.34,279.4176;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;214;914.9124,831.5682;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;166;115.6325,638.5632;Inherit;False;Property;_MainColor2;MainColor2;0;1;[HDR];Create;True;0;0;False;0;False;0,0.7686272,1,1;0,0.06603774,0.1320755,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1208.973,438.6341;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;167;111.0915,843.0223;Inherit;False;168;TexCoordTangentY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-982.996,531.7839;Inherit;False;Property;_BottomBrightness;BottomBrightness;10;0;Create;True;0;0;False;0;False;3;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;212;905.4413,595.2091;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;161;127.3461,459.4987;Inherit;False;Property;_MainColor;MainColor;1;1;[HDR];Create;True;0;0;False;0;False;0,0.7686272,1,1;0.02870238,0.2601632,0.4056604,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;229;1345.592,788.114;Inherit;False;Property;_WhiteOuterStep;WhiteOuterStep;25;0;Create;True;0;0;False;0;False;-0.17;0.02;-2;0.49;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;225;1124.878,637.2784;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;228;1411.486,698.6016;Inherit;False;Property;_WhiteInnerStep;WhiteInnerStep;26;0;Create;True;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;730.0971,325.348;Inherit;True;171;Voronoi2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;30;-735.8056,440.8328;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;164;372.0597,531.9927;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-361.6353,435.135;Inherit;False;BottomStr;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;1183.452,386.2753;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;226;1779.968,606.0079;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;37;2067.206,700.2565;Inherit;False;Property;_VertexOffset;VertexOffset;7;0;Create;True;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;1067.654,947.6905;Inherit;True;88;BottomStr;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;222;1963.034,486.8355;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;80;2059.663,783.0145;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;86;172.9027,179.8721;Inherit;True;87;Voronoi;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;163;2255.826,391.6974;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;10;-867.8223,-493.0077;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;169;-1965.539,-274.4774;Inherit;True;2;1;1;4;8;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;87;-329.709,-498.4912;Inherit;True;Voronoi;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-554.4041,-297.3069;Inherit;False;Property;_Remap;Remap;15;0;Create;True;0;0;False;0;False;0.1;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;20;-560.6981,-492.62;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;129;-1378.849,-756.4466;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.04;False;2;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;134;545.8166,150.9714;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;160;-1587.973,-536.3327;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;-2383.546,-310.8433;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;1220.012,273.3938;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;183;-810.7006,-590.6572;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;2371.636,575.4186;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-1755.043,-408.2125;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;200;2775.546,432.6686;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;150;-2941.912,-1151.153;Inherit;False;Property;_RADIAL_Strength;RADIAL_Strength;19;0;Create;True;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.StepOpNode;155;-1076.964,-735.5356;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;186;2555.367,686.9941;Inherit;False;Property;_Float0;Float 0;21;0;Create;True;0;0;False;0;False;10000;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;203;2819.229,78.89822;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;221;1501.603,455.7665;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;149;-2940.913,-1022.305;Inherit;False;Property;_RADIAL_Offset;RADIAL_Offset;18;0;Create;True;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.VertexColorNode;16;2149.375,208.8059;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;600.4597,-166.6001;Inherit;False;Property;_RipplesColor;RipplesColor;2;1;[HDR];Create;True;0;0;False;0;False;0,0.7686272,1,1;0.1132075,0.1132075,0.1132075,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;184;-1321.497,-468.0665;Inherit;False;Property;_Step;Step;20;0;Create;True;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;205;2346.675,146.5417;Inherit;False;Property;_LerpWhiteToPatern;LerpWhiteToPatern;24;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1096.603,-406.804;Inherit;False;Property;_RippleAmount;RippleAmount;6;0;Create;True;0;0;False;0;False;2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;2475.561,293.0664;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3033.031,256.1895;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Tartaros/Cascade3D;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;True;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;146;0;148;0
WireConnection;189;0;187;0
WireConnection;189;1;188;0
WireConnection;152;0;146;0
WireConnection;208;0;189;1
WireConnection;5;0;128;0
WireConnection;5;1;7;2
WireConnection;159;0;152;0
WireConnection;159;1;153;0
WireConnection;190;0;208;0
WireConnection;4;0;7;2
WireConnection;4;1;1;0
WireConnection;206;0;190;0
WireConnection;6;0;159;0
WireConnection;6;1;5;0
WireConnection;9;0;6;0
WireConnection;9;1;4;0
WireConnection;9;2;2;0
WireConnection;207;0;206;0
WireConnection;194;0;207;0
WireConnection;194;1;195;0
WireConnection;175;0;9;0
WireConnection;193;0;194;0
WireConnection;174;0;175;0
WireConnection;174;1;142;1
WireConnection;174;2;142;2
WireConnection;196;0;193;0
WireConnection;196;1;197;0
WireConnection;98;0;95;2
WireConnection;182;0;177;0
WireConnection;182;1;174;0
WireConnection;99;0;94;2
WireConnection;198;0;196;0
WireConnection;171;0;182;0
WireConnection;96;0;98;0
WireConnection;96;1;99;0
WireConnection;168;0;96;0
WireConnection;108;0;6;0
WireConnection;108;2;103;0
WireConnection;25;0;168;0
WireConnection;25;1;26;0
WireConnection;130;0;108;0
WireConnection;130;1;106;0
WireConnection;214;0;201;0
WireConnection;214;1;213;0
WireConnection;27;0;130;0
WireConnection;27;1;25;0
WireConnection;212;0;214;0
WireConnection;225;0;212;0
WireConnection;30;0;27;0
WireConnection;30;4;53;0
WireConnection;164;0;161;0
WireConnection;164;1;166;0
WireConnection;164;2;167;0
WireConnection;88;0;30;0
WireConnection;162;0;176;0
WireConnection;162;1;164;0
WireConnection;226;0;225;0
WireConnection;226;1;228;0
WireConnection;226;2;229;0
WireConnection;222;0;162;0
WireConnection;222;1;226;0
WireConnection;163;1;222;0
WireConnection;10;0;155;0
WireConnection;10;1;11;0
WireConnection;169;0;6;0
WireConnection;169;1;185;0
WireConnection;169;2;2;0
WireConnection;87;0;20;0
WireConnection;20;0;10;0
WireConnection;20;3;137;0
WireConnection;129;0;160;0
WireConnection;129;1;142;1
WireConnection;129;2;142;2
WireConnection;134;0;86;0
WireConnection;160;0;170;0
WireConnection;185;0;4;0
WireConnection;13;0;15;0
WireConnection;13;1;134;0
WireConnection;36;0;89;0
WireConnection;36;1;37;0
WireConnection;36;2;80;0
WireConnection;170;0;9;0
WireConnection;170;1;169;0
WireConnection;155;0;129;0
WireConnection;203;2;205;0
WireConnection;221;1;225;0
WireConnection;14;0;16;0
WireConnection;0;2;163;0
WireConnection;0;11;36;0
ASEEND*/
//CHKSM=7FCEBAA9DA9F98C593A74E1F39E4A0E1282798EE