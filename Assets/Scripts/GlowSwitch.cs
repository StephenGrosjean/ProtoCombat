using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowSwitch : MonoBehaviour
{
    [SerializeField] private int glowDivider;
    private Material material;
    private Color baseColor;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        baseColor = material.GetColor("_EmissionColor");
        
    }

    // Update is called once per frame
    void Update()
    {
        material.SetColor("_EmissionColor", Color.Lerp(baseColor, baseColor / glowDivider, Mathf.Sin(Time.time)));
    }
}
