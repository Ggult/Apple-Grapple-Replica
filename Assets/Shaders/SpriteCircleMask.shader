Shader "AppleGrapple/SpriteCircleMask"
{
    // Clips a rectangular sprite (e.g. a flag PNG) into a circle via UV distance, with a soft anti-aliased edge.
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0, 0.5)) = 0.5
        _Softness ("Edge Softness", Range(0.001, 0.2)) = 0.02
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; // .z = width, .w = height (Unity auto-fills this)
            fixed4 _Color;
            fixed _Radius;
            fixed _Softness;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

                // Correct for non-square textures: equal UV deltas aren't equal physical distances on a rectangle.
                float aspect = _MainTex_TexelSize.z / _MainTex_TexelSize.w;
                float2 centered = IN.texcoord - 0.5;
                centered.x *= aspect;

                float dist = length(centered);
                float mask = 1 - smoothstep(_Radius - _Softness, _Radius, dist);
                c.a *= mask;

                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
