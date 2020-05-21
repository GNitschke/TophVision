// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ImpulsePoint ("Impulse Point", Vector) = (0, 0, 0)
        _Freq ("Frequencey", Range(0.001, 100)) = 3
        _Wavelength ("Wavelength", Float) = 1.0
        _RingWidth ("Ring Width", Range(0.0001, 1.0)) = 0.25
        _RingPower ("Ring Power", Int) = 2
        _Amp ("Amplitude", Float) = 0.05
        _Speed ("Speed", Float) = 5.0
        _FadeLength ("Fade Length", Float) = 10
        _UseFade ("Use Fade", Int) = 1
        _Switch ("Switch", Int) = 1
        _Offset ("Offset", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float1 distanceToPoint : TEXCOORD1;
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
            int _Switch;
            float _Offset;

            v2f vert (appdata v)
            {
            	// float3 xyzPos = float3(v.vertex.x, v.vertex.y, v.vertex.z) * 1.0/v.vertex.w;
                float3 xyzPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            	float dist = distance(_ImpulsePoint, xyzPos);
                float wave = abs(sin(_Freq * (dist - (_Time.x * _Speed))));
                wave *= wave * wave * wave;
            	xyzPos += float3(0.0, _Amp * wave, 0.0);
            	float4 newPos = mul(unity_WorldToObject, float4(xyzPos.xyz, 1.0));
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.distanceToPoint = float1(dist);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float maxDistance = (_Time.y - _Offset) * _Speed;
                float minDistance = ((_Time.y - _Offset) * _Speed) - _Wavelength;
                float val = cos(3.14159265359 * _Freq * (i.distanceToPoint - ((_Time.y - _Offset) * _Speed)));
                val = abs(fmod((i.distanceToPoint - ((_Time.y - _Offset) * _Speed)), _Wavelength / _Freq) * _Freq);
                float r = _RingWidth * _Wavelength;
                if(val < r && _Switch == 1 && i.distanceToPoint < maxDistance && i.distanceToPoint > minDistance) {
                    col = fixed4(1.0, 1.0, 1.0, 1.0);
                    float f = 1.0 - abs((val - (r / 2)) / r);
                    for(int index = 0; index < _RingPower; index++) {
                        f *= f;
                    }
                    if(_UseFade == 1) {
                        f /= i.distanceToPoint / _FadeLength;
                    }
                    col = fixed4(f, f, f, 1.0);
                } else {
                    col = fixed4(0.0, 0.0, 0.0, 1.0);
                }

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
