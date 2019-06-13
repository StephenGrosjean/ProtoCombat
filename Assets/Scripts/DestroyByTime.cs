using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    [SerializeField] private float time = 2;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, time);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
