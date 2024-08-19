Shader "Unlit/FlashShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _FlashScl("Flash Scale", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True" }
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

            fixed4 _Color; float _FlashScl;
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
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color * i.color;
                return fixed4(col.rgb, col.a * (0.5 + sin(_Time.w * _FlashScl) * 0.5));
            }

            ENDCG
        }
    }
}
