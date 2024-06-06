using UnityEngine;
using UnityEngine.Video;

public class VoronoiUtility : MonoBehaviour
{
    public Texture2D CreateDiagram(Vector2Int dimensions, Vector2Int[] centroids)
    {
        Color[] regionColors = new Color[centroids.Length];
        Vector2Int[] randomC = new Vector2Int[centroids.Length];

        for (int i = 0; i < centroids.Length; i++)
        {
            regionColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            randomC[i] = new Vector2Int(Random.Range(0, dimensions.x), Random.Range(0, dimensions.x));
        }

        Color[] pixelColors = new Color[dimensions.x * dimensions.y];

        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                int index = x * dimensions.x + y; //! index is not accounting for non-square textures.
                pixelColors[index] = regionColors[GetClosestCentroidIndex(new Vector2Int(x, y), randomC)];
            }

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
