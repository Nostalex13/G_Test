Shader "Custom/Gradient"
{
	Properties
	{
		_UpperColor ("UpperColor", Color) = (0, 0, 0, 0)
		_LowerColor ("LowerColor", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			
			fixed4 _UpperColor;
			fixed4 _LowerColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 world	= mul(unity_ObjectToWorld, v.vertex);
				o.vertex		= mul(UNITY_MATRIX_VP, world);
                o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = lerp(_LowerColor, _UpperColor, i.uv.y);

				return col;
			}
			ENDCG
		}

	}
}
