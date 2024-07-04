using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DroneHUDController : MonoBehaviour
{

    public Slider timeScaleSlider;
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
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = timeScaleSlider.value;
    }

    private void Update()
    {
        var ts = TimeSpan.FromSeconds(GameManager.Instance.CurrentSimTime);
        simulationTime.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        timeScaleText.text = Time.timeScale.ToString();
    }

    public void PopulateDroneInfo(string name, float capacity, float battery, float velocity)
    {
        droneName.text = name;
        droneCapacity.text = capacity.ToString();
        droneBattery.text = battery.ToString();
        droneVelocity.text = velocity.ToString();
    }

    public void Clear()
    {
        droneName.text = "--";
        droneCapacity.text = "--";
        droneBattery.text = "--";
        droneVelocity.text = "--";
    }
}
