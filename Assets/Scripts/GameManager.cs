using UnityEngine;
using TMPro; // Namespace for TextMeshPro
using System.Collections.Generic;

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

    private float timer = 0.0f;
    private bool isTimerRunning = false;
    private GridManager gridManager; // Reference to the GridManager

    void Start()
    {
        // Find the GridManager in the scene using the updated method
        gridManager = UnityEngine.Object.FindFirstObjectByType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene!");
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            timerText.text = $"Time: {timer:F2}";
            UpdateBatteryLifeDisplay(); // Update the battery life display

            // ADDITION: Update the cleaned area percentage display with more decimals
            if (gridManager != null)
            {
                float cleanedPercentage = gridManager.GetCleanedPercentage();
                // Use "F8" for 8 decimal places
                cleanedAreaText.text = $"Cleaned Area: {cleanedPercentage:F8}%";
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;  

            Ray myRay = myCamera.ScreenPointToRay(mousePosition);

            RaycastHit raycastHit; 

            bool weHitSomething = Physics.Raycast(myRay, out raycastHit); 

            if (weHitSomething) 
            {
                Debug.Log(raycastHit.transform.name); 
                selectedObject = raycastHit.transform.gameObject;
                MeshRenderer renderer = selectedObject.GetComponent<MeshRenderer>();
                originalColor = renderer.material.color; // Store the original color
                renderer.material.color = Color.red; // Change the color to red

                // Display the information from the input fields
                Debug.Log($"Name: {nameInputField.text}, Speed: {velocityMeanInputField.text}, Battery: {batteryInputField.text}, Capacity: {capacityInputField.text}");
            }
            else
            {
                Debug.Log("We do not hit anything"); 
            }
        }

        if (Input.GetMouseButtonUp(0) && selectedObject != null)
        {
            // Change the color back to the original color when the mouse button is released
            selectedObject.GetComponent<MeshRenderer>().material.color = originalColor;
            selectedObject = null; // Clear the selected object
        }
    }

    private void UpdateBatteryLifeDisplay()
    {
        // Logic for calculating and displaying battery life
        if (dronesList.Count > 0)
        {
            float totalBatteryTime = 0;
            foreach (Drone drone in dronesList)
            {
                totalBatteryTime += drone.Battery; // Assuming Drone class has a Battery property
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
            drone.StartWork(); // Assuming Drone class has a StartWork method
        }
        StartTimer();
    }

    public void StopSimulation()
    {
        foreach (Drone drone in dronesList)
        {
            drone.StopWork(); // Assuming Drone class has a StopWork method
        }
        StopTimer();
    }

    public void AddDrone()
    {
        // Instantiate the drone prefab and set its properties
        GameObject droneObject = Instantiate(dronePrefab);
        Drone newDrone = droneObject.GetComponent<Drone>();
        newDrone.Name = nameInputField.text;
        newDrone.Speed = float.Parse(velocityMeanInputField.text);
        newDrone.Battery = float.Parse(batteryInputField.text);
        newDrone.Capacity = int.Parse(capacityInputField.text);
        dronesList.Add(newDrone);

        // Print the properties of the new drone to the console
        Debug.Log($"Added new drone: Name = {newDrone.Name}, Speed = {newDrone.Speed}, Battery = {newDrone.Battery}, Capacity = {newDrone.Capacity}");

        UpdateDroneListUI(); // Update the UI with the new drone
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
        // Update the UI to display the list of drones
        droneListText.text = ""; // Clear current list
        foreach (Drone drone in dronesList)
        {
            droneListText.text += drone.ToString() + "\n"; // Assuming Drone class has a ToString override
        }
    }

    public float GetTimerValue()
    {
        return timer;
    }
}