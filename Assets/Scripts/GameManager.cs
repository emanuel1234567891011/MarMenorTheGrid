using UnityEngine;
using TMPro; // Namespace for TextMeshPro
using System.Collections.Generic;
using UnityEngine.UI; // Required for UI components like Button

public class GameManager : MonoBehaviour
{
    public Camera myCamera;
    private GameObject selectedObject; // The currently selected object
    private Color originalColor; // The original color of the selected object

    public GameObject dronePrefab; // The drone prefab
    public List<Drone> dronesList = new List<Drone>();
    
    public TextMeshProUGUI timerText; // Reference to the TextMeshProUGUI component for displaying the timer
    public TextMeshProUGUI batteryLifeText; // Reference to the TextMeshProUGUI for displaying battery life
    public TextMeshProUGUI cleanedAreaText; // ADDITION: Reference for displaying the cleaned area percentage
    public TMP_InputField nameInputField;
    public TMP_InputField capacityInputField;
    public TMP_InputField batteryInputField;
    public TMP_InputField velocityMeanInputField;
    public TextMeshProUGUI droneListText;
    public TMP_InputField copiesInputField; // Assign in the Unity Editor
    public Button addButton; // Assign in the Unity Editor
    public float distance = 2.0f; // Distance between each instantiated object

    private List<GameObject> targetList;
    private float timer = 0.0f;
    private bool isTimerRunning = false;
    private GridManager gridManager; // Reference to the GridManager

    void Start()
    {
        gridManager = UnityEngine.Object.FindFirstObjectByType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene!");
        }
        targetList = new List<GameObject>(Resources.LoadAll<GameObject>("Target"));
        addButton.onClick.AddListener(OnAddButtonClicked);
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            timerText.text = $"Time: {timer:F2}";
            UpdateBatteryLifeDisplay();
            if (gridManager != null)
            {
                float cleanedPercentage = gridManager.GetCleanedPercentage();
                cleanedAreaText.text = $"Cleaned Area: {cleanedPercentage:F8}%";
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;  
            Ray myRay = myCamera.ScreenPointToRay(mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(myRay, out raycastHit))
            {
                selectedObject = raycastHit.transform.gameObject;
                MeshRenderer renderer = selectedObject.GetComponent<MeshRenderer>();
                originalColor = renderer.material.color;
                renderer.material.color = Color.red;
            }
        }
        if (Input.GetMouseButtonUp(0) && selectedObject != null)
        {
            selectedObject.GetComponent<MeshRenderer>().material.color = originalColor;
            selectedObject = null;
        }
    }

    private void UpdateBatteryLifeDisplay()
    {
        if (dronesList.Count > 0)
        {
            float totalBatteryTime = 0;
            foreach (Drone drone in dronesList)
            {
                totalBatteryTime += drone.Battery;
            }
            float averageBatteryTime = totalBatteryTime / dronesList.Count;
            batteryLifeText.text = $"Battery Time: {averageBatteryTime:F2} seconds";
        }
        else
        {
            batteryLifeText.text = "Average Battery Time: N/A";
        }
    }

    public void StartSimulation()
    {
        foreach (Drone drone in dronesList)
        {
            drone.StartWork();
        }
        StartTimer();
    }

    public void StopSimulation()
    {
        foreach (Drone drone in dronesList)
        {
            drone.StopWork();
        }
        StopTimer();
    }

    public void AddDrone()
    {
        GameObject droneObject = Instantiate(dronePrefab);
        Drone newDrone = droneObject.GetComponent<Drone>();
        newDrone.Name = nameInputField.text;
        newDrone.Speed = float.Parse(velocityMeanInputField.text);
        newDrone.Battery = float.Parse(batteryInputField.text);
        newDrone.Capacity = int.Parse(capacityInputField.text);
        dronesList.Add(newDrone);
        UpdateDroneListUI();
    }

    private void StartTimer()
    {
        isTimerRunning = true;
        timer = 0.0f;
    }

    private void StopTimer()
    {
        isTimerRunning = false;
    }

    private void UpdateDroneListUI()
    {
        droneListText.text = "";
        foreach (Drone drone in dronesList)
        {
            droneListText.text += drone.ToString() + "\n";
        }
    }

    public float GetTimerValue()
    {
        return timer;
    }

    void OnAddButtonClicked()
    {
        string inputName = nameInputField.text;
        int numberOfCopies = int.Parse(copiesInputField.text);
        GameObject prefab = targetList.Find(p => p.name == inputName);
        if (prefab != null)
        {
            Vector3 position = Vector3.zero;
            for (int i = 0; i < numberOfCopies; i++)
            {
                GameObject newObj = Instantiate(prefab, position, Quaternion.identity);
                position += new Vector3(distance, 0, 0);
            }
        }
        else
        {
            Debug.LogError("Prefab not found");
        }
    }
}
