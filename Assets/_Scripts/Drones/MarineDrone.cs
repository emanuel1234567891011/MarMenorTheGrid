using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Pathfinding;
using UnityEngine;

public class MarineDrone : Drone
{
    public MeshRenderer droneMesh;
    private Seeker seeker;
    private int totalTraversableCells;
    private int traversedCells = 1;
    private GridManager gridManager;


    private void Start()
    {
        seeker = GetComponent<Seeker>();
    }

    private void Update()
    {
        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
            return;
        }

        if (area.Count == 0)
            return;

        if (traversedCells < 2)
        {
            transform.position = gridManager.GetIndexPosition(area[traversedCells].index);
            traversedCells++;
        }
    }

    public override void Initialize(Color32 tColor)
    {
        base.Initialize(tColor);
        droneMesh.material.color = tColor;
    }

    public override void MoveToLocation(Vector3 pos)
    {
        base.MoveToLocation(pos);
    }

    public override void SetTraversableArea(List<TraversableArea> a)
    {
        base.SetTraversableArea(a);

    }

    public override void StopInLocation()
    {
        base.StopInLocation();
    }

    private float CoverageProgress()
    {
        return traversedCells / totalTraversableCells;
    }
}
