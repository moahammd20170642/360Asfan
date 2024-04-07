Shader "Vour/TeleportBlink"
{
    Properties
    {
        _Alpha("Alpha", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Overlay+100" "RenderType" = "Transparent" "IgnoreProjector" = "True" }

        ZTest Off
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float _Alpha;

            float4 frag(v2f i) : SV_Target
            {
                return float4(0, 0, 0, _Alpha);
            }
            ENDCG
        }
    }
}