using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject objectToTrack; // Assign in inspector
    public int gridWidth = 700; // Width of the grid
    public int gridHeight = 700; // Height of the grid
    public float cellSize = 1.0f; // Grid cell size
    private float checkInterval = 1.0f; // How often to check the object's position
    private bool[,] cleanedGrid; // 2D array to keep track of cleaned cells
    private float cleanedPercentage = 0.0f; // Store the cleaned area percentage

    void Start()
    {
        if (objectToTrack == null)
        {
            Debug.LogError("Object to track is not assigned!");
            return;
        }

        cleanedGrid = new bool[gridWidth, gridHeight]; // Initialize the cleaned grid
        InvokeRepeating("CheckObjectPosition", 0.0f, checkInterval);
    }

    void Update()
    {
        DrawGrid();
    }

    void CheckObjectPosition()
    {
        Vector3 objectPosition = objectToTrack.transform.position;
        int gridX = Mathf.Clamp(Mathf.FloorToInt(objectPosition.x / cellSize), 0, gridWidth - 1);
        int gridZ = Mathf.Clamp(Mathf.FloorToInt(objectPosition.z / cellSize), 0, gridHeight - 1);

        cleanedGrid[gridX, gridZ] = true; // Mark the cell as cleaned

        // Calculate the cleaned area percentage
        int cleanedCells = 0;
        foreach (bool cell in cleanedGrid)
        {
            if (cell) cleanedCells++;
        }
        cleanedPercentage = (float)cleanedCells / (gridWidth * gridHeight) * 100;

        Debug.Log($"Object is at grid position: {gridX}, {gridZ}");
        Debug.Log($"Cleaned area: {cleanedPercentage}%");
    }

    void DrawGrid()
    {
        for (int x = 0; x <= gridWidth; x++)
        {
            for (int z = 0; z <= gridHeight; z++)
            {
                // Vertical lines
                Debug.DrawLine(new Vector3(x * cellSize, 0, 0), new Vector3(x * cellSize, 0, gridHeight * cellSize), Color.blue);
                // Horizontal lines
                Debug.DrawLine(new Vector3(0, 0, z * cellSize), new Vector3(gridWidth * cellSize, 0, z * cellSize), Color.blue);
            }
        }
    }

    // Provide a public method to access the cleaned percentage
    public float GetCleanedPercentage()
    {
        return cleanedPercentage;
    }
}
