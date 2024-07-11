using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

public class Drone : MonoBehaviour
{
    //This is the base class for all drones. The functionlity in this class will
    //be accessible by ALL drones. We add basic functionality here, and we add
    //unique functionality to the derived classes. (Marine, Air)

    public string Name;
    public float Speed;
    public float ChargeDuration = 10;
    public float Battery;
    public int Capacity;
    public Color32 TraversableColor;
    public List<MapCellData> TraversableCells = new List<MapCellData>();
    public DroneCharger Charger;

    private void Start()
    {
        // Name = "Drone " + Random.Range(0, 1000).ToString();
        // Speed = Random.Range(0, 1000);
        // Battery = Random.Range(0, 100);
        // Capacity = Random.Range(0, 100);
    }

    public virtual void Initialize(Color32 tColor)
    {
        // Name = "Drone " + Random.Range(0, 1000).ToString();
        // Speed = Random.Range(0, 1000);
        // Battery = Random.Range(0, 100);
        // Capacity = Random.Range(0, 100);

        TraversableColor = tColor;
    }

    public virtual void SetTraversableCells(List<MapCellData> a)
    {
        TraversableCells = a;
    }
}

