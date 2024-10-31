using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimSpecController : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] TMP_InputField numOfDronesDP;
    [SerializeField] TMP_InputField costPerDrone;
    [SerializeField] TMP_InputField wattsPerHr;
    [SerializeField] TMP_InputField operationalHrs;
    [SerializeField] TMP_InputField operationalCost;
    [SerializeField] TMP_InputField maintenanceCost;
    [SerializeField] TMP_InputField numOfDronesM;
    [SerializeField] TMP_InputField personnel;
    [SerializeField] TMP_InputField insurance;
    [SerializeField] TMP_InputField dataStorage;
    [SerializeField] TMP_InputField software;

    [Header("Total Texts")]
    [SerializeField] TextMeshProUGUI _initialTotalInvestmentsCost;
    [SerializeField] TextMeshProUGUI _droneProcurementCost;
    [SerializeField] TextMeshProUGUI _chargingStations;
    [SerializeField] TextMeshProUGUI _monthlyTotalCost;
    [SerializeField] TextMeshProUGUI _energyCost;
    [SerializeField] TextMeshProUGUI _maintenanceCost;
    [SerializeField] TextMeshProUGUI _personnelCost;
    [SerializeField] TextMeshProUGUI _miscCost;
    [SerializeField] TextMeshProUGUI _totalMonthly;

    public void SetNumOfDrones()
    {
        numOfDronesDP.text = "1";
        numOfDronesM.text = "1";
    }
}
