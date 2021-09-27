Shader "Fur/FurSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [PerRendererData] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _Heightmap ("Heightmap", 2D) = "white" {}
        _Length ("Length", Float) = 0
        _Strength ("Strength", Float) = 1
        _LayerCount ("Layer Count", Int) = 1
        [HideInInspector] _Slice ("Slice", Float) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType" = "Opaque" }
        Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard addshadow fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        sampler2D _Heightmap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Heightmap;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float _Length;
        float _Strength;
        float _Slice;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            v.vertex.xyz += v.normal * (_Slice * _Length);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 mask = tex2D(_Heightmap, IN.uv_Heightmap);
            clip((mask.a * _Strength) - _Slice);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
