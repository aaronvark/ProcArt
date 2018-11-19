Shader "Hidden/inverseCircleGradient"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale", Float) = 1
		_Offset ("Offset", Vector ) = (0,0,0,0)
		_Strength("Strength", Float) = 1
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Scale;
			float4 _Offset;
			float _Strength;
			fixed4 _Color;

			float4 frag(v2f IN) : SV_Target
			{
				_Scale = max( _Scale, 0 );
				float r = distance(IN.uv + _Offset.xy, float2(.5,.5)) * ( 1.0 / _Scale );
				r = clamp( 1 - r, 0, 1 );
				return float4(_Color.r * r, _Color.g * r, _Color.b * r, 1);
			}
			ENDCG
		}
	}
}
