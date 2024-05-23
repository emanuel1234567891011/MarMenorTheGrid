using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;



    // Call this method when the add drone UI button is pressed
    public void AddDrone()
    {
        // You will need to implement a way to choose or create a drone to add
        // gameManager.AddDrone(droneInstance);
    }

    // Call this method when the remove drone UI button is pressed
    public void RemoveDrone()
    {
        // You will need to implement a way to choose which drone to remove
        // gameManager.RemoveDrone(droneInstance);
    }
}
