using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Drone : MonoBehaviour
{
    public string Name;
    public float Speed; // Speed
    public float Battery;   // Battery
    public int Capacity;   // Capacity
    public string Behavior; // Behavior

    public Rigidbody TargetRigidbody; 
    public float DepthBeforeSub; 
    public float DisplacementAmount; 
    public int floaters; 
    public float WaterDrag; 
    public float WaterAngularDrag; 
    public WaterSurface Water; 
    private WaterSearchParameters _search; 
    private WaterSearchResult _searchResult; 

    public Drone(string name, float speed, float battery, int capacity)
    {
        Name = name;
        Speed = speed;
        Battery = battery;
        Capacity = capacity;
    }

    public virtual void StartWork()
    {
        // Implementation of starting drone work
    }

    public virtual void StopWork()
    {
        // Implementation of stopping drone work
    }

    private void FixedUpdate() 
    {
        TargetRigidbody.AddForceAtPosition(Physics.gravity / floaters, transform.position, ForceMode.Acceleration); 

        _search.startPositionWS = transform.position; 
        Water.ProjectPointOnWaterSurface(_search, out _searchResult); 

        if (transform.position.y < _searchResult.projectedPositionWS.y)
        {
            float displacementMulti = Mathf.Clamp01((_searchResult.projectedPositionWS.y - transform.position.y) / DepthBeforeSub) * DisplacementAmount; 
            TargetRigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMulti, 0f), transform.position, ForceMode.Acceleration); 
            TargetRigidbody.AddForce(displacementMulti * -TargetRigidbody.velocity * WaterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange); 
            TargetRigidbody.AddTorque(displacementMulti * -TargetRigidbody.angularVelocity * WaterAngularDrag * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    public override string ToString()
    {
        return $"Name: {Name}, Capacity: {Capacity} kg, Battery: {Battery} hours, Speed: {Speed} km/h";
    }
}