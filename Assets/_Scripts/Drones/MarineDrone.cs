using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;

public class MarineDrone : Drone
{
    public GameObject debugCube;
    public DroneBehaviour behaviour;
    public MeshRenderer droneMesh;
    public LineRenderer chargeLine;
    private int totalTraversableCells;
    private int traversedCells = 0;
    private GridManager gridManager;
    private DroneManager droneManager;
    public bool debug;
    private bool gridConstructed;
    private List<List<MapCellData>> traversableGrid = new List<List<MapCellData>>();
    private int gridIndex = 0;
    private int gridPositionIndex = 0;
    private bool charging;
    private float startingBattery;
    private float movementDelay = 0;
    private float currentMovementDelay;
    bool movingToStartingPoint = false;

    DroneBehaviour _prevBehaviour;
    int _currentWaypoint = 0;
    float _movementSpeed = 1f;
    float _minDist = .1f;
    List<Vector3> waypointPos = new List<Vector3>();

    GameManager gameManager;

    private void Start()
    {
        startingBattery = Battery;
        droneManager = FindAnyObjectByType<DroneManager>();
        behaviour = DroneBehaviour.None;
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        Debug.Log(behaviour);

        #region references
        if (FindCharger() != null && Charger == null)
        {
            transform.position = FindCharger().transform.position;
            Charger = FindCharger();
        }

        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
            return;
        }

        if (TraversableCells.Count > 1 && gridConstructed == false)
            ContsructGrid();

        if (gridConstructed && gameManager.Playing && behaviour == DroneBehaviour.None)
            behaviour = DroneBehaviour.Initializing;

        #endregion;

        #region  initializing
        if (behaviour == DroneBehaviour.Initializing)
        {
            if (movingToStartingPoint == false)
            {
                movingToStartingPoint = true;
                StartCoroutine(MoveToStartPoint());
            }
        }
        #endregion

        #region idle
        if (behaviour == DroneBehaviour.Idle)
        {

        }
        #endregion

        #region charging
        if (behaviour == DroneBehaviour.Charging)
        {
            if (charging == false)
                StartCoroutine(Recharge());
        }
        #endregion

        #region cleaning
        if (behaviour == DroneBehaviour.Cleaning)
        {
            if (gameManager.Playing && traversedCells < TraversableCells.Count)
            {
                droneMesh.transform.position = transform.position;
                currentMovementDelay += Time.deltaTime;
                movementDelay = 1 / droneManager.metersPS;
                Battery -= Time.deltaTime;
                if (currentMovementDelay > movementDelay)
                {
                    MapCellData d = traversableGrid[gridIndex].ElementAt(gridPositionIndex);
                    Vector2Int coords = new Vector2Int(d.xIndex, d.yIndex);

                    if (Vector3.Distance(transform.position, gridManager.GetIndexPosition(coords)) > .01f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, gridManager.GetIndexPosition(coords), _movementSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.position = gridManager.GetIndexPosition(coords);
                        traversedCells++;

                        if (gridPositionIndex + 1 > traversableGrid[gridIndex].Count - 1)
                        {
                            gridIndex++;
                            gridPositionIndex = 0;
                        }
                        else
                            gridPositionIndex++;

                        droneManager.SpaceCleared();
                        currentMovementDelay = 0;
                    }
                }

                if (TraversableCells.Count == 0)
                    behaviour = DroneBehaviour.Idle;

                if (Battery <= 0)
                {
                    _prevBehaviour = DroneBehaviour.Cleaning;
                    behaviour = DroneBehaviour.Charging;
                }
            }

        }
        #endregion

