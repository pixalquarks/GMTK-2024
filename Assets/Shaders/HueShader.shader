Shader "Unlit/HueShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _HueChange("Hue Change", Range(0, 360)) = 0

        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True" }

        Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

        ColorMask [_ColorMask]
        LOD 100

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _HueChange;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            fixed3 RGBtoHSL(fixed3 rgb) {
                fixed r = rgb.r;
                fixed g = rgb.g;
                fixed b = rgb.b;

                fixed maxi = max(r, max(g, b));
                fixed mini = min(r, min(g, b));

                fixed h, s, l = (maxi + mini) * 0.5;

                if (maxi == mini) {
                    h = s = 0.0; // achromatic
                } else {
                    fixed d = maxi - mini;
                    s = l > 0.5 ? d / (2.0 - maxi - mini) : d / (maxi + mini);
                    if (maxi == r)
                        h = (g - b) / d + (g < b ? 6.0 : 0.0);
                    else if (maxi == g)
                        h = (b - r) / d + 2.0;
                    else if (maxi == b)
                        h = (r - g) / d + 4.0;
                    h /= 6.0;
                }

                return fixed3(h, s, l);
            }

            fixed hue2rgb(fixed p, fixed q, fixed t) {
                if (t < 0.0) t += 1.0;
                if (t > 1.0) t -= 1.0;
                if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
                if (t < 1.0 / 2.0) return q;
                if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
                return p;
            }

            fixed3 HSLtoRGB(fixed3 hsl) {
                fixed h = hsl.x;
                fixed s = hsl.y;
                fixed l = hsl.z;

                fixed r, g, b;

                if (s == 0.0) {
                    r = g = b = l; // achromatic
                } else {
                    fixed q = l < 0.5 ? l * (1.0 + s) : l + s - l * s;
                    fixed p = 2.0 * l - q;
                    r = hue2rgb(p, q, h + 1.0 / 3.0);
                    g = hue2rgb(p, q, h);
                    b = hue2rgb(p, q, h - 1.0 / 3.0);
                }

                return fixed3(r, g, b);
            }

            fixed4 ChangeHue(fixed4 color, fixed hueChange) {
                fixed3 hsl = RGBtoHSL(color.rgb);
                hsl.x += hueChange / 360.0; // normalize the hueChange to range [0, 1]
                hsl.x = frac(hsl.x); // wrap around if the value goes beyond 1
                fixed3 rgb = HSLtoRGB(hsl);
                return fixed4(rgb, color.a);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return ChangeHue(col, _HueChange);
            }

            ENDCG
        }
    }
}
