Shader "Stencil/Write"
{
    Properties
    {

    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
        
        Blend Zero One
        ZWrite Off
        
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
            Fail Keep
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            float4 vert (appdata v) : SV_POSITION
            {
                return UnityObjectToClipPos(v.vertex);
            }

            fixed4 frag () : SV_Target
            {
                return 0;
            }
            
            ENDCG
        }
    }
}
