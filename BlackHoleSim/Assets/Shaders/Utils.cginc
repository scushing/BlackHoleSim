static const float maxFloat = 3.402823466e+38;

// Returns 2D vector with first field being distance to sphere, and second being distance through the sphere
float2 raySphereIntersection(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDirection)
{
    // Vector from sphere center to ray origin
    float3 sphereToRay = sphereCenter - rayOrigin;
    
    // Coefficients for quadratic equation
    float a = dot(rayDirection, rayDirection);
    float b = 2 * dot(rayDirection, sphereToRay);
    float c = dot(sphereToRay, sphereToRay) - (sphereRadius * sphereRadius);
    
    float discriminant = b * b - 4 * a * c;
    
    // No intersection; ray misses
    if (discriminant < 0)
    {
        return float2(maxFloat, 0);
    }
    // Single point of intersection
    else if (discriminant < 0.01)
    {
        return float2(-b / (2 * a), 0);
    }
    // Ray passes through sphere
    else
    {
        // Handle negative values after funtion returns
        float distToSphere = (-b - sqrt(discriminant)) / (2 * a);
        float distThroughSphere = (-b + sqrt(discriminant)) / (2 * a);
        return float2(distToSphere, distThroughSphere);
    }
}

float invLerp(float from, float to, float value)
{
    return (value - from) / (to - from);
}

float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
{
    float rel = invLerp(origFrom, origTo, value);
    return lerp(targetFrom, targetTo, rel);
}