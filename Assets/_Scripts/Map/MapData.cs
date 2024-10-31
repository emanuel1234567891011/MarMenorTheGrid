using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Map Data", order = 1)]
public class MapData : ScriptableObject
{
    public string mapName;
    public Texture2D bitmap;
    public Texture2D overlayMap;
    public int mapWidth;
    public int mapHeight;
    public string description;
}
