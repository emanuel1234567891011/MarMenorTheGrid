using UnityEngine;

public class MovementColorMapping : MonoBehaviour
{
    public GameObject BlueMarineDrone;
    public Texture2D texture;
    private Vector3 blueLastPosition;
    private Color currentColor = Color.blue;
    private Color[] pixels;
    public int gridSizeX = 40;
    public int gridSizeY = 20;
    private int[,] binaryMatrix; // Binary matrix to represent land and sea

    void Start()
    {
        blueLastPosition = BlueMarineDrone.transform.position;

        if (texture.isReadable)
        {
            Debug.Log("Texture is readable.");
            texture = Instantiate(texture);
            pixels = texture.GetPixels();
            binaryMatrix = new int[texture.width / gridSizeX, texture.height / gridSizeY]; // Initialize binary matrix

            // Manually set all cells to be land
            for (int x = 0; x < binaryMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < binaryMatrix.GetLength(1); y++)
                {
                    binaryMatrix[x, y] = 0;
                }
            }

            // Set three specific cells to be land
            binaryMatrix[18, 1] = 1;
            binaryMatrix[17, 1] = 1; 
            binaryMatrix[16, 1] = 1;
            binaryMatrix[15, 1] = 1;
            binaryMatrix[14, 1] = 1; // increase y by 1 
            binaryMatrix[14, 2] = 1; 
            binaryMatrix[14, 3] = 1;
            binaryMatrix[13, 3] = 1;
            binaryMatrix[13, 4] = 1;
            binaryMatrix[12, 3] = 1;
            binaryMatrix[12, 4] = 1;
            binaryMatrix[12, 5] = 1;
            binaryMatrix[11, 5] = 1;
            binaryMatrix[11, 6] = 1;
            binaryMatrix[10, 6] = 1;
            binaryMatrix[10, 7] = 1;
            binaryMatrix[10, 8] = 1; 
            binaryMatrix[9, 8] = 1;
            binaryMatrix[9, 9] = 1;
            binaryMatrix[9, 10] = 1; 
            binaryMatrix[8, 10] = 1;
            binaryMatrix[8, 11] = 1;
            binaryMatrix[8, 12] = 1; 
            binaryMatrix[8, 13] = 1;
            binaryMatrix[7, 13] = 1;
            binaryMatrix[7, 14] = 1;
            binaryMatrix[7, 15] = 1;
            binaryMatrix[7, 16] = 1;
            binaryMatrix[7, 17] = 1; 
            binaryMatrix[7, 18] = 1;
            binaryMatrix[6, 18] = 1;
            binaryMatrix[6, 19] = 1;
            binaryMatrix[6, 20] = 1;
            binaryMatrix[6, 21] = 1;
            binaryMatrix[6, 22] = 1;
            binaryMatrix[6, 23] = 1;
            binaryMatrix[5, 23] = 1;
            binaryMatrix[5, 24] = 1;
            binaryMatrix[5, 25] = 1;
            binaryMatrix[5, 26] = 1;
            binaryMatrix[5, 27] = 1;
            binaryMatrix[5, 28] = 1; 
            binaryMatrix[5, 29] = 1;
            binaryMatrix[6, 28] = 1; 
            binaryMatrix[6, 29] = 1;
            binaryMatrix[6, 30] = 1;
            binaryMatrix[7, 30] = 1;
            binaryMatrix[7, 31] = 1;
            binaryMatrix[7, 32] = 1;
            binaryMatrix[7, 33] = 1; 
            binaryMatrix[8, 33] = 1; 
            binaryMatrix[9, 33] = 1;
            binaryMatrix[9, 34] = 1; 
            binaryMatrix[9, 35] = 1;
            binaryMatrix[10, 35] = 1;
            binaryMatrix[10, 36] = 1; 
            binaryMatrix[11, 36] = 1;
            binaryMatrix[11, 37] = 1;
            binaryMatrix[11, 38] = 1; 
            binaryMatrix[12, 38] = 1;
            binaryMatrix[12, 39] = 1;
            binaryMatrix[12, 40] = 1;
            binaryMatrix[12, 41] = 1; 
            binaryMatrix[13, 41] = 1;
            binaryMatrix[13, 42] = 1;
            binaryMatrix[13, 43] = 1;
            binaryMatrix[13, 44] = 1; 
            binaryMatrix[13, 45] = 1;
            binaryMatrix[13, 46] = 1; 
            binaryMatrix[14, 46] = 1;
            binaryMatrix[15, 46] = 1;
            binaryMatrix[15, 47] = 1; 
            binaryMatrix[16, 47] = 1;
            binaryMatrix[17, 47] = 1;
            binaryMatrix[18, 47] = 1; // decrease y by 1 
            binaryMatrix[18, 46] = 1; 
            binaryMatrix[18, 45] = 1;
            binaryMatrix[18, 44] = 1;
            binaryMatrix[18, 43] = 1;
            binaryMatrix[18, 42] = 1;
            binaryMatrix[18, 41] = 1;
            binaryMatrix[18, 40] = 1;
            binaryMatrix[18, 39] = 1;
            binaryMatrix[18, 38] = 1;
            binaryMatrix[18, 37] = 1;
            binaryMatrix[18, 36] = 1;
            binaryMatrix[18, 35] = 1;
            binaryMatrix[18, 34] = 1;
            binaryMatrix[18, 33] = 1;
            binaryMatrix[18, 32] = 1;
            binaryMatrix[18, 31] = 1;
            binaryMatrix[18, 30] = 1;
            binaryMatrix[18, 29] = 1;
            binaryMatrix[18, 28] = 1;
            binaryMatrix[18, 27] = 1;
            binaryMatrix[18, 26] = 1;
            binaryMatrix[18, 25] = 1;
            binaryMatrix[18, 24] = 1;
            binaryMatrix[18, 23] = 1;
            binaryMatrix[17, 23] = 1;
            binaryMatrix[17, 22] = 1;
            binaryMatrix[17, 21] = 1;
            binaryMatrix[17, 20] = 1;
            binaryMatrix[17, 19] = 1;
            binaryMatrix[17, 18] = 1;
            binaryMatrix[17, 17] = 1;
            binaryMatrix[17, 16] = 1;
            binaryMatrix[17, 15] = 1;
            binaryMatrix[17, 14] = 1;
            binaryMatrix[17, 13] = 1;
            binaryMatrix[17, 12] = 1;
            binaryMatrix[17, 11] = 1;
            binaryMatrix[17, 10] = 1;
            binaryMatrix[17, 9] = 1;
            binaryMatrix[18, 9] = 1; 
            binaryMatrix[18, 8] = 1;
            binaryMatrix[18, 7] = 1;
            binaryMatrix[18, 6] = 1;
            binaryMatrix[18, 5] = 1;
            binaryMatrix[18, 4] = 1;
            binaryMatrix[18, 3] = 1;
            binaryMatrix[18, 2] = 1; 
        InvokeRepeating("UpdateMethod", 0.5f, 0.5f);
        }
        else
        {
            Debug.LogError("Texture is not readable. Please check import settings.");
        }
    }

    void UpdateMethod()
    {
        UpdateObject(BlueMarineDrone, ref blueLastPosition, currentColor);

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

    void UpdateObject(GameObject obj, ref Vector3 lastPos, Color color)
    {
        // Check if the GameObject is null before accessing its properties
        if (obj == null)
        {
            Debug.LogWarning("GameObject is null.");
            return;
        }

        Vector3 currentPosition = obj.transform.position;

        if (Vector3.Distance(currentPosition, lastPos) > 0.01f)
        {
            int xOffset = 625;
            int zOffset = 60;

            int x = Mathf.Clamp(Mathf.FloorToInt(obj.transform.position.x) + xOffset, 0, texture.width - 1);
            int z = Mathf.Clamp(Mathf.FloorToInt(obj.transform.position.z) + zOffset, 0, texture.height - 1);

            int thickness = 1;

            for (int i = -thickness; i <= thickness; i++)
            {
                for (int j = -thickness; j <= thickness; j++)
                {
                    int xi = Mathf.Clamp(x + i, 0, texture.width - 1);
                    int zj = Mathf.Clamp(z + j, 0, texture.height - 1);

                    pixels[zj * texture.width + xi] = color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            Debug.Log($"Colored pixel at ({x}, {z})");
        }

        lastPos = currentPosition;
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
                if (x % gridSizeX == 0 || y % gridSizeY == 0)
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
        int startX = cellX * gridSizeX;
        int startY = cellY * gridSizeY;

        // Color the pixels within the cell
        for (int x = startX; x < startX + gridSizeX && x < texture.width; x++)
        {
            for (int y = startY; y < startY + gridSizeY && y < texture.height; y++)
            {
                pixels[y * texture.width + x] = color;
            }
        }
    }
}