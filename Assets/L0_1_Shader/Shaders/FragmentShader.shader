Shader "FragmentShader"
{
    Properties
    {
        _Amplitude("Wave Size", Range(0,1)) = 0.4
        _Frequency("Wave Freqency", Range(1, 8)) = 2
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
    }
        SubShader
    {
        CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        float _Amplitude;
        float _Frequency;
        float4 _BaseColor;

        ENDCG

        Pass
        {
            CGPROGRAM
            #define PI 3.1415926

            const float TWO_PI = 6.28318530718;
            const float vertices = 8.;
            const float startIndex = 8;// vertices;
            const float endIndex = 16;// vertices * 2.;
            sampler2D _MainTex;

            float hash(float2 p) {
                float h = dot(p,float2(127.1,311.7));
                return frac(sin(h) * 43758.5453123);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata data)
            {
                v2f i;
                i.vertex = UnityObjectToClipPos(data.vertex);
                i.uv = data.uv;
                return i;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                return float4(i.uv, 0, 1);
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
}
