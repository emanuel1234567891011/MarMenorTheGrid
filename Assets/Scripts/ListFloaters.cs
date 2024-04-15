using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListFloaters : MonoBehaviour
{
    public List<Rigidbody> Targets; // List of Rigidbody objects that will float
    public float DepthBeforeSubmerged = 1f;
    public float DisplacementAmount = 3f;
    public int FloaterCount = 1; // Number of float points per object

    public float WaterDrag = 0.99f;
    public float WaterAngularDrag = 0.5f;

    public Transform WaterSurface; // Transform representing the surface of the water

    private void FixedUpdate()
    {
        foreach (var targetRigidbody in Targets)
        {
            if (targetRigidbody != null)
            {
                RigidbodyFloating(targetRigidbody);
            }
        }
    }

    private void RigidbodyFloating(Rigidbody rigidbody)
    {
        float waveHeight = WaterSurface.position.y;
        Vector3 gravity = Physics.gravity;
        for (int i = 0; i < FloaterCount; i++)
        {
            Vector3 point = rigidbody.transform.position;
            float forceFactor = 1f - ((point.y - waveHeight) / DepthBeforeSubmerged);

            if (forceFactor > 0f)
            {
                float displacementMultiplier = Mathf.Clamp01(forceFactor) * DisplacementAmount;
                Vector3 buoyancy = -gravity * displacementMultiplier / FloaterCount;
                rigidbody.AddForceAtPosition(buoyancy, point, ForceMode.Acceleration);
            }
        }

        // Apply simple drag
        rigidbody.velocity *= WaterDrag;
        rigidbody.angularVelocity *= WaterAngularDrag;
    }
}
