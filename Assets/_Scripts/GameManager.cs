using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float timeScale = 1;
    public SimulationConfig _currentConfig;

    public bool Playing;
    public float CurrentSimTime = 0;

    public void AddConfig(SimulationConfig c)
    {
        _currentConfig = c;
    }

    private void Update()
    {
        if (Playing)
            CurrentSimTime += Time.deltaTime;
    }

    public void StartSim()
    {
        Playing = true;
        Time.timeScale = 1;
    }

    public void PauseSim()
    {
        Playing = false;
        Time.timeScale = 0;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void AddDrones(int count)
    {

    }
}

//todo specify what units each field uses (mAh, Watts, etc)
public struct SimulationConfig
{
    public SimulationConfig(string name, float batteryCapacity, float velocity, float wasteCapacity, float chargeSpeed, float costOfElectricity, int mapWidth, int mapHeight)
    {
        SimName = name;
        BatteryCapacity = batteryCapacity;
        Velocity = velocity;
        WasteCapacity = wasteCapacity;
        ChargeSpeed = chargeSpeed;
        CostOfElectricity = costOfElectricity;
        MapWidth = mapWidth;
        MapHeight = mapHeight;
    }

    public string SimName;
    public float BatteryCapacity;
    public float Velocity;
    public float WasteCapacity;
    public float ChargeSpeed;
    public float CostOfElectricity;
    public int MapWidth;
    public int MapHeight;
}
