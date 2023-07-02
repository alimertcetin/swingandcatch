Shader "Hidden/NewImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.5
        _AnimationSpeed ("Animation Speed", float) = 50
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite ON ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float4 RotateAroundZ (float4 vertex, float degrees)
            {
                float radian = degrees * UNITY_PI / 180.0;
                float sin, cos;
                sincos(radian, sin, cos);
                float3x3 m = float3x3
                (
                    cos, -sin, 0,
                    sin, cos, 0,
                    0, 0, 1
                );
                
                return float4(mul(m, vertex.xyz), vertex.w);
            }
            
            sampler2D _MainTex;
            float _AlphaThreshold;
            float _AnimationSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                // TODO : Its not cenetered but anyway...
                v.vertex = RotateAroundZ(v.vertex, sin(_Time.z) * _AnimationSpeed).xyzz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float alpha = col.a >= _AlphaThreshold;
                alpha = 1 - alpha;
                clip((-alpha));

                return col;
            }
            ENDCG
        }
    }
}
