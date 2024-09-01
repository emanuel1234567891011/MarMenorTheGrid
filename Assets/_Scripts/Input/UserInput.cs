using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInput : MonoBehaviour
{
    [SerializeField] DroneHUDController hud;
    private Drone currentDrone = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<Drone>() != null)
                {
                    currentDrone = hit.collider.GetComponent<Drone>();
                }
                else if (hit.collider.tag == "map")
                {
                    Renderer rend = hit.transform.GetComponent<Renderer>();
                    Texture2D tex = rend.material.mainTexture as Texture2D;
                    Vector2 pixelUV = hit.textureCoord;
                    pixelUV.x *= tex.width;
                    pixelUV.y *= tex.height;

                    if (tex.GetPixel((int)pixelUV.x, (int)pixelUV.y) == Color.blue)
                    {
                        FindAnyObjectByType<GridManager>().SpawnDebugCubeAtLocation(new Vector2Int((int)pixelUV.x, (int)pixelUV.y));
                    }
                }
                else
                {
                    currentDrone = null;
                    hud.Clear();
                }
            }
        }

        if (currentDrone)
            hud.PopulateDroneInfo(currentDrone.Name, currentDrone.Capacity, currentDrone.Battery, currentDrone.Speed);
    }

}

