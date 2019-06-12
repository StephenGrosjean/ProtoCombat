using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] Transform[] cubes;
    [SerializeField] int force;
    [SerializeField] float breakForce;

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
        else {
            if(GetComponent<Rigidbody>().velocity.magnitude >= breakForce) {
                EnableObjectAndBreak();
            }
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

    void EnableObjectAndBreak() {
        transform.DetachChildren();
        foreach (Transform cube in cubes) {
            cube.gameObject.SetActive(true);
            cube.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
        }
        Destroy(gameObject);
    }
}
