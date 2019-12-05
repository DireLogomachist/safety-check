Shader "Custom/BackgroundSkybox" {

    Properties {
        _Tint ("Primary Color", Color) = (.5, .5, .5, .5)
        _Tint2 ("Secondary Color", Color) = (.3, .3, .3, .3)
        _Scale ("Scale", Range(0, 10000)) = 10000
        _Rotation ("Rotation", Range(0, 360)) = 0
        [NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
    }

    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            half4 _Tint;
            half4 _Tint2;
            float _Scale;
            float _Rotation;

            const float isqrt2 = 0.70710676908493042;

            float3 RotateAroundYInDegrees (float3 vertex, float degrees) {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, vertex.xz), vertex.y).xzy;
            }

            float3 cubify (float3 s) {
                float xx2 = s.x * s.x * 2.0;
                float yy2 = s.y * s.y * 2.0;

                float vx = xx2 - yy2;
                float vy = yy2 - xx2;

                float ii = vy - 3.0;
                ii *= ii;

                float isqrt = -sqrt(ii - 12.0 * xx2) + 3.0;

                vx = -sqrt(vx + isqrt);
                vy = -sqrt(vy + isqrt);
                vx *= isqrt2;
                vy *= isqrt2;
                
                return sign(s) * float3(vx, vy, 1.0);
            }

            float3 sphere2cube(float3 sphere) {
                float3 f = abs(sphere);

                bool a = f.y >= f.x && f.y >= f.z;
                bool b = f.x >= f.z;

                return a ? cubify(sphere.xzy).xzy : b ? cubify(sphere.yzx).zxy : cubify(sphere);
            }

            struct appdata {
                float4 vertex  : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 position : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
                o.position = UnityObjectToClipPos(rotated);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 adjustedWorldPos =  floor(i.worldPos.x/(_Scale)) + floor(i.worldPos.y/_Scale) + floor(i.worldPos.z*0);
                float chessboard = adjustedWorldPos.x + adjustedWorldPos.y + adjustedWorldPos.z;
                chessboard = frac(chessboard * 0.5);
                chessboard *= 2;

                half3 c = lerp(_Tint, _Tint2, chessboard);
                return half4(c, 1);
            }
            ENDCG
        }
    }

    Fallback "Standard"
}