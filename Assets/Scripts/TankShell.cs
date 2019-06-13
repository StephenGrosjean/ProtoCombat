using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : MonoBehaviour
{

    public enum ShellType {
        Small,
        Large
    }
    
    [SerializeField] private float speed = 10;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private ShellType typeShell;
    public ShellType TypeShell { get { return typeShell; } set { typeShell = value; } }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(-Vector3.left * speed);
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter() {
        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Destroy(gameObject, 0.1f);
    }
}
