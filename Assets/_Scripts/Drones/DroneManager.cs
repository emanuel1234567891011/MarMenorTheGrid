using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
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
    public List<MarineDrone> drones = new List<MarineDrone>();
    private List<DroneCharger> chargers = new List<DroneCharger>();
    List<Vector2Int> wp = new List<Vector2Int>();
    private List<DroneConfig> droneConfigs = new List<DroneConfig>();

    int clearedSpaces = 0;

    public int GetDroneCount => drones.Count;
    public int ClearedSpace => clearedSpaces;

    public void SetDronesCleaning()
    {
        drones.ForEach(x => x.SetCleaning());
    }

    public void SetDronesPatrolling()
    {
        drones.ForEach(x => x.SetPatrolling());
    }

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
        drones = gridManager.GetDrones;
        for (int i = 0; i < chargerLocations.Count; i++)
        {
            DroneCharger dc = Instantiate(ChargerPrefab, gridManager.GetChargerPosition(chargerLocations[i]), Quaternion.identity);
            chargers.Add(dc);
        }
    }

    public void AddWaypoints(List<Vector2Int> w)
    {
        wp = new List<Vector2Int>(w);
    }

    public void AssignWaypoints()
    {
        drones = new List<MarineDrone>(FindObjectsByType<MarineDrone>(FindObjectsSortMode.None).ToList());
        foreach (MarineDrone d in drones)
            d.SetWaypoints(wp);
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
