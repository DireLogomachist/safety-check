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

            float3 RotateAroundYInDegrees (float3 vertex, float degrees) {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, vertex.xz), vertex.y).xzy;
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