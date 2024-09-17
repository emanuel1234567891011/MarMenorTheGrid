using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SimulatorSnapshot
{
    public SimulatorSnapshot(List<DroneData> datas, float time, float totalWasteCollected)
    {
        DroneDatas = new List<DroneData>(datas);
        Time = time;
        TotalWasteCollected = totalWasteCollected;
    }

    public List<DroneData> DroneDatas = new List<DroneData>();
    public float Time;
    public float TotalWasteCollected;
}

public struct DroneData
{
    public DroneData(float batteryLevel, float wasteCollected, float energyConsumed)
    {
        BatteryLevel = batteryLevel;
        WasteCollected = wasteCollected;
        EnergyConsumed = energyConsumed;
    }

    public float BatteryLevel;
    public float WasteCollected;
    public float EnergyConsumed;
}

