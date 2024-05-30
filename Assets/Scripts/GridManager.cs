using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Pathfinding;
using Unity.VisualScripting;
using System.Collections.Generic;
using System;

public class GridManager : MonoBehaviour
{
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
                mapCells[i, j].xPos = offsetPos.x * 2 + i * gs;
                mapCells[i, j].zPos = offsetPos.y * 2 + j * gs;

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

        Debug.Log($"Grid took {Time.time - startTime} to finish.");
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

        //! CREATE A SQUARE IN EACH CORNER.
        //! Fix the slight offset in the grid locations.
    }
}

//todo border check.

public struct MapCellData
{
    public MapCellData(float x, float z, bool water, bool borderPosition)
    {
        xPos = x;
        zPos = z;
        isWater = water;
        isBorder = borderPosition;
    }

    public float xPos;
    public float zPos;
    public bool isWater;
    public bool isBorder;
}

