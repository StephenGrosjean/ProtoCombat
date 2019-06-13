using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] Transform[] cubes;
    [SerializeField] int force;
    [SerializeField] float breakForce;

    private FragmentManager fragmentManager;

    // Start is called before the first frame update
    void Start()
    {
        fragmentManager = GameObject.Find("FragmentManager").GetComponent<FragmentManager>();
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
        else if(collision.transform.tag != "Fragment"){
            if(GetComponent<Rigidbody>().GetPointVelocity(collision.GetContact(0).point).magnitude >= breakForce) {
                EnableObjectAndBreak(GetComponent<Rigidbody>().GetPointVelocity(collision.GetContact(0).point));
            }
        }
    }

    void EnableObjectAndExplode(Vector3 forcePosition, Rigidbody collisionRigid) {
        transform.DetachChildren();
        foreach (Transform cube in cubes) {
            cube.gameObject.SetActive(true);
            fragmentManager.AddFragment(cube.gameObject);

            if (collisionRigid.gameObject.GetComponent<TankShell>().TypeShell == TankShell.ShellType.Small) {
                cube.GetComponent<Rigidbody>().AddExplosionForce(collisionRigid.velocity.magnitude * force, forcePosition, 3);
            }
            else{
                cube.GetComponent<Rigidbody>().AddExplosionForce(collisionRigid.velocity.magnitude * force * 100, forcePosition, 1);
            }
        }
        Destroy(gameObject);
    }

    void EnableObjectAndBreak(Vector3 collisionVelocity) {
        transform.DetachChildren();
        foreach (Transform cube in cubes) {
            cube.gameObject.SetActive(true);
            fragmentManager.AddFragment(cube.gameObject);
            cube.gameObject.GetComponent<Rigidbody>().velocity = collisionVelocity;
        }
        Destroy(gameObject);
    }
}
