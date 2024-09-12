using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorReport : MonoBehaviour
{
    //todo decide what variables are required to accurately calculate the requirements and draw for the drones
    //todo how to calculate the electrical usage and capacity etc.

    //? cost = (battery consumption cost per hour * hours) * dronecount
    public float wattsPerHour = 1;
    public float costPerWatt = .5f;

    private DroneManager _droneManager;

    private void Start()
    {
        _droneManager = FindAnyObjectByType<DroneManager>();
        for (int i = 0; i < 30; i++)
        {
            CSVManager.AppendToReport(new string[6] { i + 1.ToString(), Random.Range(0, 2000).ToString() + " mAh", "2.5 m/s", "100 kg", "2000 aH", ".23 c / hr" });
        }
        CSVManager.PrintReportPath();
    }

    //? cost = (battery consumption cost per hour * hours) * dronecount
    public float GetOperationCostPerHour()
    {
        return wattsPerHour * costPerWatt * _droneManager.GetDroneCount;
    }

    //? percentage cleaned = (currentCells / totalCells) * 100
    public float GetPercentageCleaned()
    {
        int totalCells = FindAnyObjectByType<GridManager>().TotalCellCount;
        int cleanedCells = FindAnyObjectByType<DroneManager>().ClearedSpace;
        return cleanedCells / totalCells * 100;
    }

    //? total duration = time value, total area of bitmap / (droneCount * metersPS)
    public float GetTotalDuration()
    {
        return 1;
    }

    //? amount of energy required to charge drones = energyRequiredPerHour * droneCount * hours

    //? capacity of waste removed? averageCellCapacity * cellCount

    //? (battery consumption? batteryConsumption per hour) * hours * droneCount
}
