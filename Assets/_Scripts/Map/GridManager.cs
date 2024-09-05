using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;

public class GridManager : MonoBehaviour
{
    public MarineDrone dronePrefab;

    [Header("References")]
    public GameObject droneIcon;
    public GameObject chargerIcon;
    public MapData mapData;
    public Material overlayMaterial;
    public MeshRenderer inputMap;
    public VoronoiUtility voronoiUtility;
    public LoadingBar loadingBar;
    public DroneHUDController droneHUD;
    public TMP_InputField droneCountInput;

    int traversableAreaCount;

    [Header("Map Data")]
    public MeshRenderer quad;
    public int heightInKM = 16;
    public int widthInKM = 9;

    [Header("Game Grid")]
    public int gridXLength = 200;
    private MapCellData[,] mapCells = { };
    private List<List<MapCellData>> traversableAreas = new List<List<MapCellData>>();
    private float ar;
    private float gs;
    private Vector3 offsetPos;
    private Texture2D voronoiDiagram;
    public Color cleanedColor;

    public int TotalCellCount => mapData.bitmap.width * mapData.bitmap.height;
    public float GetGridSize => gs;

    [Header("Drones")]
    private List<MapCellData> droneStartingPoints = new List<MapCellData>();

    private List<Vector2Int> centroids = new List<Vector2Int>();
    private List<Drone> drones = new List<Drone>();

    public event Action<List<Vector2Int>> OnMapGenerationComplete = delegate { };
    private bool placingDrones = true;
    private bool placingChargers;
    private List<Vector2Int> chargerLocations = new List<Vector2Int>();

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        mapCells = new MapCellData[mapData.bitmap.width, mapData.bitmap.height];

        ar = (float)mapData.bitmap.width / mapData.bitmap.height;

        quad.transform.localScale = new Vector3(gridXLength * ar, gridXLength);
        quad.transform.position = new Vector3(gridXLength / 4, 0, gridXLength / 2);

        var overlayQuad = Instantiate(quad, new Vector3(gridXLength / 4, .1f, gridXLength / 2), quad.transform.rotation);
        overlayQuad.material = overlayMaterial;
        overlayQuad.material.mainTexture = mapData.overlayMap;
        overlayQuad.material.color = new Color(1, 1, 1, .5f);
        overlayQuad.gameObject.name = "OverlayMap";
        inputMap.transform.localScale = new Vector3(transform.localScale.x * ar, 1, transform.localScale.y);

        gs = quad.GetComponent<MeshRenderer>().bounds.size.x / mapData.bitmap.width;

