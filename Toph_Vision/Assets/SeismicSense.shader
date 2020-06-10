// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/SeismicSense"
{
    Properties
    {
        _MainTex ("Normal Map", 2D) = "white" {}
        _Freq ("Frequencey", Range(0.001, 100)) = 3
        _Wavelength ("Wavelength", Float) = 1.0
        _RingWidth ("Ring Width", Range(0.0001, 1.0)) = 0.25
        _RingPower ("Ring Power", Int) = 2
        _FadeLength ("Fade Length", Float) = 10
        _UseFade ("Use Fade", Int) = 0
        _DiffuseIntensity ("Diffuse Glow Intensity", Range(0.0, 1.0)) = 0.2
        _FadeGradient ("Glow Fade Gradient", Float) = 500.0
        _FadeOffset ("Glow Fade Offset", Float) = 40.0
        _DarkerColor ("Darker Color", Color) = (0.0, 0.0, 0.0, 0.0)
        _LighterColor ("Lighter Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
            //#pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float1 distanceToPoint : TEXCOORD1;
                fixed4 lightDirection : COLOR0;
                float4 vw : COLOR1;
                float4 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _ImpulsePoint;
            float _Freq;
            float _Wavelength;
            float _RingWidth;
            int _RingPower;
            float _Amp;
            float _Speed;
            float _FadeLength;
            int _UseFade;
            float _DiffuseIntensity;
            int _Switch;
            float _Offset;
            float _FadeGradient;
            float _FadeOffset;

            float4 _DarkerColor;
            float4 _LighterColor;

            float3 _ImpulseArray[1000];
            float _SwitchArray[1000];
            float _OffsetArray[1000];
            float _SpeedArray[1000];

            v2f vert (appdata v)
            {
                float3 xyzPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float dist = distance(_ImpulsePoint, xyzPos);

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vw = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.distanceToPoint = float1(dist);
                UNITY_TRANSFER_FOG(o,o.vertex);
                
                o.lightDirection = _WorldSpaceLightPos0;
                o.normal = float4(UnityObjectToWorldNormal(v.normal).x, UnityObjectToWorldNormal(v.normal).y, UnityObjectToWorldNormal(v.normal).z, 1.0);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                // sample the texture
                float4 col = fixed4(0.0, 0.0, 0.0, 1.0);

                float diffuse = float4(0.0, 0.0, 0.0, 0.0);

                for(int n = 0; n < 1000; n++) {

                    uint myIndex = n;
                    if(_SwitchArray[myIndex] == 0.0) {
                        continue;
                    }

                    float myOffset = _OffsetArray[myIndex];
                    float mySpeed = _SpeedArray[myIndex];

                    float distanceToPoint = distance(_ImpulseArray[myIndex], i.vw);

                    float maxDistance = (_Time.y - myOffset) * mySpeed;
                    float minDistance = ((_Time.y - myOffset) * mySpeed) - _Wavelength;
                    float val = abs(fmod((distanceToPoint - ((_Time.y - myOffset) * mySpeed)), _Wavelength / _Freq) * _Freq);
                    float r = _RingWidth * _Wavelength;
                    if(distanceToPoint < maxDistance) {
                        if(val < r == 1 && distanceToPoint > minDistance) {
                            float f = 1.0 - abs((val - (r / 2)) / r);
                            for(int index = 0; index < _RingPower; index++) {
                                f *= f;
                            }
                            if(_UseFade == 1) {
                                f /= distanceToPoint / _FadeLength;
                            }
                            col += fixed4(f, f, f, 0.0);
                        }

                        float diffuseAdd = abs((distanceToPoint - ((_Time.y - myOffset) * mySpeed)) * _Freq);
                        float fadeGradient = _FadeGradient;
                        float fadeGradientOffset = _FadeOffset;
                        if(maxDistance + fadeGradientOffset - distanceToPoint < fadeGradient) {
                            diffuseAdd *= (maxDistance - distanceToPoint) / fadeGradient;
                        }
                        float maxTime = 15.0;
                        float fadeTime = 10.0;
                        diffuse = min(1.0, diffuse + max(diffuseAdd * ((fadeTime - (_Time.y - myOffset)) / maxTime), 0.0));
                    }
                }

                float3 nHat = normalize(i.normal + tex2D(_MainTex, i.uv));
                float3 lHat = normalize(i.lightDirection);
                float diffuseValue = max(dot(nHat, lHat), 0.0);
                diffuseValue *= _DiffuseIntensity * diffuse;
                col = col + float4(diffuseValue, diffuseValue, diffuseValue, 0.0);

                col = lerp(_DarkerColor, _LighterColor, col.x);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
