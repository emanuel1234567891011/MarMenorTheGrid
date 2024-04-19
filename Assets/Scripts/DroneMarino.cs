using UnityEngine;
using System.Collections.Generic;

public class DroneMarino : Drone
{
    // Define the states for the FSM
    public enum State
    {
        Stop,
        Navigate,
        EmptyReadyToRecharge
    }

    public State currentState = State.Navigate;
    public string type;
    public float speed = 10f;
    public float turnSpeed = 10f;
    public Vector3 chargingPoint; // The location of the charging point
    public List<Vector3> positions = new List<Vector3>(); // List to store the positions

    private Rigidbody rb;
    private bool isWorking = false;
    public float batteryTime = 100f; // The battery time in seconds

    // Constructor
    public DroneMarino(string name, float speed, float battery, int capacity) : base(name, speed, battery, capacity)
    {
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void StartWork()
    {
        isWorking = true;
        currentState = State.Navigate; // Set the drone to navigate when work starts
    }

    public override void StopWork()
    {
        isWorking = false;
        rb.velocity = Vector3.zero;
    }

    void Update()
    {
        // Decrease the battery time
        batteryTime -= Time.deltaTime;

        // Define the target position
        Vector3 targetPosition = new Vector3(0, transform.position.y, 699);

        // Add the current position to the list
        positions.Add(transform.position);

        switch (currentState)
        {
            case State.Stop:
                rb.velocity = Vector3.zero;
                break;

            case State.Navigate:
                if (isWorking)
                {
                    // Move towards the target position
                    Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                    rb.velocity = directionToTarget * speed;

                    // Check if the drone is close enough to the target position (e.g., within a 1-meter radius)
                    if (Vector3.Distance(transform.position, targetPosition) < 1f)
                    {
                        // If close to the target, stop the drone by switching to the Stop state
                        currentState = State.Stop;
                        StopWork(); // Optionally call StopWork if you want to completely halt operations
                    }
                }
                break;

            case State.EmptyReadyToRecharge:
                // Move towards the charging point
                Vector3 chargingDirection = (chargingPoint - transform.position).normalized;
                rb.velocity = chargingDirection * speed;

                // Check if the drone has reached the charging point
                if (Vector3.Distance(transform.position, chargingPoint) < 1f)
                {
                    // Simulate a faster recharging process
                    batteryTime += 50f * Time.deltaTime;

                    // Check if the battery is full
                    if (batteryTime >= 100f)
                    {
                        batteryTime = 100f;
                        currentState = State.Navigate; // Change back to Navigate to resume operation
                    }
                }
                break;
        }
    }
}