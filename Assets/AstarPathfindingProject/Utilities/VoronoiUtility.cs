using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Codice.Client.BaseCommands.BranchExplorer;
using System;

public class VoronoiUtility : MonoBehaviour
{
    public MeshRenderer testMesh;

    // private void Start()
    // {
    //     Vector2Int[] centroids = new Vector2Int[20];
    //     for (int i = 0; i < 20; i++)
    //         centroids[i] = new Vector2Int(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100));

    //     testMesh.material.mainTexture = CreateDiagram(new Vector2Int(100, 100), centroids);
    // }

    public Texture2D CreateDiagram(Color[] regionsC, Vector2Int dimensions, Vector2Int[] centroids)
    {
        Color[] pixelColors = new Color[dimensions.x * dimensions.y];

        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                int index = x * dimensions.x + y;
                Color c = regionsC[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
                pixelColors[index] = c;
            }

        // //! debug
        // for (int x = 0; x < dimensions.x; x++)
        //     for (int y = 0; y < dimensions.y; y++)
        //     {
        //         int index = x * dimensions.x + y;
        //         for (int i = 0; i < centroids.Length; i++)
        //         {
        //             Vector2Int pos = new Vector2Int(x, y);
        //             Vector2Int centroidPos = new Vector2Int(centroids[i].x, centroids[i].y);
        //             if (pos == centroidPos)
        //                 pixelColors[index] = Color.green;
        //         }
        //     }
        // //!

        return GetImageFromColorArray(dimensions, pixelColors);
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
