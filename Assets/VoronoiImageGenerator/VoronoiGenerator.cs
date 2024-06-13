using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoronoiGenerator
{

    public class CoordinatesOutOfVoronoi : Exception
    {
        public CoordinatesOutOfVoronoi()
        {
        }
    }
    public class DifferentNumberOfCoordinatesAndColors : Exception
    {
        public DifferentNumberOfCoordinatesAndColors()
        {
        }
    }
    public class DifferentNumberOfCoordinatesAndStrengths : Exception
    {
        public DifferentNumberOfCoordinatesAndStrengths()
        {
        }
    }

    private static bool setup = false;
    private static Material material;

    public enum DistanceFormula
    {
        EUCLEDIAN,
        MANHATTAN,
        CHEBYSHEV,
        MINKOWSKI,
    }

    //Settings
    public static DistanceFormula distanceFormula = DistanceFormula.EUCLEDIAN;
    public static float minkowskiP = 1.5f;
    public static bool wrapHorizontal = false;
    public static bool wrapVertical = false;
    public static float siteRadius = 0.005f;

    //Normalize Vector2 coordinates to UV of Size
    private static List<Vector2> NormalizeVector2(Vector2Int size, List<Vector2> coors)
    {
        List<Vector2> res = new List<Vector2>(coors.Count);
        for (int i = 0; i < coors.Count; i++)
        {
            res.Add(new Vector2(coors[i].x / size.x, coors[i].y / size.y));
        }
        return res;
    }

    //Convert Vector2 to Vector4
    private static List<Vector4> Vec2toVec4(List<Vector2> vec)
    {
        List<Vector4> res = new List<Vector4>(vec.Count);
        for (int i = 0; i < vec.Count; i++)
        {
            res.Add(new Vector4(vec[i].x, vec[i].y));
        }
        return res;
    }

    //Setup
    public static void Setup()
    {
        if (!setup)
        {
            material = new Material(Shader.Find("VoronoiGenerator"));
            setup = true;
        }
    }

    //Generate Voronoi
    public static RenderTexture GenerateVoronoi(Vector2Int size, List<Vector2> coors, List<Color> colors)
    {
        //Ensure setup was ran
        Setup();

        //No strength provided, use value 1 for all
        List<float> strength = new List<float>(coors.Count);
        for (int i = 0; i < coors.Count; i++)
        {
            strength.Add(1);
        }

        return GenerateVoronoi(size, coors, colors, strength);
    }
    public static RenderTexture GenerateVoronoi(Vector2Int size, List<Vector2> coors, List<Color> colors, List<float> strength)
    {
        //Ensure setup was ran
        Setup();

        //Check coors inside size
        for (int i = 0; i < coors.Count; i++)
        {
            if (coors[i].x < 0 || coors[i].x >= size.x || coors[i].y < 0 || coors[i].y >= size.y)
                throw new CoordinatesOutOfVoronoi();
        }

        //Check coors and colors size
        if (coors.Count != colors.Count)
            throw new DifferentNumberOfCoordinatesAndColors();

        //Check coors and strength size
        if (coors.Count != strength.Count)
            throw new DifferentNumberOfCoordinatesAndStrengths();

        //Create RT
        RenderTexture tex = new RenderTexture(size.x, size.y, 0);
        tex.filterMode = FilterMode.Point;
        tex.Create();

        //Normalize Coordinates as UV
        coors = NormalizeVector2(size, coors);

        //Set Voronoi Settings
        material.SetColorArray("pointsColor", colors);
        material.SetVectorArray("pointsCoor", Vec2toVec4(coors));
        material.SetInt("pointsCount", coors.Count);
        material.SetFloatArray("pointsStrength", strength);
        material.SetInt("wrapH", wrapHorizontal ? 1 : 0);
        material.SetInt("wrapV", wrapVertical ? 1 : 0);
        material.SetInt("distanceType", (int)distanceFormula);
        material.SetFloat("minkowskiP", minkowskiP);
        material.SetFloat("siteRadius", siteRadius);

        //Get temporary RT
        RenderTexture tmp = RenderTexture.GetTemporary(tex.descriptor);

        //Generate Voronoi
        Graphics.Blit(tex, tmp, material);
        Graphics.Blit(tmp, tex);

        //Release temporary RT
        RenderTexture.ReleaseTemporary(tmp);

        return tex;
    }
}
