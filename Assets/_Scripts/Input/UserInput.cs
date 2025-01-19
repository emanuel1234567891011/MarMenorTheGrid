using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInput : MonoBehaviour
{
    [SerializeField] DroneHUDController hud;
    [SerializeField] Color _selectedColor;
    [SerializeField] Button _placeDronesButton;
    [SerializeField] Button _placeChargerButton;
    [SerializeField] Button _selectUnuseableAreaButton;
    [SerializeField] Button _placingWayPointsButton;

    private Drone currentDrone = null;

    void Start()
    {
        PlaceDronesButtonPressed();
    }

    public void PlaceDronesButtonPressed()
    {
        _placeChargerButton.GetComponent<Image>().color = Color.white;
        _placeDronesButton.GetComponent<Image>().color = _selectedColor;
        _selectUnuseableAreaButton.GetComponent<Image>().color = Color.white;
        _placingWayPointsButton.GetComponent<Image>().color = Color.white;
    }

    public void PlaceChargersButtonPressed()
    {
        _placeChargerButton.GetComponent<Image>().color = _selectedColor;
        _placeDronesButton.GetComponent<Image>().color = Color.white;
        _placingWayPointsButton.GetComponent<Image>().color = Color.white;
        _selectUnuseableAreaButton.GetComponent<Image>().color = Color.white;
    }

    public void SetUnuseableAreaButtonsPressed()
    {
        _placeChargerButton.GetComponent<Image>().color = Color.white;
        _placeDronesButton.GetComponent<Image>().color = Color.white;
        _placingWayPointsButton.GetComponent<Image>().color = Color.white;
        _selectUnuseableAreaButton.GetComponent<Image>().color = _selectedColor;
    }


    public void SetWayPointsButtonPressed()
    {
        _placeChargerButton.GetComponent<Image>().color = Color.white;
        _placeDronesButton.GetComponent<Image>().color = Color.white;
        _selectUnuseableAreaButton.GetComponent<Image>().color = Color.white;
        _placingWayPointsButton.GetComponent<Image>().color = _selectedColor;
    }

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
                    Debug.Log("detected drone");
                    currentDrone = hit.collider.GetComponent<Drone>();
                }

                if (hit.collider.GetComponent<UIMapIcon>() != null)
                {
                    return;
                }
                else if (hit.collider.tag == "map")
                {
                    currentDrone = null;
                    hud.Clear();

                    Renderer rend = hit.transform.GetComponent<Renderer>();
                    Texture2D tex = (Texture2D)rend.material.GetTexture("Texture2D_3f3542f4b23c413d97123944acaa3ef7");

                    Vector2 pixelUV = hit.textureCoord;

                    pixelUV.x *= tex.width;
                    pixelUV.y *= tex.height;

                    if (tex.GetPixel((int)pixelUV.x, (int)pixelUV.y) == Color.blue)
                    {
                        GridManager gm = FindAnyObjectByType<GridManager>();
                        DroneManager dm = FindAnyObjectByType<DroneManager>();
                        gm.MapInteracted(new Vector2Int((int)pixelUV.x, (int)pixelUV.y), hit.point);

                        if (gm.placingDrones)
                        {
                            //todo create a panel for drone config edit
                            //todo assign to drone config based on icon index
                            dm.AddDroneConfig(gm.droneIcons.Count - 1);
                        }

                        if (gm.placingWaypoints)
                        {
                            gm.AddWaypoint(new Vector2Int((int)pixelUV.x, (int)pixelUV.y));
                        }
                    }
                }
            }
        }

        if (currentDrone)
            hud.PopulateDroneInfo(currentDrone.Name, currentDrone.Capacity, currentDrone.Battery, currentDrone.Velocity);
    }
}

