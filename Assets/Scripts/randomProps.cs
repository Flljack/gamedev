using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class randomProps : MonoBehaviour
{
    public List<Sprite> spritesList;
    public GameObject randomSpot;

    private Texture _imageCurrent;
    private RawImage _rawImage;
    private RectTransform _rectTransform;
    // Start is called before the first frame update
    private void Start()
    {
        getCurrentImage();
        changeImage();
    }

    private void getCurrentImage()
    {
        _rawImage = randomSpot.GetComponent<RawImage>();
        _rectTransform = randomSpot.GetComponent<RectTransform>();
        _imageCurrent = _rawImage.texture;
    }

    private void changeImage()
    {
        _imageCurrent = spritesList[Random.Range(0, spritesList.Count - 1)].texture;
        _rectTransform.sizeDelta = new Vector2(_imageCurrent.width, _imageCurrent.height);
        _rawImage.texture = _imageCurrent;
    }
}
