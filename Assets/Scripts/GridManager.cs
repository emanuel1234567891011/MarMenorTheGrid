using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject DebugCube;

    [Header("References")]
    public VoronoiUtility voronoiUtility;

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
    private Vector2Int[] centroids;

    void Start()
    {
        startTime = Time.time;

        mapCells = new MapCellData[bitMap.width, bitMap.height];

        ar = (float)bitMap.width / bitMap.height;
        quad.transform.localScale = new Vector3(gridXLength * ar, gridXLength);
        offsetPos = new Vector3(-quad.GetComponent<MeshRenderer>().bounds.size.x / 2, 0, quad.GetComponent<MeshRenderer>().bounds.size.z / 2);
        quad.transform.position = offsetPos;

        gs = quad.GetComponent<MeshRenderer>().bounds.size.x / bitMap.width;

        StartCoroutine(GetMapValues(bitMap));
    }

    private IEnumerator GetMapValues(Texture2D bitMap)
    {
        for (int i = 0; i < mapCells.GetLength(0); i++)
            for (int j = 0; j < mapCells.GetLength(1); j++)
            {
                mapCells[i, j].xIndex = i;
                mapCells[i, j].yIndex = j;
                mapCells[i, j].xPos = offsetPos.x * 2 + i * gs + gs / 2;
                mapCells[i, j].zPos = offsetPos.y * 2 + j * gs + gs / 2;

                Color wholeColor = new Color(Mathf.RoundToInt(bitMap.GetPixel(i, j).r), Mathf.RoundToInt(bitMap.GetPixel(i, j).g), Mathf.RoundToInt(bitMap.GetPixel(i, j).b), 1);
                if (wholeColor == Color.black)
                {
                    mapCells[i, j].isBorder = true;
                    borderCells.Add(mapCells[i, j]);
                }
                else if (wholeColor == Color.blue)
                    mapCells[i, j].isWater = true;
            }

        yield return 0;

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

        Debug.Log($"Grid took {Time.time - startTime} to finish.");

        DefineDroneStartingPositions(mapCells);

        yield return 0;

        Vector2Int[] centroids = new Vector2Int[droneCount];
        for (int i = 0; i < droneCount; i++)
        {
            centroids[i] = new Vector2Int(UnityEngine.Random.Range(0, bitMap.width), UnityEngine.Random.Range(0, bitMap.height / 2));
            Vector2Int gridPos = new Vector2Int(centroids[i].x, centroids[i].y);
            Vector3 pos = new Vector3(mapCells[gridPos.x, gridPos.y].xPos, 0, mapCells[gridPos.x, gridPos.y].zPos);
            Instantiate(DebugCube, pos, Quaternion.identity);
            Debug.Log($"Centroid X: {centroids[i].x} : Centroid Y: {centroids[i].y}");
        }

        quad.material.mainTexture = voronoiUtility.CreateDiagram(new Vector2Int(bitMap.width, bitMap.height), centroids);
    }

    private void DefineDroneStartingPositions(MapCellData[,] grid)
    {
        // //? How to evenly space out the grid to ensure the correct number of drones based on aspect ration of gird.

        // int xSteps = Mathf.FloorToInt(grid.GetLength(0) / droneCount);
        // int ySteps = Mathf.FloorToInt(grid.GetLength(1) / droneCount);

        // for (int i = 0; i < droneCount; i++)
        //     for (int j = 0; j < droneCount; j++)
        //         droneStartingPoints.Add(grid[i * xSteps, j * ySteps]);
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

        //? Consider the translation between the 2D grid and the 3D world is not accurate.
        if (centroids != null && centroids.Length > 0)
            for (int i = 0; i < centroids.Length; i++)
            {
                Vector2Int gridPos = new Vector2Int(centroids[i].x, centroids[i].y);
                Vector3 pos = new Vector3(mapCells[gridPos.x, gridPos.y].xPos, 0, mapCells[gridPos.x, gridPos.y].zPos);
                Gizmos.DrawSphere(pos, 1);
            }
    }
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

