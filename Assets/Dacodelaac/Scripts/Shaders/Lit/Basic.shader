Shader "Dacodelaac/Lit/Basic"
{
    Properties
    {
        [Header(Base)][Space]
        _Color ("Color", Color) = (1,1,1,1)
        _HColor ("Highlight Color", Color) = (1,1,1,1.0)
        _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1.0)
        _MainTex ("Main Texture (RGB) Spec/MatCap Mask (A) ", 2D) = "white" {}

        [Header(Ramp)][Space]
        [Toggle(RAMP)] _UseRamp("Enable", Int) = 0
        _RampThreshold ("Ramp Threshold", Range(0,1)) = 0.3
        _RampSmooth ("Ramp Smoothing", Range(0.01,1)) = 0.01

        [Header(Emission)][Space]
        [Toggle(EMISSION)] _UseEmission("Enable", Int) = 0
        [HDR] _Emission ("Emission Color", Color) = (0,0,0,1)
        _EmissionRate ("Emission Rate", Float) = 0

        [Header(Normal Map)][Space]
        [Toggle(BUMP)] _UseBump("Enable", Int) = 0
        [NoScaleOffset] _BumpMap ("Normal map (RGB)", 2D) = "bump" {}

        [Header(Specular)][Space]
        [Toggle(SPEC)] _UseSpec("Enable", Int) = 0
        [Toggle(SPEC_TOON)] _UseSpecToon("Toon", Int) = 0
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Shininess", Range(0.01,2)) = 0.1
        _SpecSmooth ("Smoothness", Range(0,1)) = 0.05

        [Header(Rim)][Space]
        [Toggle(RIM)] _UseRim("Enable", Int) = 0
        _RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
        _RimMin ("Rim Min", Range(0,1)) = 0.5
        _RimMax ("Rim Max", Range(0,1)) = 1.0

        [Header(Relfection)][Space]
        [Toggle(MATCAP)] _UseMatCap("Enable", Int) = 0
        _MatCap ("MatCap (RGB)", 2D) = "black" {}
        
        [Header(VerticalFog)][Space]
        [Toggle(VERTICAL_FOG)] _UseVerticalFog("Enable", Int) = 0
        _FogColor("Fog Color", Color) = (0,0,0,0)
        _FogHeight("Fog Height", Float) = 0
        _FogFade("Fog Fade", Float) = 0

        [Enum(UnityEngine.Rendering.CullMode)] _Culling("Cull", Int) = 2
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200
        Cull [_Culling]

        CGPROGRAM
        #pragma surface surf CustomLighting nodirlightmap vertex:vert noforwardadd halfasview
        #pragma target 2.0
        #pragma shader_feature RAMP
        #pragma shader_feature EMISSION
        #pragma shader_feature RIM
        #pragma shader_feature BUMP
        #pragma shader_feature SPEC
        #pragma shader_feature SPEC_TOON
        #pragma shader_feature MATCAP
        #pragma shader_feature VERTICAL_FOG

        //================================================================
        // VARIABLES

        half4 _Color;
        half4 _HColor;
        half4 _SColor;
        sampler2D _MainTex;
        #if RAMP
        half _RampThreshold;
        half _RampSmooth;
        #endif
        #if EMISSION
        half4 _Emission;
        half _EmissionRate;
        #endif
        #if RIM
        half4 _RimColor;
        half _RimMin;
        half _RimMax;
        #endif
        #if BUMP
        sampler2D _BumpMap;
        #endif
        #if SPEC
        half _Shininess;
        half _SpecSmooth;
        #endif
        #if MATCAP
        sampler2D _MatCap;
        #endif
        #if VERTICAL_FOG
        half4 _FogColor;
        half _FogHeight;
        half _FogFade;
        #endif
        
        struct Input
        {
            half2 uv_MainTex : TEXCOORD0;
            #if BUMP
            half2 uv_BumpMap : TEXCOORD1;
            #endif
            #if RIM
            half rim;
            #endif
            #if MATCAP
            half2 matcap;
            #endif
            half3 worldPos;
        };

        struct SurfaceOutputCustom
        {
            half3 Albedo;
            half3 Normal;
            half3 Emission;
            half Specular;
            half Gloss;
            half Alpha;
            half3 WorldPos;
        };

        //================================================================
        // VERTEX FUNCTION

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            #if RIM
            half3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
            half rim = 1.0f - saturate(dot(viewDir, v.normal));
            o.rim = smoothstep(_RimMin, _RimMax, rim) * _RimColor.a;
            #endif

            #if MATCAP
            //MATCAP
            half3 worldNorm = normalize(
                unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[
                    2].xyz * v.normal.z);
            worldNorm = mul((half3x3)UNITY_MATRIX_V, worldNorm);
            o.matcap.xy = worldNorm.xy * 0.5 + 0.5;
            #endif
        }

        //================================================================
        // SURFACE FUNCTION

        void surf(Input IN, inout SurfaceOutputCustom o)
        {
            half4 main = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = main.rgb * _Color.rgb;
            o.Alpha = main.a * _Color.a;
            #if SPEC
            //Specular
            o.Gloss = main.a;
            o.Specular = _Shininess;
            #endif
            #if BUMP
            //Normal map
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            #endif
            #if RIM
            o.Emission += IN.rim * _RimColor.rgb;
            #endif
            #if EMISSION
            o.Emission += _Emission * _EmissionRate;
            #endif
            #if MATCAP
            half3 matcap = tex2D(_MatCap, IN.matcap).rgb;
            matcap *= main.a * _HColor.a;
            o.Emission += matcap;
            #endif
            o.WorldPos = IN.worldPos;
        }

        //================================================================
        // LIGHTING FUNCTION

        inline half4 LightingCustomLighting(SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten)
        {
            s.Normal = normalize(s.Normal);
            half ndl = max(0, dot(s.Normal, lightDir));
            #if RAMP
            half3 ramp = smoothstep(_RampThreshold - _RampSmooth * 0.5, _RampThreshold + _RampSmooth * 0.5, ndl);
            #else
            half3 ramp = ndl;
            #endif
            ramp *= atten;
            _SColor = lerp(_HColor, _SColor, _SColor.a);
            ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);

            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * ramp;
            c.a = s.Alpha;
            #if SPEC
            half3 h = normalize(lightDir + viewDir);
            half ndh = max(0, dot(s.Normal, h));
            half spec = pow(ndh, s.Specular * 128.0) * s.Gloss * 2.0;
            #if SPEC_TOON
            spec = smoothstep(0.5 - _SpecSmooth * 0.5, 0.5 + _SpecSmooth * 0.5, spec);
            #endif
            spec *= atten;
            c.rgb += _LightColor0.rgb * _SpecColor.rgb * spec;
            c.a += _LightColor0.a * _SpecColor.a * spec;
            #endif
            
            #if VERTICAL_FOG
            c.rgb = lerp(_FogColor-ShadeSH9(half4(s.Normal,1)), c.rgb, saturate((s.WorldPos.y - _FogHeight + _FogFade) / _FogFade));
            #endif
            
            return c;
        }
        ENDCG

    }

    Fallback "Diffuse"
}