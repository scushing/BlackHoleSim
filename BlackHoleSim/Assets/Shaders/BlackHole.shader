Shader"Unlit/BlackHole"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EffectRange ("Effect Range", float) = 5
        _MarchRange ("March Range", float) = 10
        _SchwarzschildRadius ("Schwarzschild Radius", float) = 1
        _GravityScale ("Gravity Scale", float) = 1
        _StepSize ("Step Size", float) = 0.03
        _MaxSteps ("Max Steps", int) = 500
    }

    SubShader
    {
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "./Utils.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _EffectRange;
            float _MarchRange;
            float _SchwarzschildRadius;
            float _GravityScale;
            float _StepSize;
            float _MaxSteps;

            float3 _Position = (0.1, 0.1, 0.1);

            const float _MaxFloat = 3.402823466e+38;
            const float _GravitationalConstant = 6.6743e-11;
            const float _SpeedOfLight = 299792458;

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
                
                float2 uv = i.uv * 2 - 1; // Re-maps the uv so that it is centered on the screen
                float3 viewVector = mul(unity_CameraInvProjection, float4(uv.x, uv.y, 0, -1));
                o.viewDir = mul(unity_CameraToWorld, float4(viewVector, 0));
                
                // Texture coords
                o.uv = i.uv;

                return o;
            }

            // Ray marching function
            float3 rayMarch(float3 center, float3 rayOrigin, float3 rayDirection)
            {
                float3 currentPos = rayOrigin;
                float3 currentDir = rayDirection;
                //float deltaT = _StepSize / _SpeedOfLight;
                // Perform ray marching loop
                for (int i = 0; i < _MaxSteps; i++)
                {
                    // Update ray position
                    currentPos = currentPos + currentDir * _StepSize;
                    // Check for collision with event horizon
                    float2 eventHorizonCollision = raySphereIntersection(center, _SchwarzschildRadius, currentPos, currentDir);
                    if (eventHorizonCollision.x < 0)
                    {
                        // Entered event horizon; returns zero vector to be caught outside function
                        return float3(0, 0, 0);
                    }
                    // Check within effect range
                    float2 effectRadiusCollision = raySphereIntersection(center, _MarchRange, currentPos, currentDir);
                    if (effectRadiusCollision.y < 0)
                    {
                        // Ray left effect range. Return ray
                        return currentPos;
                    }
                    // Get forces and update direction
                    float dist = length(currentPos);
                    float accelerationMagnitude = _GravitationalConstant * _GravityScale / (dist * dist);
                    float3 acceleration = normalize(center - currentPos) * accelerationMagnitude;
                    currentDir = normalize(currentDir + acceleration * _StepSize);
                }
                return currentPos;
            }

            // Fragment shader function
            fixed4 frag(v2f i) : SV_Target
            {
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDirection = normalize(i.viewDir);
    
                float2 intersection = raySphereIntersection(_Position, _EffectRange, rayOrigin, rayDirection);
    
                // Ray unaffected
                if (intersection.x > _MaxFloat - 1)
                {
                    // Pixel color unchanged
                    return tex2D(_MainTex, i.uv);
                }
                else
                {
                    // Step forward to effect range
                    float3 entryPoint = rayOrigin + rayDirection * intersection.x;
                    // Iteratively march ray calculate path near black hole 
                    float3 finalPos = rayMarch(_Position, entryPoint, rayDirection);
                    // Smaller than normalized vector, ie. zero-vector case
                    if (length(finalPos) < 0.1)
                    {
                        // Return black in case where ray enters event horizon
                        return float4(0, 0, 0, 0);
                    }
                    // If hit, calculate distortion
                    float3 finalDir = normalize(finalPos - rayOrigin);
        
                    float4 finalDirClipSpace = mul(unity_WorldToCamera, float4(finalDir, 0));
                    float4 uvProjection = mul(unity_CameraProjection, finalDirClipSpace);
                    float2 distortedUv = float2(uvProjection.x / 2 + 0.5, uvProjection.y / 2 + 0.5);
                    
        float blend = 0;
        if (length(finalPos - _Position))
        {
            blend
        }
        
                    return tex2D(_MainTex, distortedUv);
                }
    
                 
            }
            ENDCG
        }
    }
}

