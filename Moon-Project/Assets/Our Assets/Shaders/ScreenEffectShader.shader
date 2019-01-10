// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Screen Shader"
{
	Properties
	{
		_Effect("Effect", 2D) = "white" {}
		_Completion("Completion", Range( 0 , 1)) = 0.6931282
		_Albedo("Albedo", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Fade("Fade", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _Completion;
		uniform sampler2D _Effect;
		uniform float4 _Effect_ST;
		uniform float _Fade;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 lerpResult32 = lerp( tex2D( _Albedo, uv_Albedo ) , float4( 1,1,1,0 ) , ( _Completion * 4.0 ));
			float2 uv_Effect = i.uv_texcoord * _Effect_ST.xy + _Effect_ST.zw;
			float ifLocalVar8 = 0;
			if( tex2D( _Effect, uv_Effect ).r >= ( _Completion * 1.1 ) )
				ifLocalVar8 = 1.0;
			else
				ifLocalVar8 = 0.0;
			float4 clampResult21 = clamp( ( lerpResult32 * ifLocalVar8 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 temp_output_27_0 = ( clampResult21 * _Fade );
			o.Albedo = temp_output_27_0.rgb;
			o.Emission = temp_output_27_0.rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
141;41;1611;940;1241.757;697.8069;1.550449;True;True
Node;AmplifyShaderEditor.RangedFloatNode;2;-792.4237,-53.40254;Float;False;Property;_Completion;Completion;1;0;Create;True;0;0;False;0;0.6931282;0.9414187;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-331.6433,230.9122;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-616.6554,225.5807;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;1.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-725.7826,-359.9886;Float;True;Property;_Effect;Effect;0;0;Create;True;0;0;False;0;d6b1788d4ac785e44ad25af6fe603cf7;d6b1788d4ac785e44ad25af6fe603cf7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-384,-384;Float;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;False;0;8f7449d4c263747dd86e28d868e560c4;59a4b0b21990dc841a8bcd285676883f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-905.4268,529.9911;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-895.327,262.291;Float;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;8;-384,-128;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;32;-58.76453,100.6747;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;0,-384;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;21;256,-384;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;25;256,-128;Float;False;Property;_Fade;Fade;4;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;407.9209,102.2249;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;512,-384;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;20;256,-64;Float;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;False;0;0;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;768,-384;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Screen Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;35;0;2;0
WireConnection;18;0;2;0
WireConnection;8;0;1;1
WireConnection;8;1;18;0
WireConnection;8;2;10;0
WireConnection;8;3;10;0
WireConnection;8;4;9;0
WireConnection;32;0;11;0
WireConnection;32;2;35;0
WireConnection;12;0;32;0
WireConnection;12;1;8;0
WireConnection;21;0;12;0
WireConnection;34;0;21;0
WireConnection;27;0;21;0
WireConnection;27;1;25;0
WireConnection;0;0;27;0
WireConnection;0;2;27;0
WireConnection;0;4;20;0
ASEEND*/
//CHKSM=8E0BBB85560D5D900F28B1D82C7B23E60469B36C