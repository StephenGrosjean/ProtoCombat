using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] Transform[] cubes;
    [SerializeField] int force;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.transform.tag == "Shell") {
            Destroy(collision.gameObject);
            EnableObjectAndExplode(collision.transform.position, collision.transform.GetComponent<Rigidbody>());
        }
    }

    void EnableObjectAndExplode(Vector3 forcePosition, Rigidbody collisionRigid) {
        transform.DetachChildren();
        foreach (Transform cube in cubes) {
            cube.gameObject.SetActive(true);
            cube.GetComponent<Rigidbody>().AddExplosionForce(collisionRigid.velocity.magnitude*force, forcePosition, 3);
        }
        Destroy(gameObject);
    }
}
