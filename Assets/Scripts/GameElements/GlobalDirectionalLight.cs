using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameElements
{
    public class GlobalDirectionalLight : MonoBehaviour
    {
        [SerializeField] private float hueShiftSpeed = 0.1f;
        
        private Light2D _light2D;
        private float baseS, baseV;
        private float currentHue;

        private void Awake()
        {
            _light2D = GetComponent<Light2D>();
            Color.RGBToHSV(_light2D.color, out var h, out baseS, out baseV);
        }

        private void Update()
        {
            currentHue += hueShiftSpeed * Time.deltaTime;
            currentHue %= 1;
            _light2D.color = Color.HSVToRGB(currentHue, baseS, baseV);
        }
    }
}
