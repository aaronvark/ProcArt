Shader "Hidden/simplePerlin"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale", Float) = 1
		_Offset ("Offset", Vector ) = (0,0,0,0)
		_Color ( "Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM

			#pragma multi_compile CNOISE PNOISE SNOISE SNOISE_AGRAD SNOISE_NGRAD
			#pragma multi_compile _ THREED
			#pragma multi_compile _ FRACTAL

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "ClassicNoise2D.hlsl"

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
			fixed4 _Color;

			float4 frag(v2f IN) : SV_Target
			{
				const float epsilon = 0.0001;

				float2 uv = IN.uv * _Scale + _Offset.xy;
				float o = 0.5;
				float s = 1.0;

				float w = 0.25;

				//#ifdef FRACTAL
				for (int i = 0; i < 6; i++)
				//#endif
				{
					float2 coord = uv * s;
					float2 period = s * 2.0;

					o += pnoise(coord, period) * w;

					s *= 2.0;
					w *= 0.5;
				}

				return float4(o * _Color.r, o * _Color.g, o * _Color.b, 1);
			}
			ENDCG
		}
	}
}
