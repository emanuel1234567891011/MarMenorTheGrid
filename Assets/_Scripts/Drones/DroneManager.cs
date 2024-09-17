using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class DroneManager : MonoBehaviour
{
    public DroneCharger ChargerPrefab;
    public TMP_InputField NameText;
    public TMP_InputField BatteryText;
    public TMP_InputField CapacityText;
    public TMP_InputField VelocityText;
    public TMP_InputField ChargerSpeed;
    public TextMeshProUGUI SpaceClearedText;
    public float metersPS = 5;

    public GridManager gridManager;
    public List<Drone> drones = new List<Drone>();
    private List<DroneCharger> chargers = new List<DroneCharger>();
    private List<DroneConfig> droneConfigs = new List<DroneConfig>();

    int clearedSpaces = 0;

    public int GetDroneCount => drones.Count;
    public int ClearedSpace => clearedSpaces;

    public void SpaceCleared()
    {
        clearedSpaces++;
        if (gridManager.GetTraversableAreaCount > 0)
        {
            float pctComplete = (float)clearedSpaces / (float)gridManager.GetTraversableAreaCount;
            SpaceClearedText.text = "Area cleared: " + (float)Math.Round(pctComplete, 4) + "%";
        }
    }

    private void Start()
    {
        gridManager.OnMapGenerationComplete += InitializeDrones;
    }

    private void InitializeDrones(List<Vector2Int> chargerLocations)
    {
        //todo Create a class with default settings from text fields for each drone created
        //todo Assign each new drone a class, pair it with an icon
        //todo if the icon is pressed, show panel
        //todo button press to change the info class

        drones = gridManager.GetDrones;

        //todo add configs.

        for (int i = 0; i < chargerLocations.Count; i++)
        {
            DroneCharger dc = Instantiate(ChargerPrefab, gridManager.GetChargerPosition(chargerLocations[i]), Quaternion.identity);
            chargers.Add(dc);
        }
    }

    public void AddDroneConfig(int iconIndex)
    {
        float batteryResult = float.Parse(BatteryText.text);
        float capacityResult = float.Parse(CapacityText.text);
        float velocity = float.Parse(VelocityText.text);
        DroneConfig dc = new DroneConfig(iconIndex, NameText.text + " # " + UnityEngine.Random.Range(0, 1000), batteryResult, capacityResult, velocity);
        droneConfigs.Add(dc);
    }
}

[SerializeField]
public class DroneConfig
{
    public DroneConfig(int iconIndex, string name, float battery, float capacity, float velocity)
    {
        IconIndex = iconIndex;
        Name = name;
        Battery = battery;
        Capacity = capacity;
        Velocity = velocity;
    }

    public int IconIndex = 0;
    public string Name;
    public float Battery;
    public float Capacity;
    public float Velocity;
}
