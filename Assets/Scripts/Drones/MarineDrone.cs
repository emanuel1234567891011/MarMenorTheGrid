using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class MarineDrone : Drone
{
    public Transform debugMoveLocation;

    private Seeker seeker;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        MoveToLocation(debugMoveLocation.position);
    }

    public override void MoveToLocation(Vector3 pos)
    {
        base.MoveToLocation(pos);
        seeker.StartPath(transform.position, pos);
    }

    public override void StopInLocation()
    {
        base.StopInLocation();
    }
}
