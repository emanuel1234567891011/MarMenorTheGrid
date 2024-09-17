using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEditor.Rendering;

public class DefaultValues : MonoBehaviour
{
    public TMP_InputField droneNameText;
    public string droneName = "explorer";
    public TMP_InputField droneVelocityText;
    public float droneVelocity = 2.5f;
    public TMP_InputField droneWasteCapacityText;
    public float droneBatteryCapacity = 5000;
    public TMP_InputField dronebatteryCapacityText;
    public float droneWasteCapacity = 100;
    public TMP_InputField chargerChargeSpeedText;
    public float chargerSpeed = 1500;
    public TMP_InputField _costOfElectricityText;
    public float _costOfElectricity = .25f;
    public TMP_InputField _mapWidthText;
    public int _mapWidth = 24;
    public int _mapHeight = 12;
    public TMP_InputField _mapHeightText;

    public TMP_InputField[] inputFields;
    int currentField = 0;

    void Start()
    {
        droneNameText.text = droneName;
        droneVelocityText.text = droneVelocity.ToString();
        droneWasteCapacityText.text = droneWasteCapacity.ToString();
        dronebatteryCapacityText.text = droneBatteryCapacity.ToString();
        chargerChargeSpeedText.text = chargerSpeed.ToString();
        inputFields[0].Select();
        _mapWidthText.text = _mapWidth.ToString();
        _mapHeightText.text = _mapHeight.ToString();
        _costOfElectricityText.text = _costOfElectricity.ToString();

    }

    public void OnConfirmConfig()
    {
        string n = droneNameText.text;
        float bc = float.Parse(dronebatteryCapacityText.text);
        float v = float.Parse(droneVelocityText.text);
        float wc = float.Parse(droneWasteCapacityText.text);
        float cs = float.Parse(chargerChargeSpeedText.text);
        float col = float.Parse(_costOfElectricityText.text);
        int mw = int.Parse(_mapWidthText.text);
        int mh = int.Parse(_mapHeightText.text);

        SimulationConfig config = new SimulationConfig(n, bc, v, wc, cs, col, mw, mh);

        GameManager.Instance.AddConfig(config);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentField + 1 > inputFields.Length - 1)
                currentField = 0;
            else
                currentField++;

            inputFields[currentField].Select();
        }
    }
}
