using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    [SerializeField] DroneHUDController hud;

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<Drone>() != null)
                {
                    var d = hit.collider.GetComponent<Drone>();
                    hud.PopulateDroneInfo(d.Name, d.Capacity, d.Battery, 1.5f);
                }
            }
            else
            {
                hud.Clear();
            }
        }
    }
}
