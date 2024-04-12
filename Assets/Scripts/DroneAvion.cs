using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAvion : Drone
{
    // Marine-specific properties and methods
    public string type; // Replace with the actual property
    public Transform target; // The target that the drone will move towards
    public float speed = 5f; // The speed at which the drone will move
    public float turnSpeed = 5f; // The speed at which the drone will turn

    private Rigidbody rb; // The Rigidbody component attached to the drone
    private bool isWorking = false; // Flag to determine if the drone is working

    // Constructor
    public DroneAvion(string name, float speed, float battery, int capacity) : base(name, speed, battery, capacity)
    {
    }

    void Awake()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    public override void StartWork()
    {
        isWorking = true;
        // Additional start work logic if necessary
    }

    public override void StopWork()
    {
        isWorking = false;
        rb.velocity = Vector3.zero; // Stop the drone
    }

    void Update()
    {
        if (isWorking && target != null)
        {
            // Move towards the target
            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }
    }
}
