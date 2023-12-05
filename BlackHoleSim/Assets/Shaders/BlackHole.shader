// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader"Unlit/BlackHole"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EffectRange ("Effect Range", float) = 5
        _SchwarzschildRadius ("Schwarzschild Radius", float) = 1
        _GravityScale ("Gravity Scale", float) = 1
        _StepSize ("Step Size", float) = 0.03
        _MaxSteps ("Max Steps", int) = 500
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
            const float _GravitationalConstant = 0.4; //6.6743e-11;
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
                float3 worldPos : TEXCOOR1;
                float3 viewVector : TEXCOORD2;
            };

            // Vertex shader function
            v2f vert(appdata input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;
                output.worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
    
                float2 uv = input.uv * 2 - 1;
                float3 viewVector = mul(unity_CameraInvProjection, float4(uv.x, uv.y, 0, -1));
                output.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));
    
                return output;
            }

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
                    float dist = length(currentPos);
                    float accelerationMagnitude = _GravitationalConstant * _GravityScale / (dist * dist);
                    float3 acceleration = -normalize(currentPos) * accelerationMagnitude;
                    currentDir = normalize(currentDir + acceleration * deltaT);
                }
                return currentDir;
            }

            // Fragment shader function
            fixed4 frag(v2f input) : SV_Target
            {
                float4 output;
    
                // Calculate ray direction based on screen space coordinates
                float3 rayOrigin = _WorldSpaceCameraPos.xyz;
                float3 rayDirection = normalize(_WorldSpaceCameraPos.xyz - input.vertex.xyz);

                // Check for collision with effect radius
                float2 effectCollision = raySphereIntersection(float3(0, 0, 0), _EffectRange, rayOrigin, rayDirection);
                float3 finalDir;
                if (effectCollision.x == _MaxFloat)
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
                if (length(finalDir) < 1)
                {
                    output = float4(0, 0, 0, 1);
                    return output;
                }
                float4 rayToCamerSpace = mul(unity_WorldToCamera, float4(finalDir, 0));
                float4 rayUVProjection = mul(unity_CameraProjection, float4(rayToCamerSpace));
                rayUVProjection = normalize(rayUVProjection);
                float2 finalScreen = float2(rayUVProjection.x / 2 + 0.5, rayUVProjection.y / 2 + 0.5);
                
                float3 preOutput = tex2D(_MainTex, finalScreen);
                output = float4(preOutput, 1);
    
                return output;
            }
            ENDCG
        }
    }
}

