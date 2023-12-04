// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader"Unlit/BlackHole"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GravityScale ("Gravity Scale", Float) = 1.0
        _SchwarzschildRadius ("Schwarzschild Radius", Float) = 1.0
        _StepSize ("Step Size", Float) = 0.01
        _MaxSteps ("Max Steps", Integer) = 1000
    }

    SubShader
    {
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "./Utils.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _EffectRange;
            float _SchwarzschildRadius;
            float _StepSize;
            float _MaxSteps;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_Position;
            };

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
                    float distance = /* Distance estimation function */ 1.0f;
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
            v2f vert(appdata input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            // Fragment shader function
            FragmentOutput frag(v2f input)
            {
                FragmentOutput output;
    
                // Calculate ray direction based on screen space coordinates
                float3 rayOrigin = _WorldSpaceCameraPos.xyz;
                float3 rayDirection = normalize(_WorldSpaceCameraPos.xyz - input.vertex.xyz);

                // Perform ray marching and get final distance
                float finalDistance = RayMarch(rayOrigin, rayDirection);

                // TODO
                // Perform coloring or apply distortion effects based on distance or other parameters

                output.color = float4(1, 1, 1, 1);
    
                return output;
            }
            ENDCG
        }
    }
}