        StartCoroutine(GenerateMap(mapData.bitmap));
    }

    private IEnumerator GenerateMap(Texture2D bitMap)
    {
        for (int i = 0; i < mapCells.GetLength(0); i++)
            for (int j = 0; j < mapCells.GetLength(1); j++)
            {
                mapCells[i, j].xIndex = i;
                mapCells[i, j].yIndex = j;
                mapCells[i, j].xPos = i * gs + gs / 2;
                mapCells[i, j].zPos = j * gs + gs / 2;

                Color wholeColor = new Color(Mathf.RoundToInt(bitMap.GetPixel(i, j).r), Mathf.RoundToInt(bitMap.GetPixel(i, j).g), Mathf.RoundToInt(bitMap.GetPixel(i, j).b), 1);
                if (wholeColor == Color.black)
                    mapCells[i, j].isBorder = true;
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
        inputMap.material.mainTexture = tex;
    }

    public void PlaceDrones()
    {
        if (centroids.Count == 0)
            return;

        FindObjectsByType<Drone>(FindObjectsSortMode.None).ToList().ForEach(x => Destroy(x.gameObject)); //! replace with a marker instead.

        Color32[] colorRegions = new Color32[centroids.Count];
        for (int i = 0; i < centroids.Count; i++)
            colorRegions[i] = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);

        for (int i = 0; i < centroids.Count; i++)
        {
            Vector2Int gridPos = new Vector2Int(centroids[i].x, centroids[i].y);
            Vector3 pos = new Vector3(mapCells[gridPos.x, gridPos.y].xPos, 0, mapCells[gridPos.x, gridPos.y].zPos);
            MarineDrone d = Instantiate(dronePrefab, pos, Quaternion.identity);
            d.Initialize(colorRegions[i]);
            drones.Add(d);
            d.name = $"C1:{i}. X: {gridPos.x}, Y: {gridPos.y}.";
        }

        voronoiDiagram = voronoiUtility.CreateDiagram(colorRegions, new Vector2Int(mapData.bitmap.width, mapData.bitmap.height), centroids.ToArray());

        for (int x = 0; x < mapData.bitmap.width; x++)
            for (int y = 0; y < mapData.bitmap.height; y++)
            {
                Color wholeColor = new Color(Mathf.RoundToInt(mapData.bitmap.GetPixel(x, y).r), Mathf.RoundToInt(mapData.bitmap.GetPixel(x, y).g), Mathf.RoundToInt(mapData.bitmap.GetPixel(x, y).b), 1);
                if (wholeColor == Color.red)
                    voronoiDiagram.SetPixel(x, y, Color.red);
                if (wholeColor == Color.black)
                    voronoiDiagram.SetPixel(x, y, Color.black);
            }

        quad.material.mainTexture = voronoiDiagram;
        voronoiDiagram.Apply();

        //todo test ----

        for (int lists = 0; lists < colorRegions.Length; lists++)
        {
            MapCellData temp = new MapCellData(0, 0, 0, 0, false, false, colorRegions[lists]);
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
                for (int ta = 0; ta < colorRegions.Length; ta++)
                    if (colorRegions[ta] == voronoiDiagram.GetPixel(x, y))
                        traversableAreas[ta].Add(mapCells[x, y]);
            }
        }

        for (int i = 0; i < drones.Count; i++)
            for (int j = 0; j < traversableAreas.Count; j++)
            {
                if (drones[i].TraversableColor.Equals(traversableAreas[i].ElementAt(0).color)) //todo sometimes this value is null or does not exist.
                    drones[i].SetTraversableCells(traversableAreas[i]);
            }

        droneHUD.gameObject.SetActive(true);
        traversableAreaCount = traversableAreas.SelectMany(list => list).Distinct().Count();
        var icons = FindObjectsByType<UIMapIcon>(FindObjectsSortMode.None).ToList();
        icons.ForEach(x => Destroy(x.gameObject));
        OnMapGenerationComplete(chargerLocations);
    }

    private void AddCoordinatesToDroneList(Vector2Int v)
    {
        if (placingDrones)
            centroids.Add(v);
        else if (placingChargers)
            chargerLocations.Add(v);
    }

    public float GetCellSizeInMeters()
    {
        int cells = mapCells.GetLength(0);
        float cellsPerKM = cells / widthInKM;
        float cellSize = cellsPerKM / 1000;
        return cellSize;
    }

    public int GetTraversableAreaCount => traversableAreaCount;

    public List<Drone> GetDrones => drones;

    private void LateUpdate()
    {
        if (updateTexture)
        {
            if (voronoiDiagram != null)
            {
                voronoiDiagram.Apply();
                updateTexture = false;
            }
        }
    }

    bool updateTexture;

    public void MapInteracted(Vector2Int coord, Vector3 worldSpace)
    {
        AddCoordinatesToDroneList(coord);

        if (placingDrones)
        {
            Instantiate(droneIcon, worldSpace, Quaternion.identity);
            Debug.Log("instance drone icon");
        }
        else if (placingChargers)
        {
            Instantiate(chargerIcon, worldSpace, Quaternion.identity);
        }
    }

    public Vector3 GetIndexPosition(Vector2Int coords)
    {
        Vector3 pos = new Vector3(mapCells[coords.x, coords.y].xPos, 0, mapCells[coords.x, coords.y].zPos);
        if (voronoiDiagram != null)
            voronoiDiagram.SetPixel(coords.x, coords.y, cleanedColor);
        updateTexture = true;
        return pos;
    }

    public Vector3 GetChargerPosition(Vector2Int coords)
    {
        Vector3 pos = new Vector3(mapCells[coords.x, coords.y].xPos, 0, mapCells[coords.x, coords.y].zPos);
        return pos;
    }

    public void PlacingDrones()
    {
        placingChargers = false;
        placingDrones = true;
    }

    public void PlacingChargers()
    {
        placingChargers = true;
        placingDrones = false;
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
