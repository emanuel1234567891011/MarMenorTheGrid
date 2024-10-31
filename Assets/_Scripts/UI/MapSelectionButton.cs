using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionButton : MonoBehaviour
{
    [SerializeField] Image _mapImage;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _widthText;
    [SerializeField] TextMeshProUGUI _heightText;
    [SerializeField] Image _selectedImage;
    [SerializeField] TextMeshProUGUI _descText;

    DefaultValues _controller;

    public void Init(MapData data, DefaultValues controller)
    {
        _controller = controller;

        _nameText.text = data.mapName;
        _heightText.text = "Height: " + data.mapHeight.ToString();
        _widthText.text = "Width: " + data.mapWidth.ToString();
        _descText.text = data.description;
        Sprite s = Sprite.Create(data.overlayMap, new Rect(0, 0, _mapImage.rectTransform.sizeDelta.x, _mapImage.rectTransform.sizeDelta.y), Vector3.zero, 100);
        _mapImage.sprite = s;
        GetComponentInChildren<Button>().onClick.AddListener(() => FindAnyObjectByType<GridManager>().mapData = data);
        GetComponentInChildren<Button>().onClick.AddListener(() => _controller.MapButtonClicked(this));
    }

    public void Select()
    {
        _selectedImage.enabled = true;
    }

    public void Deselect()
    {
        _selectedImage.enabled = false;
    }
}
