using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GridManager : MonoBehaviour
{
    public MarineDrone dronePrefab;

    [Header("References")]
    public VoronoiUtility voronoiUtility;
    public LoadingBar loadingBar;

    [Header("Map Data")]
    public MeshRenderer quad;
    public Texture2D bitMap;

    [Header("Game Grid")]
    public int gridXLength = 10;
    private MapCellData[,] mapCells = { };
    private List<MapCellData> borderCells = new List<MapCellData>();
    private float ar;
    private float gs;
    private Vector3 offsetPos;
    private float startTime;

    [Header("Drones")]
    public int droneCount = 10;
    private List<MapCellData> droneStartingPoints = new List<MapCellData>();

    void Start()
    {
        loadingBar.Show();

        startTime = Time.time;

        mapCells = new MapCellData[bitMap.width, bitMap.height];

        ar = (float)bitMap.width / bitMap.height;
        quad.transform.localScale = new Vector3(gridXLength * ar, gridXLength);

        offsetPos = new Vector3(-quad.GetComponent<MeshRenderer>().bounds.size.x / 2, 0, quad.GetComponent<MeshRenderer>().bounds.size.z / 2);
        quad.transform.position = offsetPos;

        gs = quad.GetComponent<MeshRenderer>().bounds.size.x / bitMap.width;

        StartCoroutine(GenerateMap(bitMap));
    }

    private IEnumerator GenerateMap(Texture2D bitMap)
    {
        loadingBar.UpdateProgress("Converting bitmap to map values...", 1, 5);

        for (int i = 0; i < mapCells.GetLength(0); i++)
            for (int j = 0; j < mapCells.GetLength(1); j++)
            {
                mapCells[i, j].xIndex = i;
                mapCells[i, j].yIndex = j;
                mapCells[i, j].xPos = offsetPos.x * 2 + i * gs + gs / 2;
                mapCells[i, j].zPos = offsetPos.y * 2 + j * gs + gs / 2;

                Color wholeColor = new Color(Mathf.RoundToInt(bitMap.GetPixel(i, j).r), Mathf.RoundToInt(bitMap.GetPixel(i, j).g), Mathf.RoundToInt(bitMap.GetPixel(i, j).b), 1);
                if (wholeColor == Color.black)
                    mapCells[i, j].isBorder = true;
                else if (wholeColor == Color.blue)
                    mapCells[i, j].isWater = true;
            }

        yield return 0;

        loadingBar.UpdateProgress("Creating world bitmap...", 2, 5);

        FlipGridHorizontal(mapCells);

        //mapCells = RotateGrid90(mapCells);
        MapCellData[,] vdata = RotateGrid90(mapCells);

        Color w = Color.white;
        for (int i = 0; i < mapCells.GetLength(0); i++)
            for (int j = 0; j < mapCells.GetLength(1); j++)
            {
                w = new Color(Mathf.RoundToInt(bitMap.GetPixel(i, j).r), Mathf.RoundToInt(bitMap.GetPixel(i, j).g), Mathf.RoundToInt(bitMap.GetPixel(i, j).b), 1);
                if (w == Color.black)
                    borderCells.Add(mapCells[i, j]);
            }

        yield return 0;

        loadingBar.UpdateProgress("Applying world bitmap to geo...", 3, 5);

        Texture2D tex = new Texture2D(bitMap.width, bitMap.height);
        int texIndex = 0;
        for (int i = 0; i < mapCells.GetLength(0); i++)
            for (int j = 0; j < mapCells.GetLength(1); j++)
            {
                if (mapCells[i, j].isBorder)
                    tex.SetPixel(i, j, Color.black);
                else if (mapCells[i, j].isWater)
                    tex.SetPixel(i, j, Color.blue);
                else
                    tex.SetPixel(i, j, Color.red);

                texIndex++;
            }

        tex.Apply();
        quad.material.mainTexture = tex;
        quad.material.mainTexture.filterMode = FilterMode.Point;

        yield return 0;

        loadingBar.UpdateProgress("Generating drone spawn locations...", 4, 5);

        int dimensions = Mathf.RoundToInt(Mathf.Sqrt(droneCount));
        Vector2Int[] c1 = new Vector2Int[dimensions * dimensions];
        int xDim = bitMap.width / dimensions;
        int droneIndex = 0;
        int dronesInWater = 0;
        for (int i = 0; i < dimensions; i++)
        {
            for (int y = 0; y < dimensions; y++)
            {
                int xC = xDim / 2 + xDim * y;
                int yC = xDim / 2 + xDim * i;

                c1[droneIndex] = new Vector2Int(xC, yC);
                droneIndex++;

                if (vdata[xC, yC].isWater)
                    dronesInWater++;
            }
        }

        Color[] regionColorsC1 = new Color[c1.Length];
        for (int i = 0; i < c1.Length; i++)
            regionColorsC1[i] = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);

        Array.Resize(ref c1, dronesInWater);
        droneIndex = 0;

        for (int i = 0; i < dimensions; i++)
        {
            for (int y = 0; y < dimensions; y++)
            {
                int xC = xDim / 2 + xDim * y;
                int yC = xDim / 2 + xDim * i;

                if (vdata[xC, yC].isWater)
                {
                    c1[droneIndex] = new Vector2Int(xC, yC);
                    droneIndex++;
                }
            }
        }

        for (int i = 0; i < c1.Length; i++)
        {
            //todo get values x = 0 - width, y = 0 - height / 2
            Vector2Int gridPos = new Vector2Int(c1[i].x, c1[i].y);
            Vector3 pos = new Vector3(vdata[gridPos.x, gridPos.y].xPos, 0, vdata[gridPos.x, gridPos.y].zPos);
            MarineDrone d = Instantiate(dronePrefab, pos, Quaternion.identity);
            d.Initialize(regionColorsC1[i], vdata);
            //d.name = $"C1:{i}. X: {gridPos.x}, Y: {gridPos.y}.";
        }

        Vector2Int[] c2 = new Vector2Int[dimensions * dimensions];
        droneIndex = 0;
        dronesInWater = 0;
        for (int i = 0; i < dimensions; i++)
        {
            for (int y = 0; y < dimensions; y++)
            {
                int xC = (bitMap.height / 2) + xDim / 2 + xDim * y;
                int yC = xDim / 2 + xDim * i;

                c2[droneIndex] = new Vector2Int(xC, yC);
                droneIndex++;

                if (vdata[xC, yC].isWater)
                    dronesInWater++;
            }
        }


        Color[] regionColorsC2 = new Color[c2.Length];
        for (int i = 0; i < c1.Length; i++)
            regionColorsC2[i] = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);

        Array.Resize(ref c2, dronesInWater);
        droneIndex = 0;

        for (int i = 0; i < dimensions; i++)
        {
            for (int y = 0; y < dimensions; y++)
            {
                int xC = (bitMap.height / 2) + xDim / 2 + xDim * y;
                int yC = xDim / 2 + xDim * i;

                if (vdata[xC, yC].isWater)
                {
                    c2[droneIndex] = new Vector2Int(xC, yC);
                    droneIndex++;
                }
            }
        }

        for (int i = 0; i < c2.Length; i++)
        {
            //todo get values x = width - height, y = 0 - width
            Vector2Int gridPos = new Vector2Int(c2[i].x, c2[i].y);
            Vector3 pos = new Vector3(vdata[gridPos.x, gridPos.y].xPos, 0, vdata[gridPos.x, gridPos.y].zPos);
            MarineDrone d = Instantiate(dronePrefab, pos, Quaternion.identity);
            d.Initialize(regionColorsC2[i], vdata);
            //d.name = $"C2:{i}. X: {gridPos.x}, Y: {gridPos.y}.";
        }

        for (int i = 0; i < c2.Length; i++)
            c2[i].x -= 1024;

        Texture2D voronoiDiagram = Combine2Textures(voronoiUtility.CreateDiagram(regionColorsC1, new Vector2Int(bitMap.width, bitMap.height / 2),
            c1), voronoiUtility.CreateDiagram(regionColorsC2, new Vector2Int(bitMap.width, bitMap.height / 2), c2));
        quad.material.mainTexture = voronoiDiagram;

        yield return 0;

        loadingBar.UpdateProgress("Combining bitmap with voronoi diagram...", 5, 5);

        //? add this functionality to the voronoi utility itself.
        for (int x = 0; x < voronoiDiagram.width; x++)
            for (int y = 0; y < voronoiDiagram.height; y++)
            {
                int index = x * voronoiDiagram.width + y;
                Color wholeColor = new Color(Mathf.RoundToInt(bitMap.GetPixel(x, y).r), Mathf.RoundToInt(bitMap.GetPixel(x, y).g), Mathf.RoundToInt(bitMap.GetPixel(x, y).b), 1);
                if (wholeColor == Color.red)
                    voronoiDiagram.SetPixel(x, y, Color.red);
                if (wholeColor == Color.black)
                    voronoiDiagram.SetPixel(x, y, Color.black);
            }

        voronoiDiagram.Apply();

        yield return 0;

        loadingBar.Hide();
    }

    private Texture2D Combine2Textures(Texture2D _textureA, Texture2D _textureB)
    {
        Texture2D combinedImagePair = new Texture2D(_textureA.width, _textureA.height + _textureB.height);
        combinedImagePair.SetPixels(0, 0, _textureA.width, _textureA.height, _textureA.GetPixels(), 0);
        combinedImagePair.SetPixels(0, _textureB.width, _textureB.width, _textureB.height, _textureB.GetPixels(), 0);
        combinedImagePair.filterMode = FilterMode.Point;
        combinedImagePair.Apply();
        return combinedImagePair;
    }

    public void FlipGridHorizontal(MapCellData[,] list)
    {
        for (int i = 0; i < list.GetLength(0); i++)
        {
            int start = 0;
            int end = list.GetLength(1) - 1;
            while (start < end)
            {
                MapCellData temp = list[i, start];
                list[i, start] = list[i, end];
                list[i, end] = temp;
                start++;
                end--;
            }
        }
    }

    public MapCellData[,] RotateGrid90(MapCellData[,] oldGrid)
    {
        MapCellData[,] newMatrix = new MapCellData[oldGrid.GetLength(1), oldGrid.GetLength(0)];
        int newColumn, newRow = 0;
        for (int oldColumn = oldGrid.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
        {
            newColumn = 0;
            for (int oldRow = 0; oldRow < oldGrid.GetLength(0); oldRow++)
            {
                newMatrix[newRow, newColumn] = oldGrid[oldRow, oldColumn];
                newColumn++;
            }
            newRow++;
        }
        return newMatrix;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (borderCells.Count > 0)
        {
            borderCells.ForEach(x =>
            {
                Gizmos.DrawWireCube(new Vector3(x.xPos, 0, x.zPos), new Vector3(gs, gs, gs));
            });
        }

        Gizmos.color = Color.magenta;

        if (droneStartingPoints.Count > 0)
        {
            droneStartingPoints.ForEach(x =>
            {
                if (x.isWater)
                    Gizmos.DrawSphere(new Vector3(x.xPos, 0, x.zPos), .25f);
            });
        }
    }

    //todo report progress from drones
}


public struct MapCellData
{
    public MapCellData(int xI, int yI, float x, float z, bool water, bool borderPosition)
    {
        xIndex = xI;
        yIndex = yI;
        xPos = x;
        zPos = z;
        isWater = water;
        isBorder = borderPosition;
    }

    public int xIndex;
    public int yIndex;
    public float xPos;
    public float zPos;
    public bool isWater;
    public bool isBorder;
}
