﻿Shader "Hidden/mask"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SecondTex ("Texture", 2D) = "white" {}
		_Strength ("Strength", Float) = 1
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
			
			sampler2D _MainTex, _SecondTex;
			float _Strength;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col1 = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_SecondTex, i.uv);

				if ( col2.r > _Strength ) {
					return col1;
				}
				else {
					return 0;
				}
			}
			ENDCG
		}
	}
}
