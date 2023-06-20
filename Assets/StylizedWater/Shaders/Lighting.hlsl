#include "UnityCG.cginc"

float3 ApplyAdditionalLights(in float3 color, in float smoothness, in float3 posWorld, in float3 viewDir, in float3 normalDir) {
#ifdef _LIGHTING_ON
	float3 vertexLighting = float3(0.0, 0.0, 0.0);

	//Light color
	vertexLighting = Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb,
		unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, posWorld, normalDir);

	return vertexLighting + color;	
#else
	return color;
#endif
}