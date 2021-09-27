Shader "Unlit/SparklesShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Scale ("Scale", Float) = 1
        _AbsoluteDisplacement ("Absolute Displacement", Float) = 0
        _RelativeDisplacement ("Relative Displacement", Float) = 0
    }
    SubShader
    {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off
		ZWrite Off
		Lighting Off
		Fog { Mode Off }
        
        Offset -1, -1 // depth bias

        Pass
        {
			Blend SrcAlpha One, Zero One
			BlendOp Add

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _Scale;
            float _AbsoluteDisplacement;
            float _RelativeDisplacement;

            v2f vert (appdata v)
            {
                v2f o;
                float3 origin = UnityObjectToViewPos(v.vertex.xyz);
                float3 normal = normalize(UnityObjectToViewPos(v.vertex.xyz + v.normal) - origin);
                float3 viewDir = lerp(normalize(origin), float3(0, 0, 1), unity_OrthoParams.w);
                //float3 dirX = normalize(cross(normal, float3(normal.y, -normal.z, normal.x)));
                //float3 dirY = normalize(cross(normal, dirX));
                //float2 dirScale = float2(1, 1);
                // float3 dirX = normalize(cross(normal, float3(0, 1, 0)));
                // float3 dirY = normalize(cross(normal, float3(1, 0, 0)));
                // float2 dirScale = min(abs(dirX.x), abs(dirY.y)) / abs(float2(dirX.x, dirY.y));
                float radius = _Scale * v.uv2.z * v.uv2.w;
                origin += normal * (_AbsoluteDisplacement + _RelativeDisplacement * radius);
                //float3 corner = origin + (dirX * (v.uv2.x * dirScale.x) + dirY * (v.uv2.y * dirScale.y)) * radius;
                float3 corner = origin + float3(v.uv2.x, v.uv2.y, 0) * radius;
                o.vertex = mul(UNITY_MATRIX_P, float4(corner, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = _Color * fixed4(1, 1, 1, max(dot(-normal, viewDir) * v.uv2.w, 0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * i.color;
            }
            ENDCG
        }
    }
}
