using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Pathfinding;
using Unity.VisualScripting;

public class GridManager : MonoBehaviour
{
    [Header("Map Data")]
    public MeshRenderer quad;
    public Texture2D bitMap;
    private bool[,] mapValues;

    [Header("Game Grid")]
    public int gridXLength = 10;
    private MapCellData[,] mapCells;
    private float ar;

    public Transform _debugBlock;

    void Start()
    {
        //? Create the arrays and set their total lengths.
        mapValues = new bool[bitMap.height, bitMap.width];
        mapCells = new MapCellData[mapValues.GetLength(0), mapValues.GetLength(1)];

        //? Set the aspect ratio.
        ar = (float)bitMap.width / bitMap.height;
        quad.transform.localScale = new Vector3(gridXLength * ar, gridXLength);
        Vector3 offsetPos = new Vector3(-quad.GetComponent<MeshRenderer>().bounds.size.x / 2, 0, quad.GetComponent<MeshRenderer>().bounds.size.z / 2);
        quad.transform.position = offsetPos;

        StartCoroutine(GetMapValues(bitMap));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked.");
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                Debug.Log($"Hit {hit.collider.gameObject.name}");
            }
            else
            {
                Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.TransformDirection(Vector3.down) * 100);
                Debug.Log("Hit nothing.");
            }
        }
    }

    //? Getting the pixels from the bitmap (red and blue) and determining if each pixel is water or not.
    //? Then, iterating through the array (1D) of colors and creating a 2D array of booleans to represent the water.
    //? Finaly, creating a new texture out of that data, mostly for debug reasons.
    private IEnumerator GetMapValues(Texture2D bitMap)
    {
        Color[] colors = bitMap.GetPixels();
        int xDimesnion = bitMap.width;
        int zDimension = bitMap.height;

        Debug.Log($"Color length: {colors.Length}");

        int mapIndex = 0;
        for (int i = 0; i < bitMap.height; i++)
            for (int j = 0; j < bitMap.width; j++)
            {
                mapValues[i, j] = colors[mapIndex].b == 1.00 ? true : false;
                mapIndex++;
            }

        yield return 0;

        Texture2D tex = new Texture2D(bitMap.width, bitMap.height);
        Color[] c = new Color[bitMap.width * bitMap.height];
        int textIndex = 0;
        for (int i = 0; i < mapValues.GetLength(0); i++)
            for (int j = 0; j < mapValues.GetLength(1); j++)
            {
                if (mapValues[i, j] == true)
                    c[textIndex] = Color.blue;
                else
                    c[textIndex] = Color.red;

                textIndex++;

            }

        tex.SetPixels(c);
        tex.Apply();
        quad.material.mainTexture = tex;

        //! debug remove
        _debugBlock.transform.localScale = new Vector3((float)gridXLength / (float)mapCells.GetLength(0), 0, (float)gridXLength * (float)ar / (float)mapCells.GetLength(1));
        StartCoroutine(CreateGrid());
    }

    //? Creating the grid positions and water data here by getting the grid size (grid length / cell count) then iterating through the map cells and assigning each grid location.
    //? Also assigning the water value based on the same grid location in the mapValues array which store the boolean data from the bitmap.
    private IEnumerator CreateGrid()
    {
        float gridPosSpace = (float)gridXLength / (float)bitMap.width;

        for (int i = 0; i < mapValues.GetLength(0); i++)
            for (int j = 0; j < mapValues.GetLength(1); j++)
            {
                Vector2 pos = new Vector2(gridPosSpace * i, gridPosSpace * j);
                mapCells[i, j].xPos = pos.x;
                mapCells[i, j].zPos = pos.y;
                mapCells[i, j].isWater = mapValues[i, j];
            }

        yield return null;

        StartCoroutine(StepThrough());

    }

    //? Gizmos are 3D Editor GUI tools used to visual data and objects.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Vector3.zero, .25f);
        // if (Application.isPlaying == false)
        //     return;

        // Gizmos.color = Color.green;
        // if (mapCells.GetLength(0) > 0)
        // {
        //     for (int i = mapCells.GetLength(0) - 1; i >= 0; i--)
        //         for (int j = 0; j < mapCells.GetLength(1); j++)
        //             Gizmos.DrawCube(new Vector3(mapCells[i, j].xPos, 0, mapCells[i, j].zPos), new Vector3(gridXLength / mapCells.GetLength(0), 0, gridXLength * ar / mapCells.GetLength(1)));
        // }
    }

    private IEnumerator StepThrough()
    {
        //todo need to figure out why the grid is expanding in the negative z rather than positive.
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

//? A struct is like a class but is much more efficient and can only represent data, cannot hold any functionality.
public struct MapCellData
{
    public float xPos;
    public float zPos;
    public bool isWater;
}



