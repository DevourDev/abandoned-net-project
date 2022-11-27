float gaussian(float2 i) {
	int samples = 35;
	float sigma = float(samples) * .25;
	return exp(-0.5 * dot(i /= sigma, i)) / (6.28 * sigma * sigma);
}

void BlurGauss_float(float2 uv, float size, out float2 blurredUv) {
	float PI_TWO = 6.28318530718;

	blurredUv = uv;

	float directions = 16.0;
	float quality = 3.0;

	if (size == 0) {
		size = 8.0;
	}

	float2 radius = size / uv;

	for (float d = 0.0; d < PI_TWO; d += PI_TWO / directions)
	{
		for (float i = 1.0 / quality; i <= 1.0; i += 1.0 / quality)
		{
			float2 delta = float2(uv + float2(cos(d), sin(d)) * radius * i);
			blurredUv += delta;
		}
	}

	blurredUv /= quality * directions - 15.0;

	blurredUv = float2(gaussian(blurredUv), gaussian(blurredUv));

}
