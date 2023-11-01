Shader "Dacodelaac/Water"
{
    Properties
    {
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1
		_FoamColor("Foam Color", Color) = (1,1,1,1)
		_SurfaceNoise("Surface Noise", 2D) = "white" {}
		_SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
		_SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777
		_SurfaceDistortion("Surface Distortion", 2D) = "white" {}	
		_SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.27
		_FoamMaxDistance("Foam Maximum Distance", Float) = 0.4
		_FoamMinDistance("Foam Minimum Distance", Float) = 0.04		
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
		}

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

            CGPROGRAM
			#define SMOOTHSTEP_AA 0.01

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			half4 alphaBlend(half4 top, half4 bottom)
			{
				half3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				half alpha = top.a + bottom.a * (1 - top.a);

				return half4(color, alpha);
			}

            struct appdata
            {
                half4 vertex : POSITION;
				half4 uv : TEXCOORD0;
				half3 normal : NORMAL;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;	
				half2 noiseUV : TEXCOORD0;
				half2 distortUV : TEXCOORD1;
				half4 screenPosition : TEXCOORD2;
				half3 viewNormal : NORMAL;
            };

			sampler2D _SurfaceNoise;
			half4 _SurfaceNoise_ST;

			sampler2D _SurfaceDistortion;
			half4 _SurfaceDistortion_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPosition = ComputeScreenPos(o.vertex);
				o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
				o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
				o.viewNormal = COMPUTE_VIEW_NORMAL;

                return o;
            }

			half4 _DepthGradientShallow;
			half4 _DepthGradientDeep;
			half4 _FoamColor;

			half _DepthMaxDistance;
			half _FoamMaxDistance;
			half _FoamMinDistance;
			half _SurfaceNoiseCutoff;
			half _SurfaceDistortionAmount;
			half2 _SurfaceNoiseScroll;

			sampler2D _CameraDepthTexture;
			sampler2D _CameraNormalsTexture;

            half4 frag (v2f i) : SV_Target
            {
				half existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;

				half existingDepthLinear = LinearEyeDepth(existingDepth01);

				half depthDifference = existingDepthLinear - i.screenPosition.w;

				half waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
				half4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);

				half3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPosition));

				half3 normalDot = saturate(dot(existingNormal, i.viewNormal));
				half foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);
				half foamDepthDifference01 = saturate(depthDifference / foamDistance);

				half surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;

				half2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;

				half2 noiseUV = half2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x, 
				(i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);
				half surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;				
				half surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);

				half4 surfaceNoiseColor = _FoamColor;
				surfaceNoiseColor.a *= surfaceNoise;
				
				return alphaBlend(surfaceNoiseColor, waterColor);
            }
            ENDCG
        }
    }
}
