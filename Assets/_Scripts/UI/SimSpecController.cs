using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class SimSpecController : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] TMP_InputField numOfDronesDP;
    [SerializeField] TMP_InputField costPerDrone;
    [SerializeField] TMP_InputField numOfChargers;
    [SerializeField] TMP_InputField costPerCharger;
    [SerializeField] TMP_InputField wattsPerHr;
    [SerializeField] TMP_InputField costPerWatt;
    [SerializeField] TMP_InputField operationalHrs;
    [SerializeField] TMP_InputField operationalCost;
    [SerializeField] TMP_InputField maintenanceCost;
    [SerializeField] TMP_InputField numOfDronesM;
    [SerializeField] TMP_InputField insurance;
    [SerializeField] TMP_InputField dataStorage;
    [SerializeField] TMP_InputField software;

    [Header("Totals")]
    [SerializeField] TextMeshProUGUI _equipmentText;
    [SerializeField] TextMeshProUGUI _energyConsumptionText;
    [SerializeField] TextMeshProUGUI _maintenanceText;
    [SerializeField] TextMeshProUGUI _personnelText;
    [SerializeField] TextMeshProUGUI _miscText;
    [SerializeField] TextMeshProUGUI _initialInvestmentText;
    [SerializeField] TextMeshProUGUI _totalMonthlyText;
    [SerializeField] TextMeshProUGUI _totalCostText;

    Dictionary<string, string> data = new Dictionary<string, string>();

    void Start()
    {
        SetNumOfDrones();

        costPerDrone.text = "5000";
        costPerCharger.text = "1000";
        wattsPerHr.text = "1";
        costPerWatt.text = ".25";
        operationalHrs.text = "5760";
        operationalCost.text = "62.50";
        maintenanceCost.text = "200";
        insurance.text = "3600";
        dataStorage.text = "2400";
        software.text = "6000";

        numOfDronesDP.onValueChanged.AddListener((x) => UpdateValues());
        costPerDrone.onValueChanged.AddListener((x) => UpdateValues());
        numOfChargers.onValueChanged.AddListener((x) => UpdateValues());
        costPerCharger.onValueChanged.AddListener((x) => UpdateValues());
        wattsPerHr.onValueChanged.AddListener((x) => UpdateValues());
        costPerWatt.onValueChanged.AddListener((x) => UpdateValues());
        operationalHrs.onValueChanged.AddListener((x) => UpdateValues());
        operationalCost.onValueChanged.AddListener((x) => UpdateValues());
        maintenanceCost.onValueChanged.AddListener((x) => UpdateValues());
        numOfDronesM.onValueChanged.AddListener((x) => UpdateValues());
        insurance.onValueChanged.AddListener((x) => UpdateValues());
        dataStorage.onValueChanged.AddListener((x) => UpdateValues());
        software.onValueChanged.AddListener((x) => UpdateValues());

    }

    public void UpdateValues()
    {
        float droneCount = float.Parse(numOfDronesDP.text);
        float droneCost = float.Parse(costPerDrone.text);
        float chargerCount = float.Parse(numOfChargers.text);
        float chargerCost = float.Parse(costPerCharger.text);
        float wattsPH = float.Parse(wattsPerHr.text);
        float wattCost = float.Parse(costPerWatt.text);
        float operationalHours = float.Parse(operationalHrs.text);
        float operationalCst = float.Parse(operationalCost.text);
        float maintenance = float.Parse(maintenanceCost.text);
        float insuranceCost = float.Parse(insurance.text);
        float dataCost = float.Parse(dataStorage.text);
        float softwareCost = float.Parse(software.text);
        float chargerSpeed = 0; //todo add charger speed
        float chargeDuration = 0; //todo add
        float chargingSessions = 0; //todo add

        float f_droneProcurement = droneCount * droneCost;
        float f_costPerWatt = wattCost;
        float f_energyCostPerDronePerDay = wattsPH * operationalHours * wattCost;
        float f_totalDroneEnergyCostPerDay = droneCount * f_energyCostPerDronePerDay;
        float f_energyPerChargeSession = chargerSpeed * (chargeDuration / 3600);
        float f_chargingEnergyCostPerDronePerDay = f_energyPerChargeSession * droneCount;
        float f_totalChargingEnergyCostPerDay = f_energyCostPerDronePerDay * droneCount;
        float f_totalEnergyCostPerDay = f_totalDroneEnergyCostPerDay + f_totalChargingEnergyCostPerDay;
        float f_monthlyMaintenance = maintenance * droneCount;
        float f_personnel = operationalCst;
        float f_misc = softwareCost + dataCost + insuranceCost;
        float f_initialInvestment = f_droneProcurement + (chargerCost * chargerCount);
        float f_energy = chargerCount * f_totalChargingEnergyCostPerDay;
        float f_monthlyOperational = f_energy + maintenance + f_personnel + f_misc;
        float f_annualOperational = f_monthlyOperational * 12; //?
        float f_totalFirstYear = f_initialInvestment + f_annualOperational;

        float equipmentCost = (f_droneProcurement + (chargerCount * chargerCost));
        _equipmentText.text = "$" + equipmentCost.ToString();
        float energyConsumptionCost = (f_energyCostPerDronePerDay * droneCount) * 30;
        _energyConsumptionText.text = "$" + energyConsumptionCost.ToString();
        _maintenanceText.text = "$" + f_monthlyMaintenance;
        float personnel = f_personnel * 30;
        _personnelText.text = "$" + f_personnel * 30;
        _miscText.text = "$" + f_misc;
        _initialInvestmentText.text = "$" + f_initialInvestment;
        _totalMonthlyText.text = "$" + f_monthlyOperational;
        _totalCostText.text = "$" + (f_initialInvestment + f_monthlyOperational);

        data.Clear();
        data.Add("Equipment", equipmentCost.ToString());
        data.Add("Energy Consumption", energyConsumptionCost.ToString());
        data.Add("Maintenance", f_monthlyMaintenance.ToString());
        data.Add("Personnel", personnel.ToString());
        data.Add("Misc", f_misc.ToString());
        data.Add("Initial Investment", f_monthlyOperational.ToString());
        data.Add("Daily", (f_monthlyOperational / 30).ToString());
        data.Add("Monthly", f_monthlyOperational.ToString());
        data.Add("Annually", (f_monthlyOperational * 12).ToString());
    }

    public void ExportToCSV()
    {
        string[] input = { data["Equipment"], data["Energy Consumption"], data["Maintenance"], data["Personnel"], data["Misc"], data["Initial Investment"], data["Daily"], data["Monthly"], data["Annually"] };
        CSVManager.AppendToExpenseReport(input);
    }

    public void SetNumOfDrones()
    {
        numOfDronesDP.text = FindAnyObjectByType<GridManager>().DroneCount.ToString();
        numOfDronesM.text = FindAnyObjectByType<GridManager>().DroneCount.ToString();
        numOfChargers.text = FindAnyObjectByType<GridManager>().ChargerCount.ToString();
    }
}