using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class AbstractMap : MonoBehaviour
{
    public GameManager gameManager;
    public Texture2D texture;
    private Vector3 blueLastPosition;
    private Color currentColor = Color.blue;
    private Color[] pixels;
    private int[,] binaryMatrix; // Binary matrix to represent land and sea

    void Start()
    {
        if (texture.isReadable)
        {
            Debug.Log("Texture is readable.");
            texture = Instantiate(texture);
            pixels = texture.GetPixels();
            binaryMatrix = new int[gameManager.gridManager.gridWidth, gameManager.gridManager.gridHeight]; // Initialize binary matrix

            // Manually set all cells to be land
            for (int x = 0; x < binaryMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < binaryMatrix.GetLength(1); y++)
                {
                    binaryMatrix[x, y] = 0;
                }
            }

            // Set specific cells to be sea based on your requirements
            // binaryMatrix[x, y] = 1;
        }
        else
        {
            Debug.LogError("Texture is not readable. Please check import settings.");
        }
    }

    void Update()
    {
        // Get cleanedGrid from GridManager instance
        bool[,] cleanedGrid = gameManager.gridManager.cleanedGrid;

        // Get drone list from game manager
        List<Drone> drones = gameManager.dronesList;

        // Color cells based on drone positions
        foreach (Drone drone in drones)
        {
            Vector3 dronePosition = drone.transform.position;
            int cellX = (int)(dronePosition.x / gameManager.gridManager.gridWidth);
            int cellY = (int)(dronePosition.y / gameManager.gridManager.gridHeight);
            ColorCell(cellX, cellY, Color.blue); // Color cell with blue color
        }

        // Color cells based on binary matrix
        for (int x = 0; x < binaryMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < binaryMatrix.GetLength(1); y++)
            {
                if (binaryMatrix[x, y] == 1) // Sea
                {
                    ColorCell(x, y, new Color(1, 0.5f, 0, 0.25f)); // Color cell with semi-transparent orange color
                }
            }
        }

        DrawGrid(new Color(1f, 1f, 1f, 0.1f));
        texture.SetPixels(pixels);
        texture.Apply();
    }

    void OnApplicationQuit()
    {
        SaveTextureAsPNG(texture, "/home/emanuel/MarMenorTheGrid/Assets/Results/NavigatedArea.png");
    }

    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_fullPath));
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        Debug.Log($"{_bytes.Length / 1024} Kb was saved as: {_fullPath}");
    }

    void DrawGrid(Color color)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (x % gameManager.gridManager.gridWidth == 0 || y % gameManager.gridManager.gridHeight == 0)
                {
                    pixels[y * texture.width + x] = color;
                }
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    void ColorCell(int cellX, int cellY, Color color)
    {
        // Calculate the pixel coordinates of the top left corner of the cell
        int startX = cellX * gameManager.gridManager.gridWidth;
        int startY = cellY * gameManager.gridManager.gridHeight;

        // Color the pixels within the cell
        for (int x = startX; x < startX + gameManager.gridManager.gridWidth && x < texture.width; x++)
        {
            for (int y = startY; y < startY + gameManager.gridManager.gridHeight && y < texture.height; y++)
            {
                pixels[y * texture.width + x] = color;
            }
        }
    }
}