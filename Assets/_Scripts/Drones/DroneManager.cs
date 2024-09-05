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
    public TMP_InputField SpeedText;
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
        NameText.text = "asdas";
        BatteryText.text = "100";
        CapacityText.text = "100";
        SpeedText.text = "100";

        gridManager.OnMapGenerationComplete += InitializeDrones;
    }

    private void InitializeDrones(List<Vector2Int> chargerLocations)
    {
        drones = gridManager.GetDrones;
        drones.ForEach(x =>
        {
            x.Name = NameText.text;
            x.Battery = float.Parse(BatteryText.text);
            x.Capacity = int.Parse(CapacityText.text);
            x.Speed = float.Parse(SpeedText.text);
        });

        for (int i = 0; i < chargerLocations.Count; i++)
        {
            DroneCharger dc = Instantiate(ChargerPrefab, gridManager.GetChargerPosition(chargerLocations[i]), Quaternion.identity);
            chargers.Add(dc);
        }
    }
}
