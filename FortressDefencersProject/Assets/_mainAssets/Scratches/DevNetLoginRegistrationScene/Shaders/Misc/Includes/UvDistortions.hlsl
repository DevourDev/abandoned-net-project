#define const float sigma2 = 2. * sigma * sigma;
#define const float pisigma2 = pi * sigma2;

void DistortUv_float(float2 ScreenPosition, float2 OffsetMax, float SamplesCount, out float3 BlurredRgb)
{
	int samples = (int)SamplesCount; // 8
	float2 offsetDelta = OffsetMax / samples; // (0, 0.1) / 8 = (0, 0.0125)

	float3 offsettedColor; // (0, 0, 0)

	for (int i = 1; i < samples; i++) // 8 итераций
	{
		float2 offsettedUv = ScreenPosition + offsetDelta * i;
		offsettedColor += SHADERGRAPH_SAMPLE_SCENE_COLOR(offsettedUv);
	}

	BlurredRgb = offsettedColor / samples;
}

void ColorBlur_float(float2 screenPosition, float offsetMax, float samplesCount, float directionsCount, out float3 blurredRgb) {
	int samples = (int)samplesCount;
	int directions = (int)directionsCount;
	float deltaOffsetLength = offsetMax / samples;
	float deltaAngle = (float)360 / directions;

	float3 offsettedColor;

	for (int currentDirection = 0; currentDirection < directions; ++currentDirection)
	{
		float currentAngle = deltaAngle * currentDirection;
		currentAngle = PI * currentAngle / 180.0;
		float2 deltaOffset = float2(deltaOffsetLength * cos(currentAngle), deltaOffsetLength * sin(currentAngle));

		for (int currentSample = 1; currentSample < samples; ++currentSample)
		{
			float2 offsettedUv = screenPosition + deltaOffset * currentSample;
			offsettedColor += SHADERGRAPH_SAMPLE_SCENE_COLOR(offsettedUv);
		}

	}

	blurredRgb = offsettedColor / (samples * directions);

}



void GaussianBlur_float(float2 screenPosition, float strength, float quality, out float3 blurredColor) {

	//int directions = (int)quality;
	int directions = (int)8;
	//int samples = (int)quality; 
	int samples = (int)8;


	float deltaAngle = (float)360 / directions; // 360 / 8 = 45
	//float offsetLength = (float)strength / samples;
	float offsetLength = (float)0.07 / samples;

	float currentAngle;
	float3 offsettedColor;
	for (int currentDirection = 0; currentDirection < directions; ++currentDirection)
	{
		currentAngle = deltaAngle * currentDirection; // 0

		for (int currentSample = 0; currentSample < samples; ++currentSample) // 8 итераций 
		{

			float2 offsettedScreenPositionCoord = float2(offsetLength * currentSample * cos(currentAngle), (offsetLength * sin(currentAngle)));
			offsettedColor += SHADERGRAPH_SAMPLE_SCENE_COLOR(offsettedScreenPositionCoord);
		}
	}

	blurredColor = offsettedColor / (samples * directions);

}
