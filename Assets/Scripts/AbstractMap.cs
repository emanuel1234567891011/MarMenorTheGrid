using UnityEngine;
using System.IO;

public class AbstractMap : MonoBehaviour
{
    public GameObject BlueMarineDrone;
    public Texture2D texture;
    private Vector3 blueLastPosition;
    private Color currentColor = Color.blue;
    private Color[] pixels;
    public int gridSizeX = 700; // Adjusted grid size
    public int gridSizeY = 700; // Adjusted grid size
    private float cellWidth;
    private float cellHeight;

    void Start()
    {
        blueLastPosition = BlueMarineDrone.transform.position;

        if (texture.isReadable)
        {
            Debug.Log("Texture is readable.");
            texture = Instantiate(texture);
            pixels = texture.GetPixels();
        }
        else
        {
            Debug.LogError("Texture is not readable. Please check import settings.");
        }

        cellWidth = texture.width / (float)gridSizeX;
        cellHeight = texture.height / (float)gridSizeY;

        // Call the UpdateAndDraw method every 0.5 seconds
        InvokeRepeating(nameof(UpdateAndDraw), 0f, 0.5f);
    }

    void UpdateAndDraw()
    {
        UpdateObject(BlueMarineDrone, ref blueLastPosition, currentColor);
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

        // Check if the object has moved at least one unit
        if (Mathf.Abs(currentPosition.x - lastPos.x) >= 1 || Mathf.Abs(currentPosition.z - lastPos.z) >= 1)
        {
            int currentCellX = Mathf.FloorToInt(currentPosition.x / cellWidth);
            int currentCellZ = Mathf.FloorToInt(currentPosition.z / cellHeight);

            for (int i = currentCellX * Mathf.RoundToInt(cellWidth); i < (currentCellX + 1) * Mathf.RoundToInt(cellWidth); i++)
            {
                for (int j = currentCellZ * Mathf.RoundToInt(cellHeight); j < (currentCellZ + 1) * Mathf.RoundToInt(cellHeight); j++)
                {
                    int xi = Mathf.Clamp(i, 0, texture.width - 1);
                    int zj = Mathf.Clamp(j, 0, texture.height - 1);

                    pixels[zj * texture.width + xi] = color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            Debug.Log($"Colored cell at ({currentCellX}, {currentCellZ})");

            lastPos = currentPosition;
        }
    }

    void OnApplicationQuit()
    {
        SaveTextureAsPNG(texture, "/home/emanuel/MarMenorTheGrid/Assets/Results/Output.png");
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
                if (x % Mathf.RoundToInt(cellWidth) == 0 || y % Mathf.RoundToInt(cellHeight) == 0)
                {
                    pixels[y * texture.width + x] = color;
                }
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }
}