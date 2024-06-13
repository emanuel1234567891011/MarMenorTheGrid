using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;

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

        FlipGridHorizontal();

        mapCells = RotateGrid90(mapCells);


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

        yield return 0;

        Vector2Int[] centroids = new Vector2Int[droneCount];
        // centroids[0] = new Vector2Int(50, 100);
        // centroids[1] = new Vector2Int(200, 400);
        // centroids[2] = new Vector2Int(450, 650);
        // centroids[3] = new Vector2Int(550, 700);
        // centroids[4] = new Vector2Int(800, 1000);

        for (int i = 0; i < droneCount; i++)
        {

            centroids[i] = new Vector2Int(UnityEngine.Random.Range(0, bitMap.width), UnityEngine.Random.Range(0, bitMap.height / 2));
            //centroids[i] = new Vector2Int(UnityEngine.Random.Range(0, bitMap.width), UnityEngine.Random.Range(0, bitMap.height / 2)); //? Creates full texture
            Vector2Int gridPos = new Vector2Int(centroids[i].x, centroids[i].y);
            Vector3 pos = new Vector3(mapCells[gridPos.x, gridPos.y].xPos, 0, mapCells[gridPos.x, gridPos.y].zPos);
            Instantiate(DebugCube, pos, Quaternion.identity);
            Debug.Log($"Centroid X: {centroids[i].x} : Centroid Y: {centroids[i].y}");
            //! Something wrong with offset or scaling of positions for the grid locations, using same grid locations as the Gizmo (line 115) but different positions.
        }

        //3d grid position 1. flip horizontally, rotate by 90 degrees

        Vector2Int[] corners = new Vector2Int[4];
        corners[0] = new Vector2Int(0, 0); //bottom left
        corners[1] = new Vector2Int(mapCells.GetLength(0) - 1, 0); //bottom right
        corners[2] = new Vector2Int(0, mapCells.GetLength(1) / 2 - 1); //top right
        corners[3] = new Vector2Int(mapCells.GetLength(0) - 1, mapCells.GetLength(1) / 2 - 1);

        for (int i = 0; i < corners.Length; i++)
            Debug.Log("Corner: " + corners[i]);

        for (int i = 0; i < corners.Length; i++)
        {
            Vector2Int gridPos = new Vector2Int(corners[i].x, corners[i].y);
            Vector3 pos = new Vector3(mapCells[gridPos.x, gridPos.y].xPos, 0, mapCells[gridPos.x, gridPos.y].zPos);
            GameObject d = Instantiate(DebugCube, pos, Quaternion.identity);
            d.GetComponent<MeshRenderer>().material.color = Color.green;
            d.gameObject.name = corners[i].ToString();
        }

        yield return new WaitForSeconds(1);

        quad.material.mainTexture = voronoiUtility.CreateDiagram(new Vector2Int(bitMap.width, bitMap.height), centroids);
        //quad.material.mainTexture = voronoiUtility.CreateDiagram(new Vector2Int(bitMap.width, bitMap.height / 2), centroids); //? creates full texture

    }

    //! Debug flip, refactor later
    public void FlipGridHorizontal()
    {
        for (int i = 0; i < mapCells.GetLength(0); i++)
        {

            // Initialise start and end index
            int start = 0;
            int end = mapCells.GetLength(1) / 2 - 1;

            // Till start < end, swap the element
            // at start and end index
            while (start < end)
            {
                // Swap the element
                MapCellData temp = mapCells[i, start];
                mapCells[i, start] = mapCells[i, end];
                mapCells[i, end] = temp;

                // Increment start and decrement
                // end for next pair of swapping
                start++;
                end--;
            }
        }
    }

    public MapCellData[,] RotateGrid90(MapCellData[,] oldGrid)
    {

        MapCellData[,] newMatrix = new MapCellData[oldGrid.GetLength(1), oldGrid.GetLength(0)];
        int newColumn, newRow = 0;
        for (int oldColumn = oldGrid.GetLength(1) / 2 - 1; oldColumn >= 0; oldColumn--)
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
    //!==


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
