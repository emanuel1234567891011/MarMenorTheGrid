using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using Unity.Mathematics;

public class DroneCharger : MonoBehaviour
{
    public MeshRenderer mesh;
    public TextMeshPro tmp;

    private Color startingColor;
    private float currentDuration;
    private float totalDuration;

    private void Start()
    {
        startingColor = mesh.material.color;
        tmp.enabled = false;
    }

    void Update()
    {
        if (currentDuration > 0)
            currentDuration -= Time.deltaTime;

        tmp.text = Mathf.RoundToInt(currentDuration).ToString();
    }

    public void Charge(float duration)
    {
        tmp.enabled = true;
        totalDuration = duration;
        currentDuration = totalDuration;
        mesh.material.color = Color.red;
        mesh.material.DOColor(Color.green, duration).onComplete += () =>
        {
            mesh.material.color = startingColor;
            tmp.enabled = false;
        };
    }
}
