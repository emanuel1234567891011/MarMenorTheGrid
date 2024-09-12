using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.Mathematics;
using NUnit.Framework.Constraints;

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
        drones.ForEach(x =>
        {
            x.Name = NameText.text + " " + UnityEngine.Random.Range(0, 1000).ToString();

            float batteryResult = 0;
            if (float.TryParse(BatteryText.text, out batteryResult))
                x.Battery = batteryResult;
            else
                x.Battery = 0;

            float capacityResult = 0;
            if (float.TryParse(CapacityText.text, out capacityResult))
                x.Capacity = capacityResult;
            else
                x.Capacity = 0;

            float speedResult = 0;
            if (float.TryParse(VelocityText.text, out speedResult))
                x.Velocity = speedResult;
            else
                x.Velocity = 0;
        });

        for (int i = 0; i < chargerLocations.Count; i++)
        {
            DroneCharger dc = Instantiate(ChargerPrefab, gridManager.GetChargerPosition(chargerLocations[i]), Quaternion.identity);
            chargers.Add(dc);
        }
    }
}
