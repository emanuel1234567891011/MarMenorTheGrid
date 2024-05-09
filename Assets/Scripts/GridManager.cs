using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class GridManager : MonoBehaviour
{
    public GameManager gameManager; // Reference to the GameManager
    public MeshRenderer plane;
    public Texture2D bitMap;
    public int gridWidth = 700; // Width of the grid
    public int gridHeight = 700; // Height of the grid
    public float cellSize = 1.0f; // Grid cell size
    private float checkInterval = 1.0f; // How often to check the object's position
    private bool[,] mapValues;

    // Make cleanedGrid public
    public bool[,] cleanedGrid; // 2D array to keep track of cleaned cells

    private float cleanedPercentage = 0.0f; // Store the cleaned area percentage

    void Start()
    {
        //cleanedGrid = new bool[gridWidth, gridHeight]; // Initialize the cleaned grid
        mapValues = new bool[bitMap.height, bitMap.width];
        StartCoroutine(GetMapValues(bitMap));
    }

    void Update()
    {
        //DrawGrid();
    }

    public void StartCheckingObjectPosition()
    {
        InvokeRepeating("CheckObjectPosition", 0.0f, checkInterval);
    }

    public void StopCheckingObjectPosition()
    {
        CancelInvoke("CheckObjectPosition");
    }

    void CheckObjectPosition()
    {
        List<Drone> dronesList = gameManager.dronesList; // Get the drones list from the GameManager

        foreach (Drone drone in dronesList)
        {
            if (drone == null)
            {
                Debug.LogError("A drone in dronesList is null");
                continue; // Skip this iteration
            }

            Vector3 dronePosition = drone.transform.position;
            int gridX = Mathf.Clamp(Mathf.FloorToInt(dronePosition.x / cellSize), 0, gridWidth - 1);
            int gridZ = Mathf.Clamp(Mathf.FloorToInt(dronePosition.z / cellSize), 0, gridHeight - 1);

            cleanedGrid[gridX, gridZ] = true; // Mark the cell as cleaned
        }

        // Calculate the cleaned area percentage
        int cleanedCells = 0;
        foreach (bool cell in cleanedGrid)
        {
            if (cell) cleanedCells++;
        }
        cleanedPercentage = (float)cleanedCells / (gridWidth * gridHeight) * 100;
        Debug.Log($"Cleaned area: {cleanedPercentage}%");
    }

    //todo Get the bitmap values => List<bool>
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

        //Texture2D tex = new Texture2D(bitMap.width, bitMap.height);
        //Color[] c = new Color[bitMap.width * bitMap.height];
        //int textIndex = 0;
        //for (int i = 0; i < mapValues.GetLength(0); i++)
        //    for (int j = 0; j < mapValues.GetLength(1); j++)
        //    {
        //        if(mapValues[i,j] == true)
        //        {
        //            c[textIndex] = Color.blue;
        //        } else
        //        {
        //            c[textIndex] = Color.red;
        //        }

        //        textIndex++;
                
        //    }

        //tex.SetPixels(c);
        //tex.Apply();
        //plane.material.mainTexture = tex;
    }

    //todo Draw grid

    // Provide a public method to access the cleaned percentage
    public float GetCleanedPercentage()
    {
        return cleanedPercentage;
    }
}



