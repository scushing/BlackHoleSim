// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader"Unlit/BlackHole"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float _GravityScale;
            float _StepSize;
            float _MaxSteps;

            const float _MaxFloat = 3.402823466e+38;
            const float _GravitationalConstant = 6.6743e-11;
            const float _SpeedOfLight = 299792458;

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
            float3 RayMarch(float3 rayOrigin, float3 rayDirection)
            {
                float3 currentPos = rayOrigin;
                float3 currentDir = rayDirection;
                float deltaT = _StepSize / _SpeedOfLight;
                // Perform ray marching loop
                for (int i = 0; i < _MaxSteps; i++)
                {
                    // Update ray position
                    currentPos = currentPos + currentDir * _StepSize;
                    // Check for collision with event horizon
                    float2 eventHorizonCollision = raySphereIntersection(float3(0, 0, 0), _SchwarzschildRadius, currentPos, currentDir);
                    if (eventHorizonCollision.x < 0)
                    {
                        return float3(0, 0, 0);
                    }
                    // Check within effect range
                    float2 effectRadiusCollision = raySphereIntersection(float3(0, 0, 0), _EffectRange, currentPos, currentDir);
                    if (effectRadiusCollision.y < 0)
                    {
                        return currentDir;
                    }
                    // Get forces and update direction
                    float accelerationMagnitude = _GravitationalConstant * _GravityScale / pow(length(currentPos), 2);
                    float3 acceleration = -normalize(currentPos) * accelerationMagnitude;
                    currentDir = normalize(currentDir + acceleration * deltaT);
                }
                return currentDir;
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

                // Check for collision with effect radius
                float2 effectCollision = raySphereIntersection(float3(0, 0, 0), _EffectRange, rayOrigin, rayDirection);
                float3 finalDir;
                if (effectCollision.x = _MaxFloat)
                {
                    finalDir = rayDirection;
                }
                else
                {
                    // Step ray to edge of effect radius
                    rayOrigin += rayDirection * effectCollision.x;
                    // Perform ray marching and get final distance
                    finalDir = RayMarch(rayOrigin, rayDirection);
                }
                // Determine distortion based on final ray direction
                if (finalDir.x == 0 && finalDir.y == 0 && finalDir.z == 0)
                {
                    output.color = float4(0, 0, 0, 0);
                }
                float4 rayToCamerSpace = mul(unity_WorldToCamera, float4(finalDir, 0));
                float4 rayUVProjection = mul(unity_CameraProjection, float4(rayToCamerSpace));
                float2 finalScreen = float2(rayUVProjection.x / 2 + 0.5, rayUVProjection.y / 2 + 0.5);
                
                output.color = tex2D(_MainTex, finalScreen);
    
                return output;
            }
            ENDCG
        }
    }
}

