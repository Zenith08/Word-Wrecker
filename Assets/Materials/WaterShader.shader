Shader "Custom/WaterShaderWithShadows"
{
    Properties
    {
        _Color ("Water Color", Color) = (0.0, 0.5, 1.0, 1.0)
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1.0
        _WaveStrength ("Wave Strength", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Your water shader logic here

            fixed4 c = _Color;
            c.a = 0.5; // Adjust alpha as needed
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        LOD 100

        Pass
        {
            Name "SHADOWCASTER"
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
            };

            uniform float _WaveSpeed;
            uniform float _WaveStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.pos.y += sin(_Time.y * _WaveSpeed + v.vertex.x) * _WaveStrength;
                return o;
            }
            ENDCG
        }

        Pass
        {
            Name "SHADOWS"
            Tags { "LightMode" = "ForwardAdd" }

            CGPROGRAM
            #pragma exclude_renderers gles xbox360 ps3

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 _Color;

            void surf(v2f i, inout SurfaceOutput o)
            {
                // Your water shader logic here for shadows

                fixed4 c = _Color;
                c.a = 0.5; // Adjust alpha as needed
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
        }
    }
}
