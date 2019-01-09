// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hologram Shader"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,0)
		[HDR]_Emission("Emission", Color) = (0,0,0,0)
		_Speed("Speed", Float) = -20
		_Texture0("Texture 0", 2D) = "white" {}
		_Dots("Dots", 2D) = "white" {}
		_Tilling2("Tilling 2", Vector) = (0,0,0,0)
		_Tilling("Tilling", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _Texture0;
		uniform float2 _Tilling;
		uniform float _Speed;
		uniform float2 _Tilling2;
		uniform sampler2D _Dots;
		uniform float4 _Dots_ST;
		uniform float4 _Emission;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_46_0 = ( ( _Time.y * _Speed ) * 0.02 );
			float2 temp_cast_0 = (temp_output_46_0).xx;
			float2 uv_TexCoord48 = i.uv_texcoord * _Tilling + temp_cast_0;
			float2 temp_cast_1 = (temp_output_46_0).xx;
			float2 uv_TexCoord50 = i.uv_texcoord * _Tilling2 + temp_cast_1;
			float2 uv_Dots = i.uv_texcoord * _Dots_ST.xy + _Dots_ST.zw;
			float4 tex2DNode53 = tex2D( _Dots, uv_Dots );
			float temp_output_47_0 =  ( tex2D( _Texture0, uv_TexCoord48 ).r - 0.0 > tex2D( _Texture0, uv_TexCoord50 ).r ? tex2DNode53.r : tex2D( _Texture0, uv_TexCoord48 ).r - 0.0 <= tex2D( _Texture0, uv_TexCoord50 ).r && tex2D( _Texture0, uv_TexCoord48 ).r + 0.0 >= tex2D( _Texture0, uv_TexCoord50 ).r ? tex2DNode53.r : 0.0 ) ;
			o.Albedo = ( _Color * temp_output_47_0 ).rgb;
			o.Emission = ( _Emission * temp_output_47_0 ).rgb;
			o.Alpha = temp_output_47_0;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
255;92;1296;899;1288.446;544.305;2.084977;True;True
Node;AmplifyShaderEditor.RangedFloatNode;19;-1786.553,327.3085;Float;False;Property;_Speed;Speed;2;0;Create;True;0;0;False;0;-20;-20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;16;-1786.553,263.3085;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1610.553,263.3085;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;54;-1226.175,357.3102;Float;False;Property;_Tilling;Tilling;6;0;Create;True;0;0;False;0;0,0;0.1,-0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;55;-1441.974,987.8102;Float;False;Property;_Tilling2;Tilling 2;5;0;Create;True;0;0;False;0;0,0;0.2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1374.748,480.4332;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.02;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;48;-1055.889,599.3579;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;51;-1564.337,660.4689;Float;True;Property;_Texture0;Texture 0;3;0;Create;True;0;0;False;0;None;d21f0a1bb58b96b4693845b3d34bed06;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;50;-1106.248,810.1698;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;-5,-5;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;49;-788.7162,771.407;Float;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;None;d21f0a1bb58b96b4693845b3d34bed06;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;53;-780.4001,997.8431;Float;True;Property;_Dots;Dots;4;0;Create;True;0;0;False;0;None;f0c79717c4f6e8a4e8db65b61bce56e8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;43;-715.1051,551.5413;Float;True;Property;_Noise;Noise;6;0;Create;True;0;0;False;0;None;d21f0a1bb58b96b4693845b3d34bed06;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;42;-279.4448,425.429;Float;False;Property;_Emission;Emission;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0.7108089,5.656854,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCIf;47;-112.6148,677.1163;Float;True;6;0;FLOAT;0.6;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-373.339,-119.4936;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;0,0,0,0;0,0.3845286,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-128,0;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-41.54491,261.6294;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;40;384,-128;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Hologram Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.001;0,0.7098039,1,1;VertexOffset;False;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;18;0;16;0
WireConnection;18;1;19;0
WireConnection;46;0;18;0
WireConnection;48;0;54;0
WireConnection;48;1;46;0
WireConnection;50;0;55;0
WireConnection;50;1;46;0
WireConnection;49;0;51;0
WireConnection;49;1;50;0
WireConnection;43;0;51;0
WireConnection;43;1;48;0
WireConnection;47;0;43;1
WireConnection;47;1;49;1
WireConnection;47;2;53;1
WireConnection;47;3;53;1
WireConnection;10;0;7;0
WireConnection;10;1;47;0
WireConnection;41;0;42;0
WireConnection;41;1;47;0
WireConnection;40;0;10;0
WireConnection;40;2;41;0
WireConnection;40;9;47;0
ASEEND*/
//CHKSM=D17CCA8D7064B076DFEF9052D0E52ACFB10CAE96