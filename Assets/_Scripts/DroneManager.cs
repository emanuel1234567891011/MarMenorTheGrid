using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.Mathematics;

public class DroneManager : MonoBehaviour
{
    public TMP_InputField NameText;
    public TMP_InputField BatteryText;
    public TMP_InputField CapacityText;
    public TMP_InputField SpeedText;
    public TextMeshProUGUI SpaceClearedText;

    public GridManager gridManager;
    public List<Drone> drones = new List<Drone>();

    int clearedSpaces = 0;

    public void SpaceCleared()
    {
        clearedSpaces++;
        if (gridManager.GetTraversableAreaCount > 0)
        {
            float pctComplete = (float)clearedSpaces / (float)gridManager.GetTraversableAreaCount;
            SpaceClearedText.text = "Area cleared: " + (float)Math.Round(pctComplete, 4) + "%";
            Debug.Log(pctComplete + " " + Math.Round(pctComplete, 2));

        }
    }

    private void Start()
    {
        gridManager.OnMapGenerationComplete += InitializeDrones;
    }

    private void InitializeDrones()
    {
        drones = gridManager.GetDrones;
        drones.ForEach(x =>
        {
            x.Name = NameText.text;
            x.Battery = float.Parse(BatteryText.text);
            x.Capacity = int.Parse(CapacityText.text);
            x.Speed = float.Parse(SpeedText.text);
        });
    }

    //todo transfer logic from grid manager after it's working.
}
