Shader "VoronoiGenerator"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            //Properties

            float4 pointsColor[500];
            float4 pointsCoor[500];
            int pointsCount;
            float pointsStrength[100];

            int distanceType;
            float minkowskiP;

            float siteRadius;

            int wrapH;
            int wrapV;

            /*
                Most distance functions are Toroid Aware,
                able to wrap around Horizontally and Vertically with 1 check

                Minkowski Generalization is not Toroid Aware,
                needs to check replicas in all directions
            */

            //Eucledian Distance
            float eucledianDistance(float2 a, float2 b)
            {
                float dx = abs(b.x - a.x);
                float dy = abs(b.y - a.y);

                //Toroidal Awareness
                if (wrapH > 0 && dx > 0.5f)
                    dx = 1.0f - dx;

                if (wrapV > 0 && dy > 0.5f)
                    dy = 1.0f - dy;

                return (dx*dx + dy*dy); //sqrt not needed when only comparing distances
            }

            //Manhattan Distance
            float manhattanDistance(float2 p1, float2 p2) {
                float dx = abs(p1.x - p2.x);
                float dy = abs(p1.y - p2.y);

                //Toroidal Awareness
                if (wrapH > 0 && dx > 0.5f)
                    dx = 1.0f - dx;

                if (wrapV > 0 && dy > 0.5f)
                    dy = 1.0f - dy;

                return dx + dy;
            }

            //Chebyshev Distance
            float chebyshevDistance(float2 p1, float2 p2) {
                float dx = abs(p1.x - p2.x);
                float dy = abs(p1.y - p2.y);

                //Toroidal Awareness
                if (wrapH > 0 && dx > 0.5f)
                    dx = 1.0f - dx;

                if (wrapV > 0 && dy > 0.5f)
                    dy = 1.0f - dy;

                return max(dx, dy);
            }

            //Generalized Minkowski Distance
            float minkowskiDistance(float2 p1, float2 p2, float power) {
                float dx = abs(p1.x - p2.x);
                float dy = abs(p1.y - p2.y);

                //Toroidal Awareness
                if (wrapH > 0 && dx > 0.5f)
                    dx = 1.0f - dx;

                if (wrapV > 0 && dy > 0.5f)
                    dy = 1.0f - dy;

                dx = pow(dx, power);
                dy = pow(dy, power);

                return pow(dx + dy, 1.0f / power); //pow with (1.0 / power) not needed when only comparing distances
            }

            //Distance Dispatcher
            float distanceSelector(float2 p1, float2 p2, float strength)
            {
                float dist = 1;

                if(distanceType == 0)
                    dist = eucledianDistance(p1, p2);
                else if(distanceType == 1)
                    dist = manhattanDistance(p1, p2);
                else if(distanceType == 2)
                    dist = chebyshevDistance(p1, p2);
                else if(distanceType == 3)
                    dist = minkowskiDistance(p1, p2, minkowskiP);

                //Apply site strength
                dist /= strength;

                return dist;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f input) : SV_Target
            {
                //Closest Center
                float minDistance = 1;
                float4 cl = float4(0,0,0,0);
                float currDistance = 0;
                float4 black = float4(0,0,0,1);
                int p = 0;
                for (p = 0; p < pointsCount; p++)
                {
                    //Find closest center with desired distance metric
                    currDistance = distanceSelector(input.uv, pointsCoor[p].xy, pointsStrength[p]);
                    if (currDistance < minDistance)
                    {
                        minDistance = currDistance;
                        cl = pointsColor[p];
                    }


                }

                //Draw centers
                if (siteRadius > 0)
                {
                    for (p = 0; p < pointsCount; p++)
                    {
                        if (sqrt(eucledianDistance(input.uv, pointsCoor[p].xy)) < siteRadius)
                        {
                            cl = black;
                            break;
                        }
                    }
                }

                return cl;
            }
            ENDCG
        }
    }
}
