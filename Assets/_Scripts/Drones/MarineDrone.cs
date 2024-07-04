using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class MarineDrone : Drone
{
    public MeshRenderer droneMesh;
    private Seeker seeker;
    private int totalTraversableCells;
    private int traversedCells = 0;
    private GridManager gridManager;
    private DroneManager droneManager;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
    }

    private void FixedUpdate()
    {
        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
            return;
        }

        if (TraversableCells.Count == 0)
            return;

        if (GameManager.Instance.Playing && traversedCells < TraversableCells.Count)
        {
            Vector2Int coords = new Vector2Int(TraversableCells[traversedCells].xIndex, TraversableCells[traversedCells].yIndex);
            transform.position = gridManager.GetIndexPosition(coords);
            traversedCells++;

            if (droneManager == null)
                droneManager = FindAnyObjectByType<DroneManager>();

            droneManager.SpaceCleared();
        }
    }

    //todo given a list of coordinates (left to right, top to bottom) how can we process these coordinates and move the drone logically
    //todo and realistically over the traversable area?

    //todo consider figuring out how to pass in a 2d grid rather than a list? reconstruct 2d grid from list? order list?

    public override void Initialize(Color32 tColor)
    {
        base.Initialize(tColor);
        droneMesh.material.color = tColor;
    }

    public override void MoveToLocation(Vector3 pos)
    {
        base.MoveToLocation(pos);
    }

    public override void SetTraversableCells(List<MapCellData> a)
    {
        base.SetTraversableCells(a);

    }

    public override void StopInLocation()
    {
        base.StopInLocation();
    }

    private float CoverageProgress()
    {
        return traversedCells / totalTraversableCells;
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = TraversableColor;

    //     if (TraversableCells.Count == 0)
    //         return;

    //     for (int i = 0; i < TraversableCells.Count; i++)
    //     {
    //         Gizmos.DrawWireCube(new Vector3(TraversableCells[i].xPos, 0, TraversableCells[i].zPos), new Vector3(.1f, .1f, .1f));
    //     }
    // }
}
