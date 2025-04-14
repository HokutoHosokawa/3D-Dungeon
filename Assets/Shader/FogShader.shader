Shader "Unlit/FogShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0,0,0,1) // Fogの色（黒）
        _FogDensity ("Fog Density", Range(0,1)) = 0.5 // Fogの濃さ
        _ExitPosition ("Exit Position", Vector) = (0,0,0,1) // 出口の位置
        _RoomNormal ("Room Normal", Vector) = (1,0,0,1) // 部屋の壁の法線
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend

        sampler2D _MainTex;
        fixed4 _FogColor;
        float _FogDensity;
        float3 _ExitPosition;
        float3 _RoomNormal;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            // float3 thisToExit = _ExitPosition - IN.worldPos;
            float3 thisToCamera = _WorldSpaceCameraPos - IN.worldPos;
            // float3 componentsOfThisToCameraInRoomNormalDirection = dot(thisToCamera, _RoomNormal) * _RoomNormal;
            // float3 thisToIntersectionOfThisToCameraAndRoom = length(dot(thisToExit, _RoomNormal) * _RoomNormal)/length(componentsOfThisToCameraInRoomNormalDirection) * thisToCamera;
            // float fogDistance = length(thisToIntersectionOfThisToCameraAndRoom);
            float fogDistance = length(thisToCamera);
            float fogFactor = saturate(exp(-_FogDensity * _FogDensity * fogDistance * fogDistance));
            c.rgb = lerp(_FogColor.rgb, c.rgb, fogFactor);
            o.Albedo = _FogColor.rgb;
            o.Alpha = c.a;


            // // カメラの方向ベクトル
            // float3 cameraDir = normalize(IN.viewDir);

            // // 部屋の壁面に対して Ray を投射し、交点 A を求める
            // float3 roomPlaneNormal = normalize(_RoomNormal);
            // float denom = dot(cameraDir, roomPlaneNormal);

            // if (abs(denom) > 0.0001)
            // {
            //     float t = dot(_ExitPosition - IN.worldPos, roomPlaneNormal) / denom;
            //     float3 intersectionPoint = IN.worldPos + t * cameraDir;
            //     float fogDistance = length(intersectionPoint - IN.worldPos);

            //     // Exponential Squared Fog 計算
            //     float fogFactor = saturate(exp(-_FogDensity * _FogDensity * fogDistance * fogDistance));

            //     c.rgb = lerp(_FogColor.rgb, c.rgb, fogFactor);
            // }

            // o.Albedo = c.rgb;
            // o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Transparent"
}
