using UnityEngine;

public class Drone : MonoBehaviour
{
    public string Name;
    public float Speed; // Speed
    public float Battery;   // Battery
    public int Capacity;   // Capacity
    public string Behavior; // Behavior

    

    // You can remove WaterSearchParameters and WaterSearchResult if they were only used for floating logic

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

    // FixedUpdate is no longer needed if it was only used for floating logic
    // If there are other behaviors in FixedUpdate that don't relate to floaters,
    // they should be left in place or adjusted as necessary.

    public override string ToString()
    {
        return $"Name: {Name}, Capacity: {Capacity} kg, Battery: {Battery} hours, Speed: {Speed} km/h";
    }
}
