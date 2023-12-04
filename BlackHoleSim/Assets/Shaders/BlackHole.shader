Shader "Custom/BlackHole"

{
    Properties
    {
        _GravityScale ("Gravity Scale", Float) = 1.0
        _SchwarzschildRadius ("Schwarzschild Radius", Float) = 1.0
        _StepSize ("Step Size", Float) = 0.01
        _MaxSteps ("Max Steps", Integer) = 1000
    }

    SubShader
    {
        Pass {

            float _EffectRange;
            float _SchwarzschildRadius;
            float _StepSize;
            float _MaxSteps;

            #include "UnityCG.cginc"
            #include "./Utils.cginc"
            #pragma vertex vert
            #pragma fragment frag

            struct FragmentOutput
            {
                float4 color : SV_Target;
            };

            // Ray marching function
            float RayMarch(float3 rayOrigin, float3 rayDirection)
            {
                float totalDistance = 0.0;
                float maxDistance = 100.0; // Maximum distance to trace
    
                // Alter this
                // Perform ray marching loop
                for (int i = 0; i < 100; i++)
                {
                    float distance = /* Distance estimation function */;
                    totalDistance += distance;

                    if (totalDistance > maxDistance)
                        break;
        
                    // Update ray position
                    float3 currentPos = rayOrigin + rayDirection * totalDistance;

                    // TODO
                    // Check collision or gravitational effects (e.g., distortion)
        
                    // TODO
                    // Modify ray direction or apply gravitational effects
                }

                return totalDistance;
            }

            // Vertex shader function
            VertexOutput vert(VertexInput input)
            {
                VertexOutput output;
                output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
                return output;
            }

            // Fragment shader function
            FragmentOutput frag(VertexOutput input)
            {
                FragmentOutput output;
    
                // Calculate ray direction based on screen space coordinates
                float3 rayOrigin = _WorldSpaceCameraPos.xyz;
                float3 rayDirection = normalize(_WorldSpaceCameraPos.xyz - input.pos.xyz);

                // Perform ray marching and get final distance
                float finalDistance = RayMarch(rayOrigin, rayDirection);

                // TODO
                // Perform coloring or apply distortion effects based on distance or other parameters

                output.color = /* Final color */;
    
                return output;
            }
        }
    }
}