        #region patrolling
        if (behaviour == DroneBehaviour.Patrolling)
        {
            droneMesh.transform.position = transform.position;
            float d = Vector3.Distance(transform.position, waypointPos[_currentWaypoint]);
            if (d < _minDist)
            {
                if (_currentWaypoint + 1 <= waypointPos.Count - 1)
                    _currentWaypoint++;
                else
                    _currentWaypoint = 0;
            }

            Battery -= Time.deltaTime;

            if (Battery < 0)
            {
                _prevBehaviour = DroneBehaviour.Patrolling;
                behaviour = DroneBehaviour.Charging;
            }

            transform.position = Vector3.MoveTowards(transform.position, waypointPos[_currentWaypoint], _movementSpeed * Time.deltaTime);
        }
        #endregion
    }

    public DroneCharger FindCharger()
    {
        var chargers = FindObjectsByType<DroneCharger>(FindObjectsSortMode.None);
        float max = Mathf.Infinity;
        DroneCharger current = null;
        foreach (var dc in chargers)
        {
            float dist = Vector3.Distance(transform.position, dc.transform.position);
            if (dist < max)
            {
                max = dist;
                current = dc;
            }
        }

        return current;
    }

    IEnumerator MoveToStartPoint()
    {
        MapCellData d = traversableGrid[gridIndex].ElementAt(gridPositionIndex);
        Vector2Int coords = new Vector2Int(d.xIndex, d.yIndex);
        transform.DOMove(gridManager.GetIndexPosition(coords), 5);
        yield return new WaitForSeconds(5);
        behaviour = DroneBehaviour.Cleaning;
    }

    public override void Initialize(Color32 tColor)
    {
        base.Initialize(tColor);
        droneMesh.material.color = tColor;
    }

    public float GetSpeed(float time, int tCells)
    {
        float speed = gridManager.GetCellSizeInMeters() * traversedCells / time;
        return speed;
    }

    public void SetPatrolling()
    {
        behaviour = DroneBehaviour.Patrolling;
    }

    public void SetCleaning()
    {
        behaviour = DroneBehaviour.Cleaning;
    }

    public void SetIdle()
    {
        behaviour = DroneBehaviour.Idle;
    }

    void ContsructGrid()
    {
        gridConstructed = true;
        int index = TraversableCells[0].xIndex;
        int gridIndex = 0;
        traversableGrid.Add(new List<MapCellData>());
        for (int i = 0; i < TraversableCells.Count; i++)
        {
            if (TraversableCells[i].xIndex == index)
                traversableGrid[gridIndex].Add(TraversableCells[i]);
            else
            {
                index++;
                traversableGrid.Add(new List<MapCellData>());
                gridIndex++;
            }
        }

        for (int i = 0; i < traversableGrid.Count; i++)
            if (i % 2 > 0)
                traversableGrid[i].Reverse();
    }


    public override void SetTraversableCells(List<MapCellData> a)
    {
        if (TraversableCells.Count == 0)
            TraversableCells = a;
    }

    private float CoverageProgress()
    {
        return traversedCells / totalTraversableCells;
    }

    private IEnumerator Recharge()
    {
        //chargeLine.SetPosition(0, Charger.transform.position);
        //chargeLine.SetPosition(1, transform.position);
        Charger = FindCharger();
        charging = true;

        while (Vector3.Distance(transform.position, Charger.transform.position) > .1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, Charger.transform.position, _movementSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(5);
        Charger.Charge(ChargeDuration);
        yield return new WaitForSeconds(ChargeDuration);
        Battery = startingBattery;
        MapCellData d = traversableGrid[gridIndex].ElementAt(gridPositionIndex);
        Vector2Int coords = new Vector2Int(d.xIndex, d.yIndex);
        if (_prevBehaviour == DroneBehaviour.Cleaning)
        {
            while (Vector3.Distance(transform.position, gridManager.GetIndexPosition(coords)) > .1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, gridManager.GetIndexPosition(coords), _movementSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while (Vector3.Distance(transform.position, waypointPos[_currentWaypoint]) > .1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypointPos[_currentWaypoint], _movementSpeed * Time.deltaTime);
                yield return null;
            }
        }

        charging = false;
        behaviour = _prevBehaviour;
        //chargeLine.positionCount = 0;
    }

    public void SetWaypoints(List<Vector2Int> w)
    {
        foreach (Vector2Int v2 in w)
        {
            Vector3 v3 = gridManager.GetIndexPosition(v2);
            waypointPos.Add(v3);
            Instantiate(debugCube, v3, Quaternion.identity);
        }
    }
}

public enum DroneBehaviour
{
    None,
    Initializing,
    Idle,
    Charging,
    Cleaning,
    Patrolling
}
