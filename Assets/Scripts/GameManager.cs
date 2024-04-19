using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera myCamera;
    private GameObject selectedObject; 
    private Color originalColor;

    public GameObject dronePrefab;
    public List<Drone> dronesList = new List<Drone>();
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI batteryLifeText;
    public TMP_InputField nameInputField;
    public TMP_InputField capacityInputField;
    public TMP_InputField batteryInputField;
    public TMP_InputField velocityMeanInputField;
    public TextMeshProUGUI droneListText;
    public TMP_InputField copiesInputField;
    public Button addButton;
    public Button removeButton; // The remove button
    public float distance = 2.0f; // Distance between each instantiated object

    public GridManager gridManager; // The GridManager instance
    public TextMeshProUGUI cleanedAreaText; // The TextMeshProUGUI to display the cleaned area percentage

    private float timer = 0.0f;
    private bool isTimerRunning = false;

    void Start()
    {
        addButton.onClick.AddListener(OnAddButtonClicked);
        removeButton.onClick.AddListener(OnRemoveButtonClicked); // Add a listener for the remove button
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            timerText.text = $"Time: {timer:F2}";
            UpdateBatteryLifeDisplay();
        }

        // Get the cleaned area percentage from the GridManager and display it
        float cleanedPercentage = gridManager.GetCleanedPercentage();
        cleanedAreaText.text = $"Cleaned Area: {cleanedPercentage:F6}%";
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
        gridManager.StartCheckingObjectPosition(); // Start checking the object's position
        StartTimer();
    }

    public void StopSimulation()
    {
        foreach (Drone drone in dronesList)
        {
            drone.StopWork();
        }
        gridManager.StopCheckingObjectPosition(); // Stop checking the object's position
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

    public void RemoveDrone()
    {
        if (dronesList.Count > 0)
        {
            Drone droneToRemove = dronesList[dronesList.Count - 1]; // Get the last drone
            dronesList.Remove(droneToRemove); // Remove the drone from the drones list
            
            Destroy(droneToRemove.gameObject); // Destroy the drone game object
            UpdateDroneListUI();
        }
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
        Vector3 position = Vector3.zero; // Start position for the first drone

        for (int i = 0; i < numberOfCopies; i++)
        {
            GameObject newDroneObject = Instantiate(dronePrefab, position, Quaternion.identity);
            Drone newDrone = newDroneObject.GetComponent<Drone>();
            newDrone.Name = inputName;
            dronesList.Add(newDrone);
            
            position += new Vector3(distance, 0, 0); // Update the position for the next drone
        }
    }

    void OnRemoveButtonClicked()
    {
        RemoveDrone(); // Call the RemoveDrone method when the remove button is clicked
    }
}