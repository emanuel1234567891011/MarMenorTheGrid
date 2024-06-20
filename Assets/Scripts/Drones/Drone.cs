using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Drone : MonoBehaviour
{
    //This is the base class for all drones. The functionlity in this class will
    //be accessible by ALL drones. We add basic functionality here, and we add
    //unique functionality to the derived classes. (Marine, Air)

    public string Name;
    public float Speed;
    public float Battery;
    public int Capacity;
    public Color TraversableColor;

    public Color GetTraversableColor => TraversableColor;

    public virtual void Initialize(Color tColor, MapCellData[,] data)
    {

    }

    public virtual void MoveToLocation(Vector3 pos)
    {

    }

    public virtual void StopInLocation()
    {

    }
}
