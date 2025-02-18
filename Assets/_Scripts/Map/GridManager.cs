using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;
using cube_game.Drawing_on_3D_game_objects;

public class GridManager : MonoBehaviour
{
    public MarineDrone dronePrefab;

    [Header("References")]
    public Color swimmingAreaColor;
    public UIMapIcon droneIcon;
    public UIMapIcon chargerIcon;
    public UIMapIcon waypointsIcon;
    public MapData mapData;
    public Material overlayMaterial;
    public MeshRenderer inputMap;
    public VoronoiUtility voronoiUtility;
    public LoadingBar loadingBar;
    public DroneHUDController droneHUD;
    public MeshRenderer _satelliteQuad;

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
    private Texture2D ot;

    public int TotalCellCount => mapData.bitmap.width * mapData.bitmap.height;
    public float GetGridSize => gs;

    [Header("Drones")]
    private List<MapCellData> droneStartingPoints = new List<MapCellData>();

    private List<Vector2Int> centroids = new List<Vector2Int>();
    private List<MarineDrone> drones = new List<MarineDrone>();

    public int DroneCount => centroids.Count;

    public event Action<List<Vector2Int>> OnMapGenerationComplete = delegate { };
    public bool placingDrones = true;
    public bool placingChargers;
    private List<Vector2Int> chargerLocations = new List<Vector2Int>();
    public int ChargerCount => chargerLocations.Count;
    public List<UIMapIcon> droneIcons = new List<UIMapIcon>();
    public List<UIMapIcon> chargerIcons = new List<UIMapIcon>();
    List<Vector2Int> swimmingAreas = new List<Vector2Int>();
    private Texture2D tex;

    public bool placingWaypoints;
    private List<Vector2Int> _wayPoints = new List<Vector2Int>();
    private List<UIMapIcon> _waypointIcons = new List<UIMapIcon>();

    public void Init()
    {
        mapCells = new MapCellData[mapData.bitmap.width, mapData.bitmap.height];

        ar = (float)mapData.bitmap.width / mapData.bitmap.height;
        quad.transform.localScale = new Vector3(gridXLength * ar, gridXLength);
        quad.transform.position = new Vector3(gridXLength / 4, 0, gridXLength / 2);

        var overlayQuad = Instantiate(quad, new Vector3(gridXLength / 4, .1f, gridXLength / 2), quad.transform.rotation);
        overlayQuad.material = overlayMaterial;

        ot = new Texture2D(mapData.overlayMap.width, mapData.overlayMap.height);
        ot.filterMode = FilterMode.Point;
        ot.SetPixels(mapData.overlayMap.GetPixels());
        ot.Apply();
        overlayQuad.material.mainTexture = ot;

        overlayQuad.material.color = new Color(1, 1, 1, .5f);
        overlayQuad.gameObject.name = "OverlayMap";
        Vector3 scale = new Vector3(transform.localScale.x * ar, 1, transform.localScale.y); ;
        inputMap.transform.localScale = scale;
        inputMap.gameObject.SetActive(false);
        _satelliteQuad.transform.localScale = scale;
        _satelliteQuad.transform.position = new Vector3(inputMap.transform.position.x, inputMap.transform.transform.position.y + .001f, inputMap.transform.transform.position.z);
        _satelliteQuad.material.mainTexture = mapData.overlayMap;
        gs = quad.GetComponent<MeshRenderer>().bounds.size.x / mapData.bitmap.width;

        mapData.overlayMap.filterMode = FilterMode.Point;

        ShowInputMap();

        FindAnyObjectByType<Drawing_control>().Init();
    }

    public void ShowInputMap()
    {
        inputMap.gameObject.SetActive(true);
        ar = (float)mapData.bitmap.width / mapData.bitmap.height;
        //inputMap.GetComponent<MeshRenderer>().material.mainTexture = mapData.bitmap;
        inputMap.GetComponent<MeshRenderer>().material.SetTexture("Texture2D_3f3542f4b23c413d97123944acaa3ef7", mapData.bitmap);
        inputMap.GetComponent<MeshRenderer>().material.GetTexture("Texture2D_3f3542f4b23c413d97123944acaa3ef7").filterMode = FilterMode.Point;
        inputMap.transform.localScale = new Vector3(transform.localScale.x * ar, 1, transform.localScale.y);
    }

    public void SetSwimmingArea(List<Vector2Int> pos)
    {
        swimmingAreas = pos;
    }

    private void GenerateMap(Texture2D bitMap)
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

        tex = new Texture2D(bitMap.width, bitMap.height);
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

    public void ShowUIInputMap()
    {
        _satelliteQuad.gameObject.SetActive(true);
    }

    public void HideUIInputMap()
    {
        _satelliteQuad.gameObject.SetActive(false);
    }

