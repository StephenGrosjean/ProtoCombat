using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankColor : MonoBehaviour
{
    [SerializeField] private List<Renderer> neons;

    public void SetColor(Color color) {
        foreach(Renderer neon in neons) {
            neon.material.color = color;
            neon.material.SetColor("_EmissionColor", color);

        }
    }
}
