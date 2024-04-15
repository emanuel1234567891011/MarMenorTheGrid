using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarineDrone : MonoBehaviour
{
    public enum State
    {
        Stop,
        Navigate
    }

    public State currentState = State.Navigate;
    public float speed = 1f; // Speed of the movement
    private Vector3 targetPosition; // Target position to move towards
    private Rigidbody rb;
    private bool isWorking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        targetPosition = new Vector3(699, transform.position.y, transform.position.z); // Set target position
    }

    public void StartWork()
    {
        isWorking = true;
        currentState = State.Navigate;
    }

    public void StopWork()
    {
        isWorking = false;
        currentState = State.Stop;
        rb.velocity = Vector3.zero;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Stop:
                rb.velocity = Vector3.zero;
                break;

            case State.Navigate:
                if (isWorking)
                {
                    // Move the object towards the target position
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                    // Optionally, you can destroy or deactivate the object once it reaches the target
                    if (transform.position == targetPosition)
                    {
                        Debug.Log("Reached target!");
                        // Uncomment below line to destroy the object
                        // Destroy(gameObject);
                        currentState = State.Stop;
                        StopWork();
                    }
                }
                break;
        }
    }
}