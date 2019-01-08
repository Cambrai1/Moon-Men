// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Super Shader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_Metalic("Metalic", Range( 0 , 1)) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_MetalicMap("Metalic Map", 2D) = "white" {}
		_SmoothnessMap("Smoothness Map", 2D) = "white" {}
		_Emission("Emission", 2D) = "white" {}
		[HDR]_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_Offset("Offset", Vector) = (0,0,0,0)
		_OutlineSize("Outline Size", Range( 0 , 0.005)) = 0
		_ShowOutline("Show Outline", Int) = 0
		[HDR]_OutlineColour("Outline Colour", Color) = (4,4,4,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		#include "UnityShaderVariables.cginc"
		
		
		struct Input
		{
			half filler;
		};
		uniform float _OutlineSize;
		uniform int _ShowOutline;
		uniform float4 _OutlineColour;
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 objToWorld31 = mul( unity_ObjectToWorld, float4( ase_vertex3Pos, 1 ) ).xyz;
			float outlineVar = ( distance( _WorldSpaceCameraPos , objToWorld31 ) * _OutlineSize *  ( (float)_ShowOutline - 0.0 > 1.0 ? 1.0 : (float)_ShowOutline - 0.0 <= 1.0 && (float)_ShowOutline + 0.0 >= 1.0 ? 1.0 : 0.0 )  );
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _OutlineColour.rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float2 _Tiling;
		uniform float2 _Offset;
		uniform float4 _EmissionColor;
		uniform sampler2D _Emission;
		uniform float _Metalic;
		uniform sampler2D _MetalicMap;
		uniform sampler2D _SmoothnessMap;
		uniform float _Smoothness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord15 = i.uv_texcoord * _Tiling + _Offset;
			o.Albedo = ( _Color * tex2D( _Albedo, uv_TexCoord15 ) ).rgb;
			o.Emission = ( _EmissionColor * tex2D( _Emission, uv_TexCoord15 ) ).rgb;
			o.Metallic = ( _Metalic * tex2D( _MetalicMap, uv_TexCoord15 ) ).r;
			o.Smoothness = ( tex2D( _SmoothnessMap, uv_TexCoord15 ) * _Smoothness ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
-1697;142;1566;929;1563.186;300.2929;1.561433;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;39;-672.275,511.5233;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;16;-1280,-384;Float;False;Property;_Tiling;Tiling;9;0;Create;True;0;0;False;0;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TransformPositionNode;31;-432,496;Float;False;Object;World;False;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;33;-432,336;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.IntNode;36;-368,736;Float;False;Property;_ShowOutline;Show Outline;12;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.Vector2Node;17;-1280,-272;Float;False;Property;_Offset;Offset;10;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-1120,-368;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCIf;38;-160,736;Float;False;6;0;INT;0;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;32;-112,336;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-256,640;Float;False;Property;_OutlineSize;Outline Size;11;0;Create;True;0;0;False;0;0;0.005;0;0.005;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-128,-240;Float;False;Property;_Smoothness;Smoothness;1;0;Create;True;0;0;False;0;0.5;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-432,-544;Float;True;Property;_Albedo;Albedo;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;18;-848,144;Float;False;Property;_EmissionColor;Emission Color;8;1;[HDR];Create;True;0;0;False;0;1,1,1,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;40;64,336;Float;False;Property;_OutlineColour;Outline Colour;13;1;[HDR];Create;True;0;0;False;0;4,4,4,0;4,4,4,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;64,544;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0.05;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-96,-16;Float;False;Property;_Metalic;Metalic;2;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-848,304;Float;True;Property;_Emission;Emission;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;12;16,-608;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,1;0.5849056,0.5849056,0.5849056,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-432,-368;Float;True;Property;_SmoothnessMap;Smoothness Map;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-432,-16;Float;True;Property;_MetalicMap;Metalic Map;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OutlineNode;25;304,336;Float;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;5;-432,-192;Float;True;Property;_NormalMap;Normal Map;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-528.7461,189.0332;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;80,-160;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;48,-352;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;208,-432;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;560,160;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Super Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;6.8;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.005;1,1,1,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;31;0;39;0
WireConnection;15;0;16;0
WireConnection;15;1;17;0
WireConnection;38;0;36;0
WireConnection;32;0;33;0
WireConnection;32;1;31;0
WireConnection;1;1;15;0
WireConnection;28;0;32;0
WireConnection;28;1;35;0
WireConnection;28;2;38;0
WireConnection;19;1;15;0
WireConnection;23;1;15;0
WireConnection;2;1;15;0
WireConnection;25;0;40;0
WireConnection;25;1;28;0
WireConnection;5;1;15;0
WireConnection;20;0;18;0
WireConnection;20;1;19;0
WireConnection;22;0;21;0
WireConnection;22;1;2;0
WireConnection;24;0;23;0
WireConnection;24;1;3;0
WireConnection;13;0;12;0
WireConnection;13;1;1;0
WireConnection;0;0;13;0
WireConnection;0;2;20;0
WireConnection;0;3;22;0
WireConnection;0;4;24;0
WireConnection;0;11;25;0
ASEEND*/
//CHKSM=284D7F96AC801BA797795473FF994FE133043E7C