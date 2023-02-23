Shader "DMIIShader" {
    Properties {
        _CellTextures("Texture", 2DArray) = "white" {}
    }
    SubShader {
        Tags {
            "Queue" = "Transparent"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma instancing_options procedural:setup

            struct vertex {
                float4 loc  : POSITION;
                float2 uv   : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct fragment {
                float4 loc  : SV_POSITION;
                float3 uv   : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(MyData)
                float4 posDirBuffer[7];
                float textureIndex[7];
            CBUFFER_END

    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            void setup() {
                float2 position = posDirBuffer[unity_InstanceID].xy;
                float2 direction = posDirBuffer[unity_InstanceID].zw;

                unity_ObjectToWorld = float4x4(
                    direction.x, -direction.y, 0, position.x,
                    direction.y, direction.x, 0, position.y,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                    );
            }
    #endif

            sampler2D _MainTex;
            float _FadeInT; //We'll use this later
            fragment vert(vertex v) {
                fragment f;
                UNITY_SETUP_INSTANCE_ID(v);
                f.loc = UnityObjectToClipPos(v.loc);

                #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) || defined(UNITY_INSTANCING_ENABLED)
                f.uv = float3(v.uv, textureIndex[unity_InstanceID]);
                #endif

                return f;
            }

            UNITY_DECLARE_TEX2DARRAY(_CellTextures);

            float4 frag(fragment f) : SV_Target{
                float4 c = float4(0,0,0,1);

                c = UNITY_SAMPLE_TEX2DARRAY(_CellTextures, f.uv);

                return c;
            }
            ENDCG
        }
    }
}