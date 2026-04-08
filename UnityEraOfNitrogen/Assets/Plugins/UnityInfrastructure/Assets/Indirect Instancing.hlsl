#ifndef INDIRECT_INSTANCING_INCLUDED
#define INDIRECT_INSTANCING_INCLUDED

// Transform matrix buffer.
StructuredBuffer<float4x4> _InstanceTransformsBuffer;

void ApplyIndirectTransform_float(float3 objectPosition, float3 objectNormal, float instanceID, out float3 worldPosition, out float3 worldNormal)
{
#if SHADERGRAPH_PREVIEW
    // Default values for preview.
    worldPosition = objectPosition;
    worldNormal = objectNormal;
#else
    uint id = (uint)instanceID;
    // Get transform by Instance ID
    float4x4 transformMatrix = _InstanceTransformsBuffer[id];

    worldPosition = mul(transformMatrix, float4(objectPosition, 1.0)).xyz;
    worldNormal = mul((float3x3)transformMatrix, objectNormal);
#endif
}

void ApplyIndirectTransform_half(float3 objectPosition, float3 objectNormal, float instanceID, out float3 worldPosition, out float3 worldNormal)
{
#if SHADERGRAPH_PREVIEW
    worldPosition = objectPosition;
    worldNormal = objectNormal;
#else
    uint id = (uint)instanceID;
    float4x4 transformMatrix = _InstanceTransformsBuffer[id];

    worldPosition = mul(transformMatrix, float4(objectPosition, 1.0)).xyz;
    worldNormal = mul((float3x3)transformMatrix, objectNormal);
#endif
}

#endif