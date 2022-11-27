Shader "Unlit/GaussianBlurShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "black" {}
		_BlurStrength("Blur Strength", Range(5, 90)) = 35
	}
		Subshader
	{
		Pass
		{
			Cull Off
			CGPROGRAM
			#pragma vertex VSMain
			#pragma fragment PSMain
			#pragma target 5.0

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			int _BlurStrength;

			float4 blur(sampler2D sp, float2 U, float2 scale, int samples, int LOD)
			{
				float4 O = (float4) 0;
				int sLOD = 1 << LOD;
				float sigma = float(samples) * 0.25;
				int s = samples / sLOD;
				for (int i = 0; i < s * s; i++)
				{
					float2 d = float2(i % (uint)s, i / (uint)s) * float(sLOD) - float(samples) / 2.;
					float2 t = d;
					O += exp(-0.5 * dot(t /= sigma,t)) / (6.28 * sigma * sigma) * tex2Dlod(sp, float4(U + scale * d, 0, LOD));
				}
				return O / O.a;
			}

			float4 VSMain(float4 vertex:POSITION, inout float2 uv : TEXCOORD0) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}

			float4 PSMain(float4 vertex:SV_POSITION, float2 uv : TEXCOORD0) : SV_Target
			{
				int samples = _BlurStrength, LOD = 2;
				return blur(_MainTex, uv, _MainTex_TexelSize.xy, samples, LOD);
			}
			ENDCG
		}
	}
}