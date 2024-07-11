using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DroneCharger : MonoBehaviour
{
    public MeshRenderer mesh;

    private Color startingColor;

    private void Start()
    {
        startingColor = mesh.material.color;
    }

    void Update()
    {
        //todo display value in text mesh object
    }

    public void Charge(float duration)
    {
        mesh.material.color = Color.red;
        mesh.material.DOColor(Color.green, duration).onComplete += () => mesh.material.color = startingColor;
    }
}
