using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float minFov = 20;
    [SerializeField] private float maxFov = 80;

    void Awake()
    {
        cam = GetComponent<Camera>();
        // ensures that the field of view is within min and max on startup
        UpdateFov(cam.fieldOfView);
    }

    void Update()
    {
        if (FindAnyObjectByType<GameManager>().Playing && EventSystem.current.currentSelectedGameObject == null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput > 0)
                UpdateFov(cam.fieldOfView - 1);
            else if (scrollInput < 0)
                UpdateFov(cam.fieldOfView + 1);
        }
    }

    void UpdateFov(float newFov)
    {
        cam.fieldOfView = Mathf.Clamp(newFov, minFov, maxFov);
    }
}
