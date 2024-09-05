using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class MarineDrone : Drone
{
    public MeshRenderer droneMesh;
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

    private void Start()
    {
        startingBattery = Battery;
        droneManager = FindAnyObjectByType<DroneManager>();
    }

    private void Update()
    {
        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
            return;
        }


        if (charging || TraversableCells.Count == 0)
            return;

        if (TraversableCells.Count > 0 && gridConstructed == false)
            ContsructGrid();

        if (GameManager.Instance.Playing && traversedCells < TraversableCells.Count)
        {
            currentMovementDelay += Time.deltaTime;
            movementDelay = 1 / droneManager.metersPS;

            if (currentMovementDelay > movementDelay)
            {
                MapCellData d = traversableGrid[gridIndex].ElementAt(gridPositionIndex);
                Vector2Int coords = new Vector2Int(d.xIndex, d.yIndex);
                transform.position = gridManager.GetIndexPosition(coords);
                traversedCells++;

                if (gridPositionIndex + 1 > traversableGrid[gridIndex].Count - 1)
                {
                    gridIndex++;
                    gridPositionIndex = 0;
                }
                else
                    gridPositionIndex++;


                Battery -= Time.deltaTime;

                if (Battery < 0)
                    StartCoroutine(Recharge());

                droneManager.SpaceCleared();

                currentMovementDelay = 0;
            }
            else
                return;
        }
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

    void ContsructGrid()
    {
        gridConstructed = true;
        int index = TraversableCells[0].xIndex;
        int gridIndex = 0;
        traversableGrid.Add(new List<MapCellData>());
        for (int i = 0; i < TraversableCells.Count; i++)
        {
            if (TraversableCells[i].xIndex == index)
            {
                traversableGrid[gridIndex].Add(TraversableCells[i]);
            }
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
        base.SetTraversableCells(a);

    }

    private float CoverageProgress()
    {
        return traversedCells / totalTraversableCells;
    }

    private IEnumerator Recharge()
    {
        Charger = FindCharger();

        charging = true;
        float distance = Vector3.Distance(transform.position, Charger.transform.position);
        droneMesh.transform.DOMove(Charger.transform.position, distance);
        yield return new WaitForSeconds(distance);
        Charger.Charge(ChargeDuration);
        yield return new WaitForSeconds(ChargeDuration);
        Battery = startingBattery;
        MapCellData d = traversableGrid[gridIndex].ElementAt(gridPositionIndex);
        Vector2Int coords = new Vector2Int(d.xIndex, d.yIndex);
        droneMesh.transform.DOMove(gridManager.GetIndexPosition(coords), distance);
        yield return new WaitForSeconds(distance + .25f);
        droneMesh.transform.position = transform.position;
        charging = false;
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
