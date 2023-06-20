Shader "Unlit/HealthbarShader"
{
    Properties
    {
//        _HealthFullColor ("Health Full Color", Color) = (0.5098039, 0.9137255, 0.2980392,1)
        _HealthLowColor ("Health Low Color", Color) = (0.8962264, 0.2254491, 0.1648718,1)
        _BorderColor ("Border Color", Color) = (0, 0, 0, 1)
        _HealthInnerBorderColor ("Health Inner Border Color", Color) = (1, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", Range(0, 10)) = 1
        _BorderRadius ("Border Radius", Range(0, 0.8)) = 1
        _HealthInnerBorderSize ("Health Inner Border Size", Range(0, 0.1)) = 0.05
        _FlashStart ("Flash Start", Range(0, 1)) = 0.25
        _Health ("Health", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Ztest always
        LOD 100

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float _Health;
            float _Radius;
            float _BorderRadius;
            float _HealthInnerBorderSize;
            float _FlashStart;
            // float4 _HealthFullColor;
            float4 _HealthLowColor;
            float4 _BackgroundColor;
            float4 _BorderColor;
            float4 _HealthInnerBorderColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 coords = i.uv;
                coords.x *= 8;
                float2 pointOnLineSegment = float2(clamp(coords.x, 0.5, 7.5), 0.5);
                float sdf = distance(coords, pointOnLineSegment) * _Radius - 1;

                clip(-sdf);
                
                float borderSdf = sdf + _BorderRadius;
                float borderMask = saturate(1 - saturate(borderSdf / fwidth(borderSdf)));
                
                float fillAreaMask = _Health > i.uv;
                float healthInnerBorderSize = lerp(_HealthInnerBorderSize, 0, _Health);
                float xDistance = distance(_Health, i.uv.x);
                float healthBorderMask = (1 - step(healthInnerBorderSize, xDistance)) * fillAreaMask * (_Health < 0.99 && _Health > 0.01);
                healthBorderMask *= borderMask;
                fillAreaMask *= borderMask;

                float3 borderColor = _BorderColor * (1 - borderMask);
                float3 healthBorderColor = _HealthInnerBorderColor * healthBorderMask;
                
                float3 bgColor = _BackgroundColor * ((1 - fillAreaMask) * borderMask);
                // float3 healthBarColor = lerp(_HealthLowColor, _HealthFullColor, _Health) * mask;
                float3 healthBarColor = (lerp(_HealthLowColor, i.color, _Health) * -(sdf + 0.2)) * fillAreaMask;
                
                if (_Health < _FlashStart) healthBarColor *= cos(_Time.y * 4) * 0.4 + 1;

                float3 innerColor = healthBarColor;
                innerColor += bgColor;
                innerColor += borderColor;
                innerColor *= 1 - healthBorderMask;
                innerColor += healthBorderColor;
                
                return float4(innerColor, 1);
            }
            ENDCG
        }
    }
}
