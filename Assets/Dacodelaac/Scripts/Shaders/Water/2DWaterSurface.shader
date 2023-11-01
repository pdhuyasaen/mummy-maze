Shader "Dacodelaac/2DWaterSurface"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _Deep ("Deep", Float) = 1

        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _Alpha ("Alpha", Range(0,1)) = 1
        _Glow ("Glow", Range(0,1)) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"
        }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half4 _Color1;
            half4 _Color2;

            fixed _Cutoff;
            fixed _Alpha;
            fixed _Deep;
            fixed _Glow;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed a = tex2D(_MainTex, i.texcoord).a;
                fixed b = a;

                if (a - _Cutoff < 0) a = 0;
                b = saturate(b - a);

                fixed4 col = lerp(_Color1, _Color2, a * _Deep);
                col.a = a > 0 ? _Alpha : b * _Glow;

                return col + b * _Glow;
            }
            ENDCG
        }
    }
}