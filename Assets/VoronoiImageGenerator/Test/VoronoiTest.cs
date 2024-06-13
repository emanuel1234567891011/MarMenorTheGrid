using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoronoiTest : MonoBehaviour
{
    public Vector2Int size;
    public List<Vector2> coors;
    public List<Color> colors;
    public List<float> strength;

    private int distanceFormula = 0;
    private float minkowskiP = 1.5f;
    private bool wrapHorizontal = false;
    private bool wrapVertical = false;
    private float siteRadius = 0.005f;

    public void ChangeDistanceMetric()
    {
        distanceFormula = GameObject.Find("Dropdown").GetComponent<Dropdown>().value;
        GenerateVoronoi();
    }

    public void ChangeMinkowskiP()
    {
        minkowskiP = 0.5f + GameObject.Find("Slider (1)").GetComponent<Slider>().value * 0.25f;
        GameObject.Find("MinkowskiP").GetComponent<Text>().text = minkowskiP.ToString();
        if(distanceFormula == 3)
            GenerateVoronoi();
    }

    public void ToggleWrapH()
    {
        wrapHorizontal = GameObject.Find("Toggle").GetComponent<Toggle>().isOn;
        GenerateVoronoi();
    }

    public void ToggleWrapV()
    {
        wrapVertical = GameObject.Find("Toggle (1)").GetComponent<Toggle>().isOn;
        GenerateVoronoi();
    }

    public void ChangeSiteRadius()
    {
        siteRadius = GameObject.Find("Slider (2)").GetComponent<Slider>().value * 0.0025f;
        GameObject.Find("SiteRadius").GetComponent<Text>().text = siteRadius.ToString();
        GenerateVoronoi();
    }

    public void ChangeTilingX()
    {
        float scale = 0.5f + GameObject.Find("Slider (3)").GetComponent<Slider>().value*0.5f;

        GameObject.Find("VoronoiTest").GetComponent<MeshRenderer>().material.mainTextureScale =
            new Vector2(scale, GameObject.Find("VoronoiTest").GetComponent<MeshRenderer>().material.mainTextureScale.y);

        GameObject.Find("TileX").GetComponent<Text>().text = scale.ToString();
    }

    public void ChangeTilingY()
    {
        float scale = 0.5f + GameObject.Find("Slider (4)").GetComponent<Slider>().value*0.5f;

        GameObject.Find("VoronoiTest").GetComponent<MeshRenderer>().material.mainTextureScale =
            new Vector2(GameObject.Find("VoronoiTest").GetComponent<MeshRenderer>().material.mainTextureScale.x, scale);

        GameObject.Find("TileY").GetComponent<Text>().text = scale.ToString();
    }

    void Start()
    {
        GenerateSites();
    }

    //Generate Sites and associated colors
    public void GenerateSites()
    {
        size = new Vector2Int(2048,2048);
        coors = new List<Vector2>();
        colors = new List<Color>();
        strength = new List<float>();
        for (int i = 0; i < 100; i++)
        {
            coors.Add(new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y)));
            colors.Add(new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f), 1));
            strength.Add(1);
        }

        GenerateVoronoi();
    }

    private void GenerateVoronoi()
    {
        //Apply Settings
        VoronoiGenerator.distanceFormula = (VoronoiGenerator.DistanceFormula)distanceFormula;
        VoronoiGenerator.minkowskiP = minkowskiP;
        VoronoiGenerator.wrapHorizontal = wrapHorizontal;
        VoronoiGenerator.wrapVertical = wrapVertical;
        VoronoiGenerator.siteRadius = siteRadius;

        //Generate Voronoi
        RenderTexture RT = VoronoiGenerator.GenerateVoronoi(size, coors, colors, strength);

        //Display
        RT.filterMode = FilterMode.Point;
        RT.wrapMode = TextureWrapMode.Repeat;
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", RT);
    }
}
