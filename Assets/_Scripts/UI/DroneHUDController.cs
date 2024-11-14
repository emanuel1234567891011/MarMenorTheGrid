using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DroneHUDController : MonoBehaviour
{
    public Slider timeScaleSlider;
    public Slider overlayMapSlider;

    public TextMeshProUGUI timeScaleText;
    public TextMeshProUGUI simulationTime;
    public TextMeshProUGUI areaCleared;

    public TextMeshProUGUI droneName;
    public TextMeshProUGUI droneCapacity;
    public TextMeshProUGUI droneBattery;
    public TextMeshProUGUI droneVelocity;

    private void Start()
    {
        timeScaleSlider.value = Time.timeScale;
        timeScaleSlider.onValueChanged.AddListener(delegate { UpdateTimeScale(); });

        overlayMapSlider.value = .5f;
        overlayMapSlider.onValueChanged.AddListener(delegate { UpdateOverlayMap(overlayMapSlider.value); });
    }

    void UpdateOverlayMap(float v)
    {
        GameObject.Find("OverlayMap").GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, v);
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = timeScaleSlider.value;
    }

    private void Update()
    {
        var ts = TimeSpan.FromSeconds(FindAnyObjectByType<GameManager>().CurrentSimTime);
        simulationTime.text = "Simulation Time: " + string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        timeScaleText.text = "Time Scale: " + Time.timeScale.ToString("#.##");
    }

    public void PopulateDroneInfo(string name, float capacity, float battery, float velocity)
    {
        droneName.text = name;
        droneCapacity.text = "Waste Capacity: " + capacity.ToString("#.##");
        droneBattery.text = "Battery Percentage: " + battery.ToString("#.##") + "%";
        droneVelocity.text = "Current Velocity: " + velocity.ToString("#.##");
    }

    public void Clear()
    {
        droneName.text = "no drone selected";
        droneCapacity.text = "";
        droneBattery.text = "";
        droneVelocity.text = "";
    }
}
