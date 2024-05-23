using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Pathfinding;
using Unity.VisualScripting;

public class GridManager : MonoBehaviour
{
    [Header("Map Data")]
    public MeshRenderer quad;
    public Texture2D bitMap;
    private byte[,] mapValues; // 0 = land, 1 = border, 2 = water

    [Header("Game Grid")]
    public int gridXLength = 10;
    private MapCellData[,] mapCells;
    private MapCellData[,] borderCells;
    private float ar;

    public Transform _debugBlock;

    // CircleMover variables
    [Header("Circle Mover Settings")]
    public Transform objectToMove; // The object to be moved
    public float radius = 5.0f; // Radius of the circle
    public float speed = 2.0f; // Speed of movement
    public int points = 12; // Number of points on the circle

    private Vector3 centerPosition;
    private Vector3[] clockPositions;
    private int currentStep = 0;
    private LineRenderer lineRenderer;

    void Start()
    {
        // GridManager initialization
        mapValues = new byte[bitMap.height, bitMap.width];
        mapCells = new MapCellData[mapValues.GetLength(0), mapValues.GetLength(1)];

        // Set the aspect ratio
        ar = (float)bitMap.width / bitMap.height;
        quad.transform.localScale = new Vector3(gridXLength * ar, gridXLength);
        Vector3 offsetPos = new Vector3(-quad.GetComponent<MeshRenderer>().bounds.size.x / 2, 0, quad.GetComponent<MeshRenderer>().bounds.size.z / 2);
        quad.transform.position = offsetPos;

        StartCoroutine(GetMapValues(bitMap));

        // CircleMover initialization
        if (objectToMove == null)
        {
            Debug.LogError("Object to move is not assigned!");
            return;
        }

        // Set the center position to the center of the map on the X-Z plane
        centerPosition = new Vector3(quad.bounds.center.x, objectToMove.position.y, quad.bounds.center.z);
        clockPositions = new Vector3[points];

        // Calculate the positions on the border of the circle
        for (int i = 0; i < points; i++)
        {
            float angle = i * (360f / points) * Mathf.Deg2Rad; // Calculate the angle in radians
            clockPositions[i] = new Vector3(centerPosition.x + radius * Mathf.Cos(angle), centerPosition.y, centerPosition.z + radius * Mathf.Sin(angle));
        }

        // Initialize LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = points + 1; // One extra to close the loop
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startColor = Color.blue; // Set the start color to blue
        lineRenderer.endColor = Color.blue; // Set the end color to blue

        CreateCircle();
        StartCoroutine(MoveObject());
    }

    void CreateCircle()
    {
        for (int i = 0; i < points; i++)
        {
            lineRenderer.SetPosition(i, clockPositions[i]);
        }
        // Close the circle
        lineRenderer.SetPosition(points, clockPositions[0]);
    }

    private IEnumerator MoveObject()
    {
        while (true)
        {
            Vector3 targetPosition;

            // Move to the center
            targetPosition = centerPosition;
            yield return StartCoroutine(MoveToPosition(targetPosition));

            // Move to the clock position
            targetPosition = clockPositions[currentStep % points];
            yield return StartCoroutine(MoveToPosition(targetPosition));

            currentStep++;
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(objectToMove.position, targetPosition) > 0.01f)
        {
            objectToMove.position = Vector3.MoveTowards(objectToMove.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    // Existing GridManager methods

    private IEnumerator GetMapValues(Texture2D bitMap)
    {
        Color[] colors = bitMap.GetPixels();
        int xDimesnion = bitMap.width;
        int zDimension = bitMap.height;

        Debug.Log($"Color length: {colors.Length}");

        int mapIndex = 0;
        int borderPosition = 0;
        for (int i = 0; i < bitMap.height; i++)
            for (int j = 0; j < bitMap.width; j++)
            {
                if (colors[mapIndex] == Color.red)
                    mapValues[i, j] = 0;
                else if (colors[mapIndex] == Color.black)
                {
                    borderPosition++;
                    mapValues[i, j] = 1;
                }
                else
                    mapValues[i, j] = 2;

                mapIndex++;
            }

        Debug.Log($"Border pos: {borderPosition}");

        yield return 0;

        Texture2D tex = new Texture2D(bitMap.width, bitMap.height);
        Color[] c = new Color[bitMap.width * bitMap.height];
        int texIndex = 0;
        for (int i = 0; i < mapValues.GetLength(0); i++)
            for (int j = 0; j < mapValues.GetLength(1); j++)
            {
                switch (mapValues[i, j])
                {
                    case 0:
                        c[texIndex] = Color.red;
                        break;
                    case 1:
                        c[texIndex] = Color.black;
                        break;
                    case 2:
                        c[texIndex] = Color.blue;
                        break;
                }
                texIndex++;
            }

        tex.SetPixels(c);
        tex.Apply();
        quad.material.mainTexture = tex;

        _debugBlock.transform.localScale = new Vector3((float)gridXLength / (float)mapCells.GetLength(0), 0, (float)gridXLength * (float)ar / (float)mapCells.GetLength(1));
        StartCoroutine(CreateGrid());
    }

    private IEnumerator CreateGrid()
    {
        float gridPosSpace = (float)gridXLength / (float)bitMap.width;

        for (int i = 0; i < mapValues.GetLength(0); i++)
            for (int j = 0; j < mapValues.GetLength(1); j++)
            {
                Vector2 pos = new Vector2(gridPosSpace * i, gridPosSpace * j);
                mapCells[i, j].xPos = pos.x;
                mapCells[i, j].zPos = pos.y;
                mapCells[i, j].isBorder = mapValues[i, j] == 1;
                mapCells[i, j].isWater = mapValues[i, j] == 2;
            }

        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Vector3.zero, .25f);
    }

    private IEnumerator StepThrough()
    {
        for (int i = 0; i < mapValues.GetLength(0); i++)
            for (int j = 0; j < mapCells.GetLength(1); j++)
            {
                Vector3 newPos = new Vector3(mapCells[i, j].xPos, 0, mapCells[i, j].zPos);
                _debugBlock.transform.position = newPos;
                Debug.Log($"Water value : {mapCells[i, j].isWater}");
                yield return 0;
            }
    }
}

// A struct is like a class but is much more efficient and can only represent data, cannot hold any functionality.
public struct MapCellData
{
    public float xPos;
    public float zPos;
    public bool isWater;
    public bool isBorder;
}