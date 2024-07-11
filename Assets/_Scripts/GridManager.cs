using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;
using TMPro;
using System.Security.Cryptography;

public class GridManager : MonoBehaviour
{
    public MarineDrone dronePrefab;

    [Header("References")]
    public VoronoiUtility voronoiUtility;
    public LoadingBar loadingBar;
    public DroneHUDController droneHUD;
    public TMP_InputField droneCountInput;

    int traversableAreaCount;

    [Header("Map Data")]
    public MeshRenderer quad;
    public Texture2D bitMap;
    public int heightInKM = 16;
    public int widthInKM = 9;

    [Header("Game Grid")]
    public int gridXLength = 10;
    private MapCellData[,] mapCells = { };
    private List<List<MapCellData>> traversableAreas = new List<List<MapCellData>>();
    private float ar;
    private float gs;
    private Vector3 offsetPos;
    private Texture2D voronoiDiagram;
    public Color cleanedColor;

    [Header("Drones")]
    private List<MapCellData> droneStartingPoints = new List<MapCellData>();

    private MapCellData[,] vdata;

    //! refactor
    List<Drone> d1 = new List<Drone>();
    List<Drone> d2 = new List<Drone>();

    public event Action OnMapGenerationComplete = delegate { };

    private void Start()
    {
        droneCountInput.text = "5";
    }

    public void Init()
    {
        loadingBar.Show();

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
        loadingBar.UpdateProgress("Converting bitmap to map values...", 1, 6);

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

        loadingBar.UpdateProgress("Creating world bitmap...", 2, 6);

        FlipGridHorizontal(mapCells);

        //mapCells = RotateGrid90(mapCells);
        vdata = RotateGrid90(mapCells);

        loadingBar.UpdateProgress("Applying world bitmap to geo...", 3, 6);

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

        loadingBar.UpdateProgress("Generating drone spawn locations...", 4, 6);

        int dCount = int.Parse(droneCountInput.text);
        if (dCount % 2 > 0)
            dCount += 1;

        int dimensions = dCount / 2;


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

        Color32[] regionColorsC1 = new Color32[c1.Length];
        for (int i = 0; i < c1.Length; i++)
            regionColorsC1[i] = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);

        Array.Resize(ref c1, dronesInWater);
        Array.Resize(ref regionColorsC1, dronesInWater);
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
            d.Initialize(regionColorsC1[i]);
            d1.Add(d);
            //d.name = $"C1:{i}. X: {gridPos.x}, Y: {gridPos.y}.";
        }

        Vector2Int[] c2 = new Vector2Int[dimensions * dimensions]; //! if index is broken when changing drone count check dim vs region colors
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

        Color32[] regionColorsC2 = new Color32[c2.Length];


        for (int i = 0; i < c1.Length; i++)
            regionColorsC2[i] = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);

        Array.Resize(ref c2, dronesInWater);
        Array.Resize(ref regionColorsC2, dronesInWater);

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
            d.Initialize(regionColorsC2[i]);
            d2.Add(d);
            //d.name = $"C2:{i}. X: {gridPos.x}, Y: {gridPos.y}.";
        }

        for (int i = 0; i < c2.Length; i++)
            c2[i].x -= 1024;

        voronoiDiagram = Combine2Textures(voronoiUtility.CreateDiagram(regionColorsC1, new Vector2Int(bitMap.width, bitMap.height / 2),
            c1), voronoiUtility.CreateDiagram(regionColorsC2, new Vector2Int(bitMap.width, bitMap.height / 2), c2));
        quad.material.mainTexture = voronoiDiagram;

        yield return 0;

        loadingBar.UpdateProgress("Combining bitmap with voronoi diagram...", 5, 6);

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

        loadingBar.UpdateProgress("Assigning traversable areas...", 6, 6);

        Color32[] allRegions = regionColorsC1.Union(regionColorsC2).ToArray();

        for (int lists = 0; lists < allRegions.Length; lists++)
        {
            MapCellData temp = new MapCellData(0, 0, 0, 0, false, false, allRegions[lists]);
            traversableAreas.Add(new List<MapCellData>());
        }

        for (int x = 0; x < voronoiDiagram.width; x++)
        {
            for (int y = 0; y < voronoiDiagram.height; y++)
            {
                mapCells[x, y].xIndex = x;
                mapCells[x, y].yIndex = y;
                mapCells[x, y].xPos = offsetPos.x * 2 + x * gs + gs / 2;
                mapCells[x, y].zPos = offsetPos.y * 2 + y * gs + gs / 2;
                mapCells[x, y].color = voronoiDiagram.GetPixel(x, y);
                for (int ta = 0; ta < allRegions.Length; ta++)
                    if (allRegions[ta] == voronoiDiagram.GetPixel(x, y))
                        traversableAreas[ta].Add(mapCells[x, y]);
            }
        }

        var allDrones = d1.Union(d2).ToList();

        for (int i = 0; i < allDrones.Count; i++)
        {
            for (int j = 0; j < traversableAreas.Count; j++)
            {
                if (allDrones[i].TraversableColor.Equals(traversableAreas[i].ElementAt(0).color))
                    allDrones[i].SetTraversableCells(traversableAreas[i]);
            }
        }

        yield return 0;

        loadingBar.Hide();
        droneHUD.gameObject.SetActive(true);
        traversableAreaCount = traversableAreas.SelectMany(list => list).Distinct().Count();
        OnMapGenerationComplete();
    }

    public float GetCellSizeInMeters()
    {
        int cells = mapCells.GetLength(0);
        float cellsPerKM = cells / widthInKM;
        float cellSize = cellsPerKM / 1000;
        return cellSize;
    }

    public int GetTraversableAreaCount => traversableAreaCount;

    private Texture2D Combine2Textures(Texture2D _textureA, Texture2D _textureB)
    {
        Texture2D combinedImagePair = new Texture2D(_textureA.width, _textureA.height + _textureB.height);
        combinedImagePair.SetPixels(0, 0, _textureA.width, _textureA.height, _textureA.GetPixels(), 0);
        combinedImagePair.SetPixels(0, _textureB.width, _textureB.width, _textureB.height, _textureB.GetPixels(), 0);
        combinedImagePair.filterMode = FilterMode.Point;
        combinedImagePair.Apply();
        return combinedImagePair;
    }

    public List<Drone> GetDrones => d1.Union(d2).ToList();

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

    private void LateUpdate()
    {
        if (updateTexture)
        {
            voronoiDiagram.Apply();
            updateTexture = false;
        }
    }

    bool updateTexture;

    public Vector3 GetIndexPosition(Vector2Int coords)
    {
        Vector3 pos = new Vector3(mapCells[coords.x, coords.y].xPos, 0, mapCells[coords.x, coords.y].zPos);
        voronoiDiagram.SetPixel(coords.x, coords.y, cleanedColor);
        updateTexture = true;
        return pos;
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

}


public struct MapCellData
{
    public MapCellData(int xI, int yI, float x, float z, bool water, bool borderPosition, Color c)
    {
        xIndex = xI;
        yIndex = yI;
        xPos = x;
        zPos = z;
        isWater = water;
        isBorder = borderPosition;
        color = c;
    }

    public int xIndex;
    public int yIndex;
    public float xPos;
    public float zPos;
    public bool isWater;
    public bool isBorder;
    public Color32 color;
}

public struct TraversableArea
{
    public TraversableArea(Vector2Int i, Color32 c)
    {
        index = i;
        color = c;
    }

    public Vector2Int index;
    public Color32 color;
}
