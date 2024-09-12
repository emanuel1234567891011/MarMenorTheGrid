using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

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
