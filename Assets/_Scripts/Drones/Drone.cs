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
    public Vector2Int StartingCell;
    public float Speed;
    public float Battery;
    public int Capacity;
    public Color32 TraversableColor;

    public Color32 GetTraversableColor => TraversableColor;
    public List<TraversableArea> area = new List<TraversableArea>();

    public virtual void Initialize(Color32 tColor)
    {
        TraversableColor = tColor;
    }

    public virtual void SetTraversableArea(List<TraversableArea> a)
    {
        area = a;
    }

    public virtual void MoveToLocation(Vector3 pos)
    {

    }

    public virtual void StopInLocation()
    {

    }
}
