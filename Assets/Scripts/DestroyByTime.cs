using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Destroy by time script
/// </summary>
public class DestroyByTime : MonoBehaviour
{
    [SerializeField] private float time = 2; //Time to destroy

    void Start()
    {
        Destroy(gameObject, time);   
    }
}
