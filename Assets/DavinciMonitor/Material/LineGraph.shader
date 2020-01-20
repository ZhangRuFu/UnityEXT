Shader "Line Graph"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

		_Color("Color", Color) = (1, 1, 1, 0.2)
		_LineColor("LineColor", Color) = (1, 0, 0, 1)
		_60Color("60 Color", Color) = (0, 1, 0, 1)
		_30Color("30 Color", Color) = (0, 1, 1, 1)
		_0Color("0 Color", Color) = (1, 0, 0, 1)
	}

		SubShader
		{			
			Tags
			{ 
				"Queue"="Transparent" 
				"RenderType"="Transparent" 
			}

			Cull Off
			ZWrite Off
			ZTest Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				Name "LineGraph"
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex    : POSITION;
					float2 uv  : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 uv  : TEXCOORD0;
				};

				v2f vert(appdata IN)
				{
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.uv = IN.uv;

					return OUT;
				}

				sampler2D _MainTex;
				half4 _Color;
				half4 _LineColor;
				half4 _60Color;
				half4 _30Color;
				half4 _0Color;

				float Average;
				
				float FPSSample[256];
				float FPSSampleLength;
/*
				float drawLine (float2 p1, float2 p2, float2 uv, float a)
				{
					float r = 0.;
					float one_px = 1. / iResolution.x; //not really one px
					
					// get dist between points
					float d = distance(p1, p2);
					
					// get dist between current pixel and p1
					float duv = distance(p1, uv);

					//if point is on line, according to dist, it should match current uv 
					r = 1.-floor(1.-(a*one_px)+distance (mix(p1, p2, clamp(duv/d, 0., 1.)),  uv));
						
					return r;
				}
*/
				half4 frag(v2f IN) : SV_Target
				{
					float xCoord = IN.uv.x;
					float yCoord = IN.uv.y;
					half4 finalColor = _Color;

					float index = xCoord * FPSSampleLength;
					float fpsY = lerp(FPSSample[floor(index)], FPSSample[ceil(index)], frac(index)) / 60.0;
					if(fpsY >= yCoord)
						finalColor = lerp(_0Color, _60Color, yCoord);

						//standard line
					if(yCoord > 0.995 && yCoord <= 1.000)
						finalColor = _60Color;

					if(yCoord > 0.497 && yCoord < 0.503)
						finalColor = _30Color;

					if(yCoord > 0.000 && yCoord < 0.01)
						finalColor = _0Color;

					//return finalColor;
					return finalColor;
				}

				ENDCG
			}
		}
}