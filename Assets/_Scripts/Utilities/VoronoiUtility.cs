using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Data.Common;
using UnityEngine.UI;

public class VoronoiUtility : MonoBehaviour
{
    public Texture2D CreateDiagram(Color32[] regionsC, Vector2Int dimensions, Vector2Int[] centroids)
    {
        Color[] pixelColors = new Color[dimensions.x * dimensions.y];

        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                int index = y * dimensions.x + x;
                Vector2Int coord = new Vector2Int(x, y);
                int closest = GetClosestCentroidIndex(coord, centroids);
                Color c = regionsC[closest];
                pixelColors[index] = c;
            }

        Texture2D tex = GetImageFromColorArray(dimensions, pixelColors);
        tex.SetPixel(0, 0, Color.green);
        tex.SetPixel(0, 1, Color.blue);
        tex.SetPixel(1, 0, Color.red);
        tex.Apply();
        return tex;
    }

    private int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
    {
        float smallestDst = float.MaxValue;
        int index = 0;
        for (int i = 0; i < centroids.Length; i++)
        {
            float d = Vector2.Distance(pixelPos, centroids[i]);
            if (d < smallestDst)
            {
                smallestDst = d;
                index = i;
            }
        }
        return index;
    }

    private Texture2D GetImageFromColorArray(Vector2Int dimensions, Color[] pixelColors)
    {
        Texture2D tex = new Texture2D(dimensions.x, dimensions.y);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixelColors);
        tex.Apply();
        return tex;
    }
}