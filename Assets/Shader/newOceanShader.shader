﻿Shader "Custom/newOceanShader"
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _WaterFogColor ("Water Fog Color", Color) = (0, 0, 0, 0)
		_WaterFogDensity ("Water Fog Density", Range(0, 2)) = 0.1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        // _WaveA ("Wave A (dir,steepness, wavelength", Vector) = (1,0,0.5,10)
        // _WaveB ("Wave B", Vector) = (0,1,0.25,20)
        // _WaveC ("Wave B", Vector) = (0,1,0.25,20)
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        GrabPass { "_WaterBackground" }

        CGPROGRAM
        #pragma surface surf Standard alpha finalcolor:ResetAlpha vertex:vert addshadow
        #pragma target 3.0

        #include "LookingThroughWater.cginc"

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color, _WaveA, _WaveB, _WaveC;
        float _CustomTime;
        
        float3 GerstnerWave (
            float4 wave, float3 p, inout float3 tangent, inout float3 binormal
        ) {
            float steepness = wave.z;
            float wavelength = wave.w;
            float k = 2 * UNITY_PI / wavelength;
            float c = sqrt(9.8 / k);
            float2 d = normalize(wave.xy);
            float f = k * (dot(d, p.xz) - c * _CustomTime);
            float a = steepness / k;

            tangent += float3(
                -d.x * d.x * (steepness * sin(f)),
                d.x * (steepness * cos(f)),
                -d.x * d.y * (steepness * sin(f))
            );
            binormal += float3(
                -d.x * d.x * (steepness * sin(f)),
                d.x * ( steepness * cos(f)),
                -d.x * d.y * (steepness * sin(f))
            );
            return float3(
                d.x * (a * cos(f)),
                a * sin(f),
                d.y * (a * cos(f))
            );
        }

        void vert (inout appdata_full vertexData) {
            float3 gridPoint = vertexData.vertex.xyz;
            float3 tangent = float3(1, 0, 0);
            float3 binormal = float3(0, 0, 1);
            float3 p = gridPoint;
            p += GerstnerWave(_WaveA, gridPoint, tangent, binormal);
            // p += GerstnerWave(_WaveB, gridPoint, tangent, binormal);
            // p += GerstnerWave(_WaveC, gridPoint, tangent, binormal);
            float3 normal = normalize(cross(binormal, tangent));

            vertexData.vertex.xyz = p;
            vertexData.normal = normal;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            o.Emission = ColorBelowWater(IN.screenPos) * (1 - c.a);
        }

        void ResetAlpha (Input IN, SurfaceOutputStandard o, inout fixed4 color) {
			color.a = 1;
		}
        
        ENDCG
    }
}
