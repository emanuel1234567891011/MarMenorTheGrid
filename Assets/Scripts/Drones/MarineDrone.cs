using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Pathfinding;
using UnityEngine;

public class MarineDrone : Drone
{
    public MeshRenderer droneMesh;
    private Seeker seeker;
    private MapCellData[,] mapData;
    private int totalTraversableCells;
    private int traversedCells = 0;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
    }

    //todo drone movement logic
    //todo choose a direction to move in
    //todo each movement, check against the data and check the color of that mapcelldata

    public override void Initialize(Color tColor, MapCellData[,] data)
    {
        base.Initialize(tColor, data);
        droneMesh.material.color = tColor;
        mapData = data;
    }

    //todo figure out movement logic.

    public override void MoveToLocation(Vector3 pos)
    {
        base.MoveToLocation(pos);
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