    public void PlaceDrones()
    {
        GenerateMap(mapData.bitmap);
        StartCoroutine(PlaceDronesRoutine());
        FindAnyObjectByType<DroneManager>().AddWaypoints(_wayPoints);
    }

    public IEnumerator PlaceDronesRoutine()
    {
        if (centroids.Count == 0)
            yield break;

        loadingBar.Show();
        loadingBar.UpdateProgress("Adding drones and chargers...", 0, 3);

        yield return 0;

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

        loadingBar.UpdateProgress("Drawing region values...", 1, 3);

        yield return 0;

        for (int x = 0; x < mapData.bitmap.width; x++)
            for (int y = 0; y < mapData.bitmap.height; y++)
            {
                Color wholeColor = new Color(Mathf.RoundToInt(mapData.bitmap.GetPixel(x, y).r), Mathf.RoundToInt(mapData.bitmap.GetPixel(x, y).g), Mathf.RoundToInt(mapData.bitmap.GetPixel(x, y).b), 1);
                if (wholeColor == Color.red)
                    voronoiDiagram.SetPixel(x, y, Color.red);
                if (wholeColor == Color.black)
                    voronoiDiagram.SetPixel(x, y, Color.black);
            }

        foreach (Vector2Int v in swimmingAreas)
        {
            Color wholeColor = new Color(Mathf.RoundToInt(mapData.bitmap.GetPixel(v.x, v.y).r), Mathf.RoundToInt(mapData.bitmap.GetPixel(v.x, v.y).g), Mathf.RoundToInt(mapData.bitmap.GetPixel(v.x, v.y).b), 1);
            if (wholeColor == Color.blue)
                voronoiDiagram.SetPixel(v.x, v.y, swimmingAreaColor);
        }

        quad.material.mainTexture = voronoiDiagram;
        voronoiDiagram.Apply();


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
                {
                    drones[i].SetTraversableCells(traversableAreas[i]);
                }
            }

        loadingBar.UpdateProgress("Assigning traversable areas...", 2, 3);

        yield return 0;

        droneHUD.gameObject.SetActive(true);
        traversableAreaCount = traversableAreas.SelectMany(list => list).Distinct().Count();
        var icons = FindObjectsByType<UIMapIcon>(FindObjectsSortMode.None).ToList();
        icons.ForEach(x => Destroy(x.gameObject));
        OnMapGenerationComplete(chargerLocations);

        loadingBar.UpdateProgress("Simulation loaded...", 3, 3);
        yield return new WaitForSeconds(.5f);
        loadingBar.Hide();
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

    public List<MarineDrone> GetDrones => drones;

    private void LateUpdate()
    {
        if (updateTexture)
        {
            if (voronoiDiagram != null)
            {
                voronoiDiagram.Apply();
                ot.Apply();
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
            UIMapIcon d = Instantiate(droneIcon, new Vector3(worldSpace.x, worldSpace.y + .01f, worldSpace.z), Quaternion.identity);
            droneIcons.Add(d);
            d.iconIndex = droneIcons.Count;
        }

        if (placingChargers)
        {
            UIMapIcon d = Instantiate(chargerIcon, new Vector3(worldSpace.x, worldSpace.y + .01f, worldSpace.z), Quaternion.identity);
            chargerIcons.Add(d);
            d.iconIndex = chargerIcons.Count;
        }

        if (placingWaypoints)
        {
            UIMapIcon w = Instantiate(waypointsIcon, new Vector3(worldSpace.x, worldSpace.y + .01f, worldSpace.z), Quaternion.identity);
            _waypointIcons.Add(w);
            w.iconIndex = _waypointIcons.Count;
        }
    }

    public Vector3 GetIndexPosition(Vector2Int coords)
    {
        Vector3 pos = new Vector3(mapCells[coords.x, coords.y].xPos, 0, mapCells[coords.x, coords.y].zPos);
        if (voronoiDiagram != null)
        {
            ot.SetPixel(coords.x, coords.y, cleanedColor);
            voronoiDiagram.SetPixel(coords.x, coords.y, cleanedColor);
        }
        updateTexture = true;
        return pos;
    }

    public Vector3 GetChargerPosition(Vector2Int coords)
    {
        Vector3 pos = new Vector3(mapCells[coords.x, coords.y].xPos, 0, mapCells[coords.x, coords.y].zPos);
        return pos;
    }

    public void SelectingUnreachableAreas()
    {
        placingChargers = false;
        placingDrones = false;
        placingWaypoints = false;
    }

    public void PlacingDrones()
    {
        placingChargers = false;
        placingDrones = true;
        placingWaypoints = false;
    }

    public void PlacingChargers()
    {
        placingChargers = true;
        placingDrones = false;
        placingWaypoints = false;
    }

    public void PlacingWaypoints()
    {
        placingChargers = false;
        placingDrones = false;
        placingWaypoints = true;
    }

    public void AddWaypoint(Vector2Int pos)
    {
        _wayPoints.Add(pos);
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
