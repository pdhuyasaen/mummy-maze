Shader "Dacodelaac/Unlit/Transparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)                       		

		[Header(Special)]
		[Enum(UnityEngine.Rendering.CullMode)] _Culling ("Cull", Int) = 2
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull [_Culling]

        Pass
        {                                    
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0            

            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;                
                UNITY_VERTEX_OUTPUT_STEREO       
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            half4 _Color;            

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);                
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {                
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;                
                return col;
            }
            ENDCG
        }        
    }
}
