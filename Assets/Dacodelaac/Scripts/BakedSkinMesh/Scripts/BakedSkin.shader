Shader "Screw/BakedSkin"{
    Properties {
        _Color ("Color Tint", Color) = (1.0,1.0,1.0,1.0)
        _AmbientColor ("Ambient Color", Color) = (1,1,1,1.0)
        _MainTex ("Diffuse Texture", 2D) = "white" {}
        _DiffuseStrength ("Diffuse Factor", Range(0, 1)) = 0.5
        _AmbientStrength ("Ambient Factor", Range(0, 1)) = 0.5

        _AnimTex ("Texture", 2D) = "white" {}
        _Magnitude ("Magnitude", float) = 1        
    }
    SubShader {
        Pass {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers flash
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"
            
            //user defined variables
            uniform sampler2D _MainTex;
            uniform half4 _MainTex_ST;
            uniform half4 _Color;
            uniform half _DiffuseStrength;
            uniform half4 _AmbientColor;
            uniform half _AmbientStrength;
            
			
            sampler2D _AnimTex;
            float4 _AnimTex_ST;			
            float _Magnitude;

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float, _OffsetY)				
			UNITY_INSTANCING_BUFFER_END(Props)
           
            //base input structs
            struct vertexInput{
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half4 texcoord : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct vertexOutput{
                half4 pos : SV_POSITION;
                half4 tex : TEXCOORD0;
                
                half4 diffuse : COLOR;
            };
            
            //vertex Function
            
            vertexOutput vert(vertexInput v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				float offsetY = UNITY_ACCESS_INSTANCED_PROP(Props, _OffsetY);				

                float4 anim = tex2Dlod(_AnimTex, float4(v.uv1.x, v.uv1.y + offsetY, 0, 0));
                float4 shift = (anim - 0.5) * _Magnitude;
                shift.a = v.vertex.w;
                v.vertex = shift;

                vertexOutput o;
                
                half3 normalDir = normalize( mul( half4( v.normal, 0.0 ), unity_WorldToObject ).xyz );
                o.pos = UnityObjectToClipPos(v.vertex);
                o.tex = v.texcoord;

                half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

                half nl = saturate(dot(normalDir, lightDirection));
                o.diffuse = _LightColor0 * nl * _DiffuseStrength;

                return o;
            }
            
            half4 frag(vertexOutput i) : COLOR
            {
                half4 ambient = _AmbientColor * _AmbientStrength;
				half4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				half4 light = i.diffuse + ambient;
                half4 color = tex * _Color * light;
                
                return color;

            }
            
            ENDCG
            
        }
    }
    //Fallback "Specular"
}



// Shader "Unlit/BakedSkin"
// {
//     Properties
//     {
//         _MainTex ("Texture", 2D) = "white" {}
//         _AnimTex ("Texture", 2D) = "white" {}
//         _Magnitude ("Magnitude", float) = 1
//         _OffsetY ("Offset Y", Range(0, 0.1)) = 0
//     }
//     SubShader
//     {
//         Tags { "RenderType"="Opaque" }
//         LOD 100

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             // make fog work
//             #pragma multi_compile_fog

//             #include "UnityCG.cginc"

//             struct appdata
//             {
//                 float4 vertex : POSITION;
//                 float2 uv : TEXCOORD0;
//                 float2 uv1 : TEXCOORD1;
//             };

//             struct v2f
//             {
//                 float2 uv : TEXCOORD0;
//                 UNITY_FOG_COORDS(1)
//                 float4 vertex : SV_POSITION;
//             };

//             sampler2D _MainTex;
//             float4 _MainTex_ST;
//             sampler2D _AnimTex;
//             float4 _AnimTex_ST;
//             float _OffsetY;

//             float _Magnitude;

//             v2f vert (appdata v)
//             {
//                 v2f o;

//                 float4 anim = tex2Dlod(_AnimTex, float4(v.uv1.x, v.uv1.y + _OffsetY, 0, 0));
//                 float4 shift = (anim - 0.5) * _Magnitude;
//                 shift.a = v.vertex.w;

//                 v.vertex = shift;

//                 o.vertex = UnityObjectToClipPos(v.vertex);
//                 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                 UNITY_TRANSFER_FOG(o,o.vertex);
//                 return o;
//             }

//             fixed4 frag (v2f i) : SV_Target
//             {
//                 // sample the texture
//                 fixed4 col = tex2D(_MainTex, i.uv);
//                 // apply fog
//                 UNITY_APPLY_FOG(i.fogCoord, col);
//                 return col;
//             }
//             ENDCG
//         }
//     }
// }
