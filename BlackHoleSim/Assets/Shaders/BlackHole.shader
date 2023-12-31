Shader"Unlit/BlackHole"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SchwarzschildRadius ("Schwarzschild Radius", float) = 1
        _GravitationalConstant ("Gravitational Const", float) = 0.000000000066743
        _StepSize ("Step Size", float) = 0.1
        _MaxSteps ("Max Steps", int) = 1000
        _PositionX ("Position X", float) = 0
        _PositionY ("Position Y", float) = 0
        _PositionZ ("Position Z", float) = 0
    }

    SubShader
    {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "./Utils.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // Mutable to more easily make more visible
            float _GravitationalConstant;
            float _SchwarzschildRadius;
            float _EffectRange;
            float _BlurRange;

            float _StepSize;
            float _MaxSteps;

            float _PositionX;
            float _PositionY;
            float _PositionZ;

            const float _MaxFloat = 3.402823466e+38;
            const float _SpeedOfLight = 299792458;

            const float _Epsilon = 0.001;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            // Vertex shader function
            v2f vert(appdata i)
            {
                v2f o; 
                // Clip position
                o.pos = UnityObjectToClipPos(i.vertex);
                // World pos
                o.worldPos = mul(unity_ObjectToWorld, i.vertex);
                
                // Re-scales and centers (uv ranges 0 to 1 and screen ranges -1 to 1)
                float2 uv = i.uv * 2 - 1;
                float3 viewVector = mul(unity_CameraInvProjection, float4(uv.x, uv.y, 0, -1));
                o.viewDir = mul(unity_CameraToWorld, float4(viewVector, 0));
                
                // Texture coords
                o.uv = i.uv;

                return o;
            }

            // Ray marching function
            // Iteratively euler updates ray until it reaches the center, leaves the effect range, or takes max steps
            float3 rayMarch(float3 center, float3 rayOrigin, float3 rayDirection)
            {
                float3 currentPos = rayOrigin;
                float3 currentDir = rayDirection;
                // Perform ray marching loop
                for (int i = 0; i < _MaxSteps; i++)
                {
                    // Update ray position
                    currentPos = currentPos + currentDir * _StepSize;
                    // Check within effect range
                    float2 effectRadiusCollision = raySphereIntersection(center, _EffectRange, currentPos, currentDir);
                    if (effectRadiusCollision.x > 0)
                    {
                        // Ray left effect range. Return ray
                        return currentPos;
                    }
                    // Get forces and update direction
                    float dist = length(currentPos - center);
                    // Using Schwarzchild Radius instead of mass is to prevent floating point errors
                    // Has same effect when scaling because they scale to each other linearly (radius = 2 * G * Mass / dist^2)
                    // Gravitational Constant is altered to accomodate this.
                    float accelerationMagnitude = _GravitationalConstant * _SchwarzschildRadius / (dist * dist);
                    float3 acceleration = normalize(center - currentPos) * accelerationMagnitude;
                    currentDir = normalize(currentDir + acceleration * _StepSize);
                }
                return currentPos;
            }

            // Fragment shader function
            fixed4 frag(v2f i) : SV_Target
            {
                _EffectRange = 8 * _SchwarzschildRadius;
                _BlurRange = _SchwarzschildRadius;
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDirection = normalize(i.viewDir);
    
                float3 center = float3(_PositionX, _PositionY, _PositionZ);
    
                // Here intersection.x is distance to enter sphere, intersection.y is distance to exit
                float2 intersection = raySphereIntersection(center, _EffectRange, rayOrigin, rayDirection);
    
                // Ray unaffected (never contacts sphere)
                if (intersection.x > _MaxFloat - _Epsilon)
                {
                    // Pixel color unchanged
                    return tex2D(_MainTex, i.uv);
                }
                else
                {
                    // Step forward to effect range
                    float3 entryPoint = rayOrigin + rayDirection * -intersection.y;
                    // March ray calculate path near black hole 
                    float3 finalPos = rayMarch(center, entryPoint, rayDirection);
                    // Ray ended up within single step of center, meaning it has failed to escape
                    if (length(finalPos - center) < _StepSize)
                    {
                        // Return black in case where ray is trapped
                        return float4(0, 0, 0, 0);
                    }
                    float3 finalDir = (finalPos - rayOrigin);
                    // Convert the final direction to camera uv
                    float4 clipPos = mul(unity_CameraProjection, mul(unity_WorldToCamera, float4(finalPos, 1.0)));
                    clipPos /= clipPos.w;
                    // Reverse of conversion done in vert function
                    float2 distortedUv = float2((clipPos.x + 1.0) * 0.5, (clipPos.y + 1.0) * 0.5);
        
                    // If close to edge, blur (otherwise edge of effect range can have visible contrast)
                    // Chosen values are somewhat arbitrary here. Decided these values look good and scale well.
                    float blurIntensity = intersection.y - length(center - rayOrigin);
                    float2 uv;
                    if (abs(blurIntensity) < _BlurRange)
                    {
                        uv = lerp(distortedUv, i.uv, abs(blurIntensity) / _BlurRange);
                    }
                    else
                    {
                        uv = float2(1.0 - distortedUv.x, 1.0 - distortedUv.y);
                    }
                    
                    return tex2D(_MainTex, uv);
                }
                 
            }
            ENDCG
        }
    }
}

