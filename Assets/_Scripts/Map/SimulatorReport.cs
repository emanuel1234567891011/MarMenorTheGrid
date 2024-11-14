using System.Collections;
using System.Collections.Generic;
using E2C;
using UnityEngine;

public class SimulatorReport : MonoBehaviour
{
    public CanvasGroup graphCG;

    public float SnapShotInterval = 1;
    float _currentShotTimer = 0;
    List<SimulatorSnapshot> snaps = new List<SimulatorSnapshot>();

    //? cost = (battery consumption cost per hour * hours) * dronecount
    public float wattsPerHour = 1;
    public float costPerWatt = .5f;
    E2Chart wasteCollectedChart;
    E2ChartData.Series wasteCollectedSeries = new E2ChartData.Series();
    E2ChartData.Series batteryConsumedSeries = new E2ChartData.Series();


    private DroneManager _droneManager;

    void Update()
    {
        if (FindAnyObjectByType<GameManager>().Playing)
        {
            _currentShotTimer += Time.deltaTime;
            if (_currentShotTimer > SnapShotInterval)
            {
                List<DroneData> datas = new List<DroneData>();
                foreach (Drone drone in FindAnyObjectByType<DroneManager>().drones)
                {
                    DroneData d = new DroneData(drone.Battery, drone.WasteCollected, drone.EnergyConsumed);
                    datas.Add(d);
                }

                SimulatorSnapshot snap = new SimulatorSnapshot(datas, FindAnyObjectByType<GameManager>().CurrentSimTime, GetPercentageCleaned());
                snaps.Add(snap);
                float w = Random.Range(0, 100);
                wasteCollectedSeries.dataY.Add(w);
                batteryConsumedSeries.dataY.Add(Time.time);
                wasteCollectedChart.UpdateChart();
                _currentShotTimer = 0;

                CSVManager.AppendToReport(new string[3] { Time.time.ToString(), w.ToString(), Time.time.ToString() });
            }
        }
    }

    public void ShowGraph()
    {
        graphCG.alpha = 1;
    }

    public void HideGraph()
    {
        graphCG.alpha = 0;
    }

    private void Start()
    {
        wasteCollectedChart = FindAnyObjectByType<E2Chart>();
        wasteCollectedChart.chartData = wasteCollectedChart.gameObject.AddComponent<E2ChartData>();
        wasteCollectedChart.chartData.title = "Waste Collected";
        wasteCollectedChart.chartData.yAxisTitle = "Weight (KG)";
        wasteCollectedChart.chartData.xAxisTitle = "Time (H)";
        //myChart.chartData.categoriesX = new List<string> { "Apple", "Banana", "Cherries", "Durian", "Grapes", "Lemon" }; //set categories

        //create new series
        wasteCollectedSeries.name = "Weight Collected";
        wasteCollectedSeries.dataY = new List<float>();

        batteryConsumedSeries.name = "Battery ";
        batteryConsumedSeries.dataY = new List<float>();



        //add series into series list
        wasteCollectedChart.chartData.series = new List<E2ChartData.Series>();
        wasteCollectedChart.chartData.series.Add(wasteCollectedSeries);
        wasteCollectedChart.chartData.series.Add(batteryConsumedSeries);

        _droneManager = FindAnyObjectByType<DroneManager>();
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
