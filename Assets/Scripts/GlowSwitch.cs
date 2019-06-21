using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to make objects change in glow
/// </summary>

public class GlowSwitch : MonoBehaviour
{
    [SerializeField] private int glowDivider; //Value to divide glow
    private Material material; //Material of the object
    private Color baseColor; //Base color of the object

    void Start()
    {
        material = GetComponent<Renderer>().material; //Get material
        baseColor = material.GetColor("_EmissionColor"); //Get emissionColor
        
    }

    void Update()
    {
        material.SetColor("_EmissionColor", Color.Lerp(baseColor, baseColor / glowDivider, Mathf.Sin(Time.time))); //Change Glow intensity
    }
}
