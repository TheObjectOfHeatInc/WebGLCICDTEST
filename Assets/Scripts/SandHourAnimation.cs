using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class SandHourAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image image;
    private int frame = 0;

    private void Reset()
    {
        image = GetComponent<Image>();
    }
    
    private void Update()
    {
        image.sprite = sprites[frame];
        frame = DateTime.Now.Second % sprites.Length;
    }
}
